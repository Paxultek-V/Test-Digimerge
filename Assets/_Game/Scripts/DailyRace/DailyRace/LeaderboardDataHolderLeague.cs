using UnityEngine;

namespace Features.Experimental.Scripts.Leaderboard
{
    [CreateAssetMenu(fileName = "LeaderboardDataHolderLeague", menuName = "SnB/Leaderboard/LeaderboardDataHolderLeague")]
    public class LeaderboardDataHolderLeague: SerializedResourcedScriptableObject<LeaderboardDataHolderLeague>
    {
        [field: SerializeField]
        public DailyRaceData_League[] Leagues { get; private set; }

        public DailyRaceData_League GetLeague(int points)
        {
            foreach (var league in Leagues)
            {
                if (points < league.MaxPointValue)
                {
                    return league;
                }
            }
            return Leagues[^1];
        }

        public Vector2Int GetRandomPointsRange(LeagueType points)
        {
            var index = (int)points;
            int min = 0;
            int max = 0;
            if (index > 0)
            {
                var previousLeague = Leagues[Mathf.Max(0, (int)points - 1)];
                min = previousLeague.MaxPointValue + 3;
            }
            else
            {
                min = 0;
            }
            var currentLeague = Leagues[(int)points];
            max = currentLeague.MaxPointValue - 1;
            return new Vector2Int(min, max);
        }


    }
}