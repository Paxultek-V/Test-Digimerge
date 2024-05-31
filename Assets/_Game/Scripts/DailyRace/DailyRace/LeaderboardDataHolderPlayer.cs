using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Experimental.Scripts.Leaderboard
{
    [Serializable]
    public class LeaderboardDataHolderPlayer
    {
        [field: SerializeField]
        public Vector2Int PlayerCountRange { get; private set; }

        
        
        [field: SerializeField]
        public List<LeaderboardDataPlayer> LeaderboardDataPlayers { get; private set; }

        public List<LeaderboardDataPlayer> GetSortedLeaderboardDataPlayers()
        {
            return LeaderboardDataPlayers.OrderByDescending(player => player.Points).ToList();
        }

        private void SortTheList()
        {
            LeaderboardDataPlayers = LeaderboardDataPlayers.OrderByDescending(player => player.Points).ToList();
        }

        public void CreateRandomLeaderboardDataPlayers(LeagueType league)
        {
            var count = Random.Range(PlayerCountRange.x, PlayerCountRange.y);
            LeaderboardDataPlayers = new List<LeaderboardDataPlayer>();
            for (var i = 0; i < count; i++)
            {
                var leaderboardDataPlayer = new LeaderboardDataPlayer();
                leaderboardDataPlayer.SetRandomPoints(LeaderboardDataHolderLeague.Instance().GetRandomPointsRange(league));
                leaderboardDataPlayer.SetRandomName();
                LeaderboardDataPlayers.Add(leaderboardDataPlayer);
            }
            SortTheList();
        }

        public void CreateLeaderboardWithPlayerData(LeagueType leagueType, int playerPoints, int MaxPlayers)
        {
            LeaderboardDataPlayers = new List<LeaderboardDataPlayer>();

            // Calculate the number of players to be created above and below the playerPoints
            int halfPlayers = MaxPlayers / 2;

            // Assuming GetRandomPointsRange returns a tuple (min, max) for the leagueType
            var pointsRange = LeaderboardDataHolderLeague.Instance().GetRandomPointsRange(leagueType);
            int minPoints = pointsRange.x;
            int maxPoints = pointsRange.y;

            // Adjust min and max if necessary to ensure they don't overlap with playerPoints
            if (minPoints >= playerPoints) minPoints = playerPoints - 10; // Example adjustment
            if (maxPoints <= playerPoints) maxPoints = playerPoints + 10;

            // Create players with points below the playerPoints
            for (int i = 0; i < halfPlayers; i++)
            {
                var leaderboardDataPlayer = new LeaderboardDataPlayer();
                // Ensure points are below the playerPoints, within the league's range
                int randomPoints = Random.Range(minPoints, playerPoints - 1); // -1 to ensure below
                leaderboardDataPlayer.SetPoints(randomPoints);
                leaderboardDataPlayer.SetRandomName();
                LeaderboardDataPlayers.Add(leaderboardDataPlayer);
            }

            // Create players with points above the playerPoints
            for (int i = 0; i < halfPlayers; i++)
            {
                var leaderboardDataPlayer = new LeaderboardDataPlayer();
                // Ensure points are above the playerPoints, within the league's range
                int randomPoints = Random.Range(maxPoints, playerPoints + 1); // +1 to ensure above
                leaderboardDataPlayer.SetPoints(randomPoints);
                leaderboardDataPlayer.SetRandomName();
                LeaderboardDataPlayers.Add(leaderboardDataPlayer);
            }

            // Optionally, add a player at playerPoints if MaxPlayers is odd
            // if (MaxPlayers % 2 != 0)
            // {
            //     var playerAtPoints = new LeaderboardDataPlayer();
            //     playerAtPoints.SetPoints(playerPoints);
            //     playerAtPoints.SetRandomName();
            //     LeaderboardDataPlayers.Add(playerAtPoints);
            // }

            SortTheList();
        }


    }

    public static class RandomNameGenerator
    {
        private static List<string> firstNames = new List<string> 
        { 
            "Alex", "Jordan", "Taylor", "Morgan", "Casey", "Riley", "Jamie", "Avery", "Skyler", "Dakota" , "James" , "John" 
            , "Richard", "David", "William" , "Barbara" , "Thomas" , "Paul", "Steven", "Brian" , "George" , "Andrew" , "Kevin" 
            ,"Eric" , "Kind" , "Blank" , "Speedy" , "Flower" , "Big" , "Small" , "Lewis"
        };
        private static List<string> lastNames = new List<string>
        {
            "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor" , "Gonzales" , "Martinez" 
            , "Anderson" , "Lee" , "Perez" , "Ramirez" , "Young" , "Walker" , "Hill", "Scott", "Nguyen", "Nelson", "Baker" , "Carter"
            , "Hall" , "Green", "Sanchez", "White"
        };

        private static System.Random random = new System.Random();
        private static float NumberedPlayerChance = 0.4f;
        private static float UseSpaceChance = 0.75f;
        private static float AddRandomNumberChance = 0.3f;
        private static float ForceLowercaseChance = 0.25f;

        public static string GenerateRandomName()
        {
            string firstName = firstNames[random.Next(firstNames.Count)];
            string lastName = lastNames[random.Next(lastNames.Count)];
            if (random.NextDouble()<NumberedPlayerChance)
            {
                firstName = "Player_";
                var num = random.Next(1,999999);
                lastName = $"{num:D6}";
                return $"{firstName}{lastName}";
            }

            var useSpace = random.NextDouble() < UseSpaceChance ? " " : String.Empty;
            var randNumber = random.NextDouble() < AddRandomNumberChance ? random.Next(10,9999).ToString() : String.Empty;
            var name = $"{firstName}{useSpace}{lastName}{randNumber}";

            if (random.NextDouble() < ForceLowercaseChance)
            {
                name = name.ToLower();
            }
            
            return name;
        }
    }
}