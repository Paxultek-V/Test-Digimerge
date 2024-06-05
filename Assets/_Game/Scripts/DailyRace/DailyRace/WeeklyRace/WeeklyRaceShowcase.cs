using System;
using Features.Experimental.Scripts.Leaderboard;
using Features.Leaderboard.Recyclable_Scroll_Rect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Features.Leaderboard.WeeklyRace
{
    
    public enum WeeklyRaceStatus
    {
        NotJoined,
        Joined,
        NotComeYet,
    }
    
    public class WeeklyRaceShowcase : MonoBehaviour
    {
        [field: SerializeField]
        public int LastDays { get; private set; } = 14;

        [field: SerializeField]
        public Toggle[] DayToggles { get; private set; }

        [field: SerializeField]
        public Sprite FinishedMark { get; private set; }

        [field: SerializeField]
        public Sprite CrossedMark { get; private set; }

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            
            DailyRaceEvents.GrantCollected += Refresh;
            DailyRaceEvents.JoinEvent += Refresh;
            DailyRaceEvents.ShowEvent += Refresh;
            Refresh();
        }
        
        private void OnDisable()
        {
            DailyRaceEvents.GrantCollected -= Refresh;
            DailyRaceEvents.JoinEvent -= Refresh;
            DailyRaceEvents.ShowEvent -= Refresh;
        }
        
        private void Refresh(bool obj)
        {
            Refresh();
        }
        private void Refresh(bool arg1, bool arg2)
        {
            Refresh();
        }
        
        public void Refresh()
        {
            // LeaderboardTimerManager.SetThisWeekDaysAsJoined(WeeklyRaceStatus.Joined, WeeklyRaceStatus.Joined, WeeklyRaceStatus.NotJoined, WeeklyRaceStatus.NotJoined, WeeklyRaceStatus.NotJoined,WeeklyRaceStatus.NotJoined,WeeklyRaceStatus.NotJoined);

            string debugMessage = "Last Played Days \n";

            WeeklyRaceStatus[] lastPlayedDays = LeaderboardTimerManager.GetThisWeekJoinedRaceStatus();

            for (int i = 0; i < lastPlayedDays.Length; i++)
            {
                debugMessage += $"Day {i + 1} : {lastPlayedDays[i]}\n";
            }

            Debug.Log(debugMessage);
            
            string[] dayNames = new string[DayToggles.Length];
            
            
            for (int i = 0; i < DayToggles.Length; i++)
            {
                //Day name will start from monday, and it will be shortened to 3 letters
                string dayName =  ((DayOfWeek)((i + 1) % 7)).ToString().Substring(0, 3);

                DayToggles[i].SetText(dayName);

                switch (lastPlayedDays[i])
                {
                    case WeeklyRaceStatus.NotJoined:
                        DayToggles[i].SetToggle(true);
                        DayToggles[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = CrossedMark;
                        break;
                    case WeeklyRaceStatus.Joined:
                        DayToggles[i].SetToggle(true);
                        DayToggles[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = FinishedMark;
                        break;
                    case WeeklyRaceStatus.NotComeYet:
                        DayToggles[i].SetToggle(false);
                        break;
                }
            }
        }
    }
    


    public static class ToggleExtensions
    {
        public static Toggle SetText(this Toggle toggle, string text)
        {
            if (toggle.GetComponentInChildren<TMP_Text>())
            {
                toggle.GetComponentInChildren<TMP_Text>().text = text;
            }
            else
            {
                toggle.GetComponentInChildren<Text>().text = text;
            }
            return toggle;
        }
        
        public static Toggle SetText(this Toggle toggle, string text, Color color)
        {
            bool isTmpro = toggle.GetComponentInChildren<TMP_Text>();
            
            if (isTmpro)
            {
                toggle.GetComponentInChildren<TMP_Text>().text = text;
                toggle.GetComponentInChildren<TMP_Text>().color = color;
            }
            else
            {
                toggle.GetComponentInChildren<Text>().text = text;
                toggle.GetComponentInChildren<Text>().color = color;
            }

            return toggle;
        }
        
        public static Toggle SetToggle(this Toggle toggle, bool isOn)
        {
            toggle.isOn = isOn;
            return toggle;
        }
    }
}