using System;
using Features.Leaderboard.Recyclable_Scroll_Rect;
using Features.Utility;
using UnityEngine;

namespace Features.Experimental.Scripts.Leaderboard
{
    public static class DailyRaceEvents
    {
        public static Action<PointsEarnedData> PointEarnedEvent;
        public static Action<bool> GrantCollected;
        public static Action<bool, bool> ShowEvent;
        public static Action<bool> LeaderboardSetInteractableEvent;
        public static Action JoinEvent;

        public static void SendPointsEarnedEvent(PointsEarnedData obj)
        {
            if (!LeaderboardTimerManager.PlayerJoinedTodaysRace()) return;
            PointEarnedEvent?.Invoke(obj);
            LeaderboardAccess.PointsToEarn += obj.PointCount;

            Debug.Log("Earned Points : " + obj.PointCount);
        }

        public static void SendGrantEvent(bool isActive)
        {
            GrantCollected?.Invoke(isActive);
        }

        public static void SendLeaderboardJoinEvent()
        {
            JoinEvent?.Invoke();
        }

        public static void SendLeaderboardShowEvent(bool isActive, bool isInteractable = true)
        {
            ShowEvent?.Invoke(isActive, isInteractable);
        }
    }

    public struct PointsEarnedData
    {
        public PointsEarnedData(int pointCount) : this(Vector2.zero, pointCount)
        {
        }
        public PointsEarnedData(Vector2 startPosition, int pointCount)
        {
            StartPosition = startPosition;
            PointCount = pointCount;
        }

        public Vector2 StartPosition;
        public int PointCount;
    }
}