using System;
using DG.Tweening;
using UnityEngine;

namespace Features.Experimental.Scripts.Leaderboard
{
    [CreateAssetMenu(fileName = "LeaderboardSettings", menuName = "SnB/Leaderboard/LeaderboardSettings")]
    public class LeaderboardSettings: SerializedResourcedScriptableObject<LeaderboardSettings>
    {
        [field: SerializeField]
        public float AnimationSpeed { get; private set; }

        [field: SerializeField]
        public Vector2Int MinMadAnimationDuration { get; private set; }

        [field: SerializeField]
        public int SpacingY { get; private set; } = 20;

        [field: SerializeField]
        public Ease AnimationEase { get; private set; }

        [field: SerializeField]
        public LeagueSettings LeagueSettings { get; private set; }

        [field: SerializeField]
        public LeaderboardRanks LeaderboardRanks { get; private set; }
    }

    [Serializable]
    public class LeagueSettings
    {
        [field: SerializeField]
        public int LeagueChangeTotalDuration { get; private set; }
        
        [field: SerializeField]
        public Ease AnimationEase { get; private set; }

        [field: SerializeField]
        public Sprite CompletedCrown { get; private set; }

        [field: SerializeField]
        public Sprite ActiveCrown { get; private set; }
    }
    
    [Serializable]
    public class LeaderboardRanks
    {
        [field: SerializeField]
        private Sprite gold;

        [field: SerializeField]
        private Sprite silver;
        
        [field: SerializeField]
        private Sprite bronze;
        
        public Sprite GetRankSprite(int rank)
        {
            return rank switch
            {
                0 => gold,
                1 => silver,
                2 => bronze,
                _ => null
            };
        }
    }
}