using System;
using System.Collections;
using Features.Experimental.Scripts.Leaderboard;
using Features.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Features.Leaderboard.RewardedVideo
{
    public class DailyRaceRewardedVideoPanel : MonoBehaviour
    {
        // [field: SerializeField]
        // public TMP_Text CoinText { get; private set; }
        //
        // [field: SerializeField]
        // public Button ContinueButton { get; private set; }

        [field: SerializeField]
        public Button ShowRewardedAddButton { get; private set; }

        [field: SerializeField]
        public Button SkipButton { get; private set; }

        [field: SerializeField]
        public Image TimerFill { get; private set; }

        [field: SerializeField]
        public LeaderboardLogicManager_Recyclable LeaderboardLogicManager { get; private set; }

        [field: SerializeField]
        public CanvasGroup cg { get; private set; }

        private Coroutine autoSkipCoroutine;

        [field: SerializeField]
        public TMP_Text TapToContinueButton { get; private set; }

        [field: SerializeField]
        public Image CloseImage { get; private set; }

        
        private int points, previousPoints;
        
        


        private void OnEnable()
        {
            cg = GetComponent<CanvasGroup>();
            // ContinueButton.onClick.AddListener(ContinueNormal);
            SkipButton.onClick.AddListener(SkipNormal);
            ShowRewardedAddButton.onClick.AddListener(ContinueWithRewardedVideo);
            //GameActions.SetRewardedVideoPanelVisible += SetPanelVisible;
        }

        private void OnDisable()
        {
            // ContinueButton.onClick.RemoveListener(ContinueNormal);
            SkipButton.onClick.RemoveListener(SkipNormal);
            ShowRewardedAddButton.onClick.RemoveListener(ContinueWithRewardedVideo);
            //GameActions.SetRewardedVideoPanelVisible -= SetPanelVisible;
        }
        
        private void SkipNormal()
        {
            StartCoroutine(IE_ContinueNormal());
        }
        
        private void ContinueWithRewardedVideo()
        {
            DailyRaceEvents.LeaderboardSetInteractableEvent?.Invoke(false);
            //VoodooSauce.ShowRewardedVideo(OnRewardedVideoShown, "DailyRaceRewardedVideo");
        }
        
        private void OnRewardedVideoShown(bool Success)
        {

            if (Success)
            {
                Debug.Log($"Daily Race Rewarded // Rewarded Video Success Previous Points = {LeaderboardAccess.PlayerLeaderboardScore}");
                LeaderboardAccess.PlayerLeaderboardScore += points;
                Debug.Log($"Daily Race Rewarded // Added more points! Current Points = {LeaderboardAccess.PlayerLeaderboardScore}");
                StartCoroutine(IE_ContinueWithRewardedVideo(points, previousPoints));

            }else
            {
                StartCoroutine(IE_ContinueNormal(false));
            }
        }


        private IEnumerator IE_ContinueNormal(bool addPoints = false)
        {
            if (autoSkipCoroutine != null)
            {
                StopCoroutine(autoSkipCoroutine);
            }

            
            //GameActions.SetRewardedVideoPanelVisible?.Invoke(false);
            
            if (addPoints)
            {
                LeaderboardLogicManager.MoveToMainPlayer();
                yield return new WaitForSeconds(2.5f);
            }else
            {

            }
            
            //GameActions.OnLevelCompletedEnd?.Invoke();
            DailyRaceEvents.SendLeaderboardShowEvent(false);
        }
        
        private IEnumerator IE_ContinueWithRewardedVideo(int points, int previousPoints)
        {
            if (autoSkipCoroutine != null)
            {
                StopCoroutine(autoSkipCoroutine);
            }
            
            //GameActions.SetRewardedVideoPanelVisible?.Invoke(false);
            LeaderboardLogicManager.MoveToMainPlayer();
            yield return new WaitForSeconds(2.5f);
            //GameActions.OnLevelCompletedEnd?.Invoke();
            DailyRaceEvents.SendLeaderboardShowEvent(false);
        }
        
        private void SetPanelVisible(bool visible)
        {
            // CoinText.text = $"Earned : {LeaderboardAccess.PointsToEarn}";
            StartCoroutine(IE_SetPanelVisible(visible));
        }

        private IEnumerator IE_SetPanelVisible(bool visible)
        {
            Debug.Log("Daily Race Rewarded // Set Panel Visible!");
            


            SkipButton.interactable = false;
            CloseImage.enabled = false;
            TapToContinueButton.text = "";
            if (visible)
            {
                points = LeaderboardAccess.PointsToEarn;
                previousPoints = points;

                LeaderboardAccess.PointsToEarn = 0;
                LeaderboardAccess.PlayerLeaderboardScore += points; 
                LeaderboardLogicManager.MoveToMainPlayer();
                yield return new WaitForSeconds(2f);
            }
            SkipButton.interactable = true;
            CloseImage.enabled = true;
            TapToContinueButton.text = "Tap to Continue!";
            
            cg.SetVisibility(visible);
            cg.ignoreParentGroups = visible;
            if (visible)
            {
                if (autoSkipCoroutine != null)
                {
                    StopCoroutine(autoSkipCoroutine);
                }
                autoSkipCoroutine = StartCoroutine(IE_SkipAutomaticallyAfterSeconds(5f));
            }
        }
        
        private IEnumerator IE_SkipAutomaticallyAfterSeconds(float seconds)
        {
            // yield return new WaitForSeconds(seconds);
            // bool setTheContinueText = false;
            float elapsedTime = 0;
            while (elapsedTime < seconds)
            {
                elapsedTime += Time.deltaTime;
                TimerFill.fillAmount = Mathf.Lerp(0, 1, elapsedTime / seconds);
                //If elapsed time is more or equal to the half seconds, set the continue text
                
                // if (elapsedTime >= (seconds / 2) && !setTheContinueText)
                // {
                //     setTheContinueText = true;
                //     TapToContinueButton.text = "Tap to continue";
                // }
                
                yield return null;
            }
            TimerFill.fillAmount = 1;
            
            SkipNormal();
            
        }

        // private IEnumerator IE_GainPoint(int points, int previousPoints)
        // {
        //     float duration = .5f;
        //     float elapsedTime = 0;
        //     
        //     CoinText.text = $"Earned : {previousPoints}";
        //     
        //     while (elapsedTime < duration)
        //     {
        //         elapsedTime += Time.deltaTime;
        //         CoinText.text = $"Earned : {Mathf.RoundToInt(Mathf.Lerp(0, points, elapsedTime / duration))}";
        //         yield return null;
        //     }
        //     
        //     CoinText.text = $"Earned : {points}";
        //     
        //     
        // }
    }


}