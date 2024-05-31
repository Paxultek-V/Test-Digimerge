
using Features.Utility;
using UnityEngine;

namespace Features.Experimental.Scripts.Leaderboard
{
    [CreateAssetMenu(fileName = "LeaderboardDataLeague", menuName = "SnB/Leaderboard/LeaderboardDataLeague")]
    public class DailyRaceData_League: ScriptableObject
    {
        [field: SerializeField]
        public int MaxPointValue { get; private set; }
        
        [field: SerializeField]
        public LeagueType LeagueType { get; private set; }
        
        [field: SerializeField]
        public LeaderboardDataHolderPlayer PlayerDataHolder { get; private set; }
        
        
#if UNITY_EDITOR

        [ContextMenu(" Create Random Leaderboard Data Players")]
        private void CreateRandomLeaderboardDataPlayers()
        {
            PlayerDataHolder.CreateRandomLeaderboardDataPlayers(LeagueType);
        }

#endif
        public void GenerateListAccordingToPlayer()
        {
            PlayerDataHolder.CreateLeaderboardWithPlayerData(LeagueType, LeaderboardAccess.PlayerLeaderboardScore, 30);
        }

    }
}