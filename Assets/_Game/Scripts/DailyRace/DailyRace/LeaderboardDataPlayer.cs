using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Experimental.Scripts.Leaderboard
{
    [Serializable]
    public class LeaderboardDataPlayer
    {
        [field: SerializeField]
        public string Name { get; set; }

        [field: SerializeField]
        public int Points { get; set; }

        [field: SerializeField]
        public bool IsMainPlayer { get; set; }
        
        public void SetRandomPoints(Vector2Int range)
        {
            Points = Random.Range(range.x, range.y);
        }
        
        public void SetPoints(int points)
        {
            Points = points;
        }
        
        public void SetRandomName()
        {
            Name = RandomNameGenerator.GenerateRandomName();
        }

    }
}