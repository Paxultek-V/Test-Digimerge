using System;
using Features.Experimental.Scripts.Leaderboard;
using Features.Leaderboard.WeeklyRace;
using Features.Utility;
using UnityEngine;

namespace Features.Leaderboard.Recyclable_Scroll_Rect
{
    public static class LeaderboardTimerManager
    {
        
        private const string joinedDailyRaceKey = "JoinedDailyRaceOn";
        
        public static TimeSpanUntil TimeToReset = new TimeSpanUntil
        {
            Years = -1,
            Months = -1,
            Days = -1,
            Hours = 21,
            Minutes = 0,
            Seconds = 0
        };

        public static TimeSpan GetTimeUntil(int year = -1, int month = -1, int day = -1, int hour = -1, int minute = -1, int second = -1)
        {
            var now = DateTime.Now;
            var targetTime = new DateTime(
                year == -1 ? now.Year : year,
                month == -1 ? now.Month : month,
                day == -1 ? now.Day : day,
                hour == -1 ? now.Hour : hour,
                minute == -1 ? now.Minute : minute,
                second == -1 ? now.Second : second
            );
            var timeUntilTarget = targetTime - now;
            if (timeUntilTarget.TotalMilliseconds < 0)
            {
                timeUntilTarget = timeUntilTarget.Add(TimeSpan.FromDays(1));
            }
            return timeUntilTarget;
        }


        /// <summary>
        /// Year, Month, Day, Hour, Minute, Second
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static TimeSpan GetTimeUntil(params int[] time)
        {
            if (time.Length <= 0 || time.Length > 6)
            {
                throw new ArgumentException("Invalid number of arguments. Must be between 1 and 6.");
            }
            return GetTimeUntil(time[0], time.Length > 1 ? time[1] : -1, time.Length > 2 ? time[2] : -1, time.Length > 3 ? time[3] : -1, time.Length > 4 ? time[4] : -1, time.Length > 5 ? time[5] : -1);
        }

        /// <summary>
        /// Year, Month, Day, Hour, Minute, Second
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static TimeSpan GetTimeUntil(this TimeSpanUntil timeSpanUntil)
        {
            return GetTimeUntil(timeSpanUntil.Years, timeSpanUntil.Months, timeSpanUntil.Days, timeSpanUntil.Hours, timeSpanUntil.Minutes, timeSpanUntil.Seconds);
        }

        public static DateTime GetDateTimeFromNow(this TimeSpanUntil timeSpan)
        {
            TimeSpan timeSpan1 = timeSpan.GetTimeUntil();
            return DateTime.Now.Add(timeSpan1);
        }

        public static string GetTimeFormatted(this TimeSpan timeSpan)
        {
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (hours > 0)
            {
                return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            }
            else
            {
                return $"{minutes:D2}:{seconds:D2}";
            }
        }

        public static string GetDayNameOfTimeSpan(this TimeSpan timeSpan)
        {
            return DateTime.Now.Add(timeSpan).DayOfWeek.ToString();
        }

        public static bool PlayerJoinedTodaysRace()
        {
            // Assuming that joining today's race is determined by the player having a recorded
            // time for today, we can utilize the PlayerRecordedToday method to check this.
            return PlayerRecordedToday();
        }

        public static bool PlayerRecordedToday()
        {
            DateTime lastRecordedDate = GetPlayerLastRecordedDate();

            // Get the reset time for today and yesterday
            DateTime resetTimeToday = TimeToReset.GetDateTimeFromNow();
            DateTime resetTimeYesterday = resetTimeToday.AddDays(-1);

            // Check if the last recorded date falls within today according to the reset time
            return (lastRecordedDate >= resetTimeYesterday && lastRecordedDate < resetTimeToday);
        }



        public static string GetCurrentDateText()
        {
            return DateTime.Now.Date.ToLongDateString();
        }

        public static string GetTimeRemainingText(this TimeSpan timeSpan)
        {
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (hours > 0)
            {
                return $"{hours}h {minutes}m {seconds}s left!";
            }
            else
            {
                return $"{minutes}m {seconds}s left!";
            }
        }

        public static DateTime GetPlayerLastRecordedDate()
        {
            if (PlayerPrefs.HasKey("LastRecordedDate"))
            {
                return DateTime.Parse(PlayerPrefs.GetString("LastRecordedDate"));
            }
            else
            {
                // Returning MinValue if there's no recorded date to indicate that the player
                // has never recorded a time.
                return DateTime.MinValue;
            }
        }

        public static void SetPlayerLastRecordedDate(DateTime date)
        {
            PlayerPrefs.SetString("LastRecordedDate", date.ToString("o")); // Using a round-trip date/time pattern
            PlayerPrefs.Save(); // Ensure the changes are saved immediately
        }
        
        public static void SetTodayAs(WeeklyRaceStatus status)
        {
            SetDateAsJoined(DateTime.Today, (int)status);
        }

        public static void ResetPlayerRecordedDate()
        {
            PlayerPrefs.DeleteKey("LastRecordedDate");
            PlayerPrefs.Save();
            LeaderboardAccess.PlayerLeaderboardScore = 0;
        }

        public static bool ShowAtTheEnd()
        {
            bool ShowAtTheEnd = PlayerJoinedTodaysRace();
            return ShowAtTheEnd;
        }

        public static bool IsShowRewardedVideo = true;
        
        public static bool ShowRewardedVideo()
        {
            bool show = false;
            if (IsShowRewardedVideo)
            {
                show = PlayerJoinedTodaysRace();
            }
            return show;
        }

        public static void SetThisWeekDaysAsJoined(params WeeklyRaceStatus[] days)
        {
            DateTime[] weekDates = GetThisWeekStartingFromMonday();
            for (int i = 0; i < 7; i++)
            {
                SetDateAsJoined(weekDates[i], (int)days[i]);
            }
        }

        public static WeeklyRaceStatus[] GetThisWeekJoinedRaceStatus()
        {
            WeeklyRaceStatus[] joinedRaceStatus = new WeeklyRaceStatus[7];
            DateTime[] weekDates = GetPastWeekStartingFromMonday(1);
            for (int i = 0; i < 7; i++)
            {
                joinedRaceStatus[i] = GetDailyRaceJoinedDataPlayerRecordedDate(weekDates[i]);
            }

            string debugMessage = "Last Played Days \n";

            for (int i = 0; i < joinedRaceStatus.Length; i++)
            {
                debugMessage += $"Day {weekDates[i].ToString("o")} : {joinedRaceStatus[i]}\n";
            }

            Debug.Log(debugMessage);




            return joinedRaceStatus;
        }

        public static void SetDateAsJoined(DateTime date, int joined)
        {
            string key = joinedDailyRaceKey + date.ToString("o");
            PlayerPrefs.SetInt(key, joined);
            PlayerPrefs.Save();
            Debug.Log("Setting date as joined: " + key + " : " + joined);
        }


        private static WeeklyRaceStatus GetDailyRaceJoinedDataPlayerRecordedDate(DateTime weekDate)
        {
            bool dayHasPassed = DateTime.Now.Date > weekDate.Date;
            bool isToday = DateTime.Now.Date == weekDate.Date;
            bool hasKey = PlayerPrefs.HasKey(joinedDailyRaceKey + weekDate.ToString("o"));

            if (isToday)
            {
                if (hasKey)
                {
                    return (WeeklyRaceStatus)PlayerPrefs.GetInt(joinedDailyRaceKey + weekDate.ToString("o"));
                }
                else
                {
                    return WeeklyRaceStatus.NotComeYet;
                }
            }
            else
            {
                if (dayHasPassed)
                {
                    if (hasKey)
                    {
                        return (WeeklyRaceStatus)PlayerPrefs.GetInt(joinedDailyRaceKey + weekDate.ToString("o"));
                    }
                    else
                    {
                        return WeeklyRaceStatus.NotJoined;
                    }
                }
                else
                {
                    return WeeklyRaceStatus.NotComeYet;
                }
            }
        }

        public static DateTime[] GetThisWeekStartingFromMonday()
        {
            DateTime[] weekDates = new DateTime[7];
            DateTime today = DateTime.Now;
            int daysFromMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime monday = today.AddDays(-daysFromMonday).Date;  // Change here to subtract days

            for (int i = 0; i < 7; i++)
            {
                weekDates[i] = monday.AddDays(i);
            }

            string debugMessage = "Getting week days : ";
    
            for (int i = 0; i < weekDates.Length; i++)
            {
                debugMessage += $"Day {i + 1} : {weekDates[i]}\n";
            }
    
            Debug.Log(debugMessage);
    
            return weekDates;
        }


        public static DateTime[] GetPastWeekStartingFromMonday(int weeksAgo)
        {
            DateTime[] weekDates = new DateTime[7];
            DateTime today = DateTime.Now;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            DateTime monday = today.AddDays(daysUntilMonday).Date;
            for (int i = 0; i < 7; i++)
            {
                weekDates[i] = monday.AddDays(i - 7 * weeksAgo);
            }
            return weekDates;
        }

        public static DateTime[] GetRecordedDatesForTheLastDays(int days)
        {
            DateTime[] recordedDates = new DateTime[days];
            for (int i = 0; i < days; i++)
            {
                recordedDates[i] = TimeToReset.GetDateTimeFromNow().AddDays(-i);
            }
            return recordedDates;
        }
    }


    [Serializable]
    public struct TimeSpanUntil
    {
        public int Years;
        public int Months;
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;
    }

}