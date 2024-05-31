using System;
using Features.Leaderboard;
using UnityEngine;
namespace Features.Utility
{
    public static class LeaderboardAccess
    {
        public static int PlayerLeaderboardScore
        {
            get
            {
                int currentPoints = PlayerPrefs.GetInt("LeaderboardTotalScore", 0);
                return currentPoints;
            }
            set
            {
                PlayerPrefs.SetInt("LeaderboardTotalScore", value);
            }
        }
        
        public static int PlayerLeaderboardRank
        {
            get
            {
                int currentRank = PlayerPrefs.GetInt("PlayerLeaderboardRank", 0);
                return currentRank;
            }
            set
            {
                PlayerPrefs.SetInt("PlayerLeaderboardRank", value);
            }
        }
        
        public static int PlayerLeaderboardLastRank
        {
            get
            {
                int currentRank = PlayerPrefs.GetInt("PlayerLeaderboardLastRank", Int32.MaxValue);
                return currentRank;
            }
            set
            {
                PlayerPrefs.SetInt("PlayerLeaderboardLastRank", value);
            }
        }
        
        
        public static string PlayerLeaderboardName
        {
            get
            {
                string currentName = PlayerPrefs.GetString("PlayerUsername", "You");
                if (string.IsNullOrEmpty(currentName))
                {
                    currentName = "You";
                }
                return currentName;
            }
            set
            {
                PlayerPrefs.SetString("PlayerUsername", value);
            }
        }

        private static LeaderboardLogicManager_Recyclable _leaderboardLogicManagerRecyclable;
        public static LeaderboardLogicManager_Recyclable LeaderboardLogicManagerRecyclable
        {
            get
            {
                if (_leaderboardLogicManagerRecyclable == null)
                {
                    _leaderboardLogicManagerRecyclable = GameObject.FindObjectOfType<LeaderboardLogicManager_Recyclable>();
                }
                return _leaderboardLogicManagerRecyclable;
            }
        }
        
        public static int PointsToEarn;
    }
}