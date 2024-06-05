using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Features.Experimental.Scripts.Leaderboard;
using Features.Leaderboard.Recyclable_Scroll_Rect;
using Features.Utility;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Leaderboard
{
    public class LeaderboardLogicManager_Recyclable : MonoBehaviour, IRecyclableScrollRectDataSource
    {

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private RecyclableScrollRect recyclableScrollRect;

        [SerializeField]
        private DailyRaceData_League league;

        [SerializeField]
        private LeaderboardDataPlayer mainPlayerData;

        private List<LeaderboardDataPlayer> _sortedLeaderboardDataPlayers;
        private int _mainPlayerIndex;

        [field: SerializeField]
        public TMP_Text TodayRaceText { get; private set; }

        [field: SerializeField]
        public TMP_Text TimeUntilResetText { get; private set; }

        [field: SerializeField]
        public GameObject JoinBlocker { get; private set; }

        [field: SerializeField]
        public LeaderboardJoinButton JoinButton { get; protected set; }
        
        private Sequence canvasSequence;
        
        [field: SerializeField]
        public TextMeshProUGUI JoinedPlayersCountText { get; private set; }

        [field: SerializeField]
        public CanvasGroup LastPlacementCanvas { get; private set; }

        [field: SerializeField]
        public TextMeshProUGUI DateText { get; private set; }

        [field: SerializeField]
        public TextMeshProUGUI LastPlacementText { get; private set; }
        
        [field: SerializeField]
        public RectTransform LastPlacementShine { get; private set; }
        
        [field: SerializeField]
        public int PlayerIndexOffsetScroll { get; private set; }

        [field: SerializeField]
        public Button CloseButton { get; private set; }
        

        [field: SerializeField]
        public Image CloseButtonImg { get; private set; }

        
        
        private void Awake()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            Setup();
            recyclableScrollRect.DataSource = this;
            
            Controller_Level.OnFinishedLevel += Controller_Level_OnFinishedLevel;
            PiggyBank.OnSendTotalMoneyCollected += PiggyBank_OnPiggyBankFinishedCollectingMoney;
            PiggyBank.OnTargetAmountNotReached += PiggyBank_OnTargetAmountNotReached;
            Manager_GameState.OnBroadcastGameState += OnGameStateChanged;
            
            TodayRaceText.text = $"Daily Race";
            DailyRaceEvents.ShowEvent += Show;
            DailyRaceEvents.JoinEvent += Join;
            DailyRaceEvents.GrantCollected += GrantCollected;
            DailyRaceEvents.LeaderboardSetInteractableEvent += SetInteractable;
            
            CloseButton.onClick.AddListener(() => DailyRaceEvents.SendLeaderboardShowEvent(false, false));
        }

        private void PiggyBank_OnTargetAmountNotReached()
        {
            LeaderboardAccess.PointsToEarn = 0;
        }
        
        private void Controller_Level_OnFinishedLevel()
        {
            DailyRaceEvents.SendLeaderboardShowEvent(true, false);
        }

        private void PiggyBank_OnPiggyBankFinishedCollectingMoney(float money)
        {
            DailyRaceEvents.SendPointsEarnedEvent(new PointsEarnedData((int)money));
            
        }
        
        private void OnGameStateChanged(GameState obj)
        {
            switch (obj)
            {
                default:
                    Show(false);
                    break;
            }
        }
        
        private void SetInteractable(bool isInteractable)
        {
            canvasGroup.interactable = isInteractable;
            canvasGroup.blocksRaycasts = isInteractable;
        }

        private void OnDestroy()
        {
            DailyRaceEvents.ShowEvent -= Show;
            DailyRaceEvents.GrantCollected -= GrantCollected;
            DailyRaceEvents.JoinEvent -= Join;
            DailyRaceEvents.LeaderboardSetInteractableEvent -= SetInteractable;
            Controller_Level.OnFinishedLevel -= Controller_Level_OnFinishedLevel;
            PiggyBank.OnSendTotalMoneyCollected -= PiggyBank_OnPiggyBankFinishedCollectingMoney;
            PiggyBank.OnTargetAmountNotReached -= PiggyBank_OnTargetAmountNotReached;
        }
        
        public void Show(bool isShow, bool interactable = true)
        {
            canvasGroup.alpha = isShow ? 1 : 0;
            canvasGroup.interactable = isShow;
            canvasGroup.blocksRaycasts = isShow;
            canvasSequence?.Kill(true);
            //GameActions.SetRewardedVideoPanelVisible?.Invoke(false);
            if (isShow)
            {
                var rect = canvasGroup.GetComponent<RectTransform>();
                var startPos = rect.anchoredPosition;
                MoveList();
                canvasSequence = DOTween.Sequence();
                canvasSequence.Append(canvasGroup.transform.DOScale(Vector3.one * 1.04f, 0.25f).From(Vector3.one * 0.25f));
                canvasSequence.Join(canvasGroup.DOFade(1f, 0.33f).From(0f));
                canvasSequence.Join(rect.DOAnchorPos(startPos, 0.2f).From(startPos + Vector2.down * 500f).SetEase(Ease.OutSine));
                canvasSequence.Append(canvasGroup.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.InSine));

                JoinedPlayersCountText.text = $"{(_sortedLeaderboardDataPlayers.Count-1)} other players in race!";
                if (interactable == false)
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                    // JoinedPlayersCountText.text = "Tap To Continue";
                    //CloseButtonImg.enabled = false;
                }
                
                // canvasGroup.interactable = isInteractable;
                // canvasGroup.blocksRaycasts = isInteractable;
            }
            else
            {
                var startAlpha = canvasGroup.alpha > 0f ? 1f : 0f;
                canvasSequence.Append(canvasGroup.DOFade(0f, 0.25f).From(startAlpha));
                recyclableScrollRect.velocity = Vector2.zero;
            }

            DateText.text = LeaderboardTimerManager.GetCurrentDateText();
            

            
            Debug.Log("Leaderboard Show" + isShow);
        }

        public void MoveList()
        {
            if (LeaderboardTimerManager.PlayerJoinedTodaysRace())
            {
                SnapToMainPlayer();
            }
            else
            {
                MoveToMainPlayer();
            }
        }

        [ContextMenu("Update Time Strings")]
        public void UpdateTimeStrings(){
            DateText.text = LeaderboardTimerManager.GetCurrentDateText();
            TimeSpan ts = LeaderboardTimerManager.GetTimeUntil(LeaderboardTimerManager.TimeToReset);
            TimeUntilResetText.text = ts.GetTimeRemainingText();
        }

        private void GrantCollected(bool isActive)
        {
            LeaderboardAccess.PlayerLeaderboardScore += LeaderboardAccess.PointsToEarn;
            LeaderboardAccess.PointsToEarn = 0;
            MoveToMainPlayer();
        }

        private void Join()
        {
            LeaderboardAccess.PlayerLeaderboardScore = 0;
            LeaderboardAccess.PointsToEarn = 0;
            //oveToMainPlayer();
            LeaderboardAccess.PlayerLeaderboardLastRank = LeaderboardAccess.PlayerLeaderboardRank;
            var rect = canvasGroup.GetComponent<RectTransform>();
            rect.DOPunchScale(Vector3.one * 0.02f, 0.2f, 5);
        }

        public void ConfirmLatestRank()
        {
            var rect = LastPlacementCanvas.GetComponent<RectTransform>();
            var confirmSequence = DOTween.Sequence();
            confirmSequence.Append(rect.DOScale(Vector3.zero, 0.24f).From(Vector3.one).SetEase(Ease.InBack,0.05f).OnComplete(() =>
            {
                LastPlacementCanvas.alpha = 0;
                LastPlacementCanvas.interactable = false;
                LastPlacementCanvas.blocksRaycasts = false;
                canvasGroup.GetComponent<RectTransform>().DOPunchScale(Vector3.one * 0.02f, 0.2f, 5);
            }));
            confirmSequence.Join(LastPlacementCanvas.DOFade(0f, 0.24f).From(1f));

        }
        
        private void Setup()
        {
            var joinedToday = LeaderboardTimerManager.PlayerJoinedTodaysRace();
            var lastPlacementActive = LeaderboardAccess.PlayerLeaderboardLastRank < 5000 && !joinedToday;
            LastPlacementCanvas.alpha = lastPlacementActive ? 1 : 0;
            LastPlacementCanvas.interactable = lastPlacementActive;
            LastPlacementCanvas.blocksRaycasts = lastPlacementActive;
            if (lastPlacementActive)
            {
                ShowLastPlacement();
            }
            
            JoinButton.gameObject.SetActive(!joinedToday);
            JoinBlocker.SetActive(!joinedToday);
            SortUsers();
            JoinedPlayersCountText.text = (_sortedLeaderboardDataPlayers.Count-1).ToString()+" other players in race!";
        }

        private void ShowLastPlacement()
        {
            var rectTransform = LastPlacementCanvas.GetComponent<RectTransform>();
            rectTransform.DOScale(Vector3.one, 0.25f).From(Vector3.one * 0.34f);
            LastPlacementText.text = $"You placed \n<size=165%><b>#{LeaderboardAccess.PlayerLeaderboardRank}</b></size>\n in your last race!";
            LeaderboardAccess.PlayerLeaderboardLastRank = Int32.MaxValue;
            var shineSequence = DOTween.Sequence();
            shineSequence.Append(LastPlacementShine.DOAnchorPosX(470f, 1f).From(new Vector2(-470f, LastPlacementShine.anchoredPosition.y)).SetEase(Ease.OutSine));
            shineSequence.AppendInterval(1f);
            shineSequence.Append(LastPlacementShine.DOAnchorPosX(470f, 1f).From(new Vector2(-470f, LastPlacementShine.anchoredPosition.y)).SetEase(Ease.OutSine));
            shineSequence.AppendInterval(3f);
            shineSequence.SetLoops(-1);
            MoveToMainPlayer();
        }

        public void ShareButtonPressed()
        {
            
        }

        private void SortUsers()
        {
            _sortedLeaderboardDataPlayers = league.PlayerDataHolder.LeaderboardDataPlayers.ToList();
            SetMainPlayerData();
            _sortedLeaderboardDataPlayers.Add(mainPlayerData);
            _sortedLeaderboardDataPlayers = _sortedLeaderboardDataPlayers.OrderByDescending(x => x.Points).ToList();
            _mainPlayerIndex = _sortedLeaderboardDataPlayers.IndexOf(mainPlayerData);
            LeaderboardAccess.PlayerLeaderboardRank = _mainPlayerIndex + 1;
            LeaderboardAccess.PlayerLeaderboardLastRank = LeaderboardAccess.PlayerLeaderboardRank;
        }


        public int startIndexOffset = 10;

        [ContextMenu("MoveToPlayer")]
        public void MoveToMainPlayer()
        {

            SortUsers();
            int firstMoveIndex = 0;
            if (_mainPlayerIndex < 980)
            {
                firstMoveIndex = _mainPlayerIndex + 10;
                PlayerIndexOffsetScroll = -10;
            }
            else
            {
                firstMoveIndex = _mainPlayerIndex - 10;
                PlayerIndexOffsetScroll = 10;
            }
            recyclableScrollRect.MoveToCellWithTime(firstMoveIndex, _mainPlayerIndex + PlayerIndexOffsetScroll);
            Debug.Log("Leaderboard: MovingToMainPlayer: Main Player Index :" + _mainPlayerIndex + " FirstMoveIndex : " + firstMoveIndex);

        }

        private void SnapToMainPlayer()
        {
            SortUsers();
            recyclableScrollRect.SnapToCell(_mainPlayerIndex+1);
        }

        public int movePosition;

        [ContextMenu("MoveToPosition")]
        private void SetData()
        {
            recyclableScrollRect.MoveToCellWithTime(movePosition, movePosition);
            Debug.Log("Leaderboard: Move to Index:" + movePosition);
        }

        public int movePoints;

        [ContextMenu("MoveToPoints")]
        private void IncreaseData()
        {
            LeaderboardAccess.PlayerLeaderboardScore = movePoints;
            SortUsers();
            int firstMoveIndex = 0;
            firstMoveIndex = _mainPlayerIndex < 500
                ? _mainPlayerIndex + startIndexOffset
                : _mainPlayerIndex - startIndexOffset;
            recyclableScrollRect.MoveToCellWithTime(firstMoveIndex, _mainPlayerIndex);
            Debug.Log("Leaderboard: Main Player Points :" + movePoints + "\n" + "Main Player Index :" + _mainPlayerIndex);
        }

        private void Update()
        {
            CheckIfNewDayBegin();

#if UNITY_EDITOR
            DebugControlLeaderboard();
#endif
        }

        private void CheckIfNewDayBegin()
        {
            TimeSpan ts = LeaderboardTimerManager.GetTimeUntil(LeaderboardTimerManager.TimeToReset);
            TimeUntilResetText.text = ts.GetTimeRemainingText();
            
            if (LeaderboardTimerManager.PlayerJoinedTodaysRace() && ts.TotalSeconds < 1.5f)
            {
                LeaderboardTimerManager.ResetPlayerRecordedDate();
                DailyRaceEvents.SendLeaderboardShowEvent(false);
                DOVirtual.DelayedCall(0.5f, Setup);
            }
        }


        private void SetMainPlayerData()
        {
            mainPlayerData.Points = PlayerPoints();
            var text = LeaderboardAccess.PlayerLeaderboardName;
            mainPlayerData.Name = text;
            mainPlayerData.IsMainPlayer = true;
        }

        private int PlayerPoints()
        {
            return LeaderboardAccess.PlayerLeaderboardScore;
        }

        #region DATA-SOURCE

        public int GetItemCount()
        {
            return _sortedLeaderboardDataPlayers.Count;
        }

        public void SetCell(ICell cell, int index)
        {
            var item = cell as LeaderboardElement;
            var itemData = _sortedLeaderboardDataPlayers[index];
            if (item != null)
                item.SetDataAndIndex(itemData, index);
        }

        #endregion

#if UNITY_EDITOR

        private void DebugControlLeaderboard()
        {
            return;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                recyclableScrollRect.MoveToCell(-1);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                recyclableScrollRect.MoveToCell(1);
            }
        }

#endif
    }
}