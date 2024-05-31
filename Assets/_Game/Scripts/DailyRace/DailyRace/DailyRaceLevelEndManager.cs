using System;
using System.Collections;
using Features.Leaderboard.Recyclable_Scroll_Rect;
using UnityEngine;
namespace Features.Leaderboard
{
    public class DailyRaceLevelEndManager : MonoBehaviour
    {
        private void OnEnable()
        {
            Controller_Level.OnFinishedLevel += OnLevelCompletedBegin;
        }

        private void OnDisable()
        {
            Controller_Level.OnFinishedLevel -= OnLevelCompletedBegin;
        }

        private void OnLevelCompletedBegin()
        {
            StartCoroutine(IE_OnLevelCompleteBegin());
        }

        private IEnumerator IE_OnLevelCompleteBegin()
        {
            bool showLeaderboardAtTheEnd = LeaderboardTimerManager.ShowAtTheEnd();
            if (showLeaderboardAtTheEnd)
            {
                bool showRewardedVideo = LeaderboardTimerManager.ShowRewardedVideo();
                if (showRewardedVideo)
                {
                    yield return new WaitForSeconds(1.5f);
                    //GameActions.SetRewardedVideoPanelVisible?.Invoke(true);
                }
                else
                {
                    yield return new WaitForSeconds(5f);
                    //GameActions.OnLevelCompletedEnd?.Invoke();
                }
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                //GameActions.OnLevelCompletedEnd?.Invoke();
            }

        }
    }
}