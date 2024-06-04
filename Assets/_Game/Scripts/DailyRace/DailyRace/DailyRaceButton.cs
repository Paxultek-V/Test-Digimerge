
using System;
using DG.Tweening;
using Features.Experimental.Scripts.Leaderboard;
using Features.Leaderboard.Recyclable_Scroll_Rect;
using Features.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DailyRaceButton : Button
{
    private Sequence jumpSequence;
    private Sequence moveSequence;
    private Sequence shineSequence;

    [field: SerializeField]
    public TextMeshProUGUI TimeText { get; private set; }

    [field: SerializeField]
    public Image CheckImage { get; private set; }

    [field: SerializeField]
    public Image ShineImage { get; private set; }

   
    
    private CanvasGroup canvasGroup;
    
    
    protected override void Awake()
    {
        base.Awake();
        
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.SetVisibility(false);
    }


    public void Initialize()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        canvasGroup.SetVisibility(true);
        CheckImage.enabled = false;

        var rect = gameObject.GetComponent<RectTransform>();
        var startPos = rect.anchoredPosition;
        moveSequence = DOTween.Sequence();
        moveSequence.AppendInterval(2f);
        moveSequence.Append(rect.DOAnchorPos(startPos, 0.75f).From(startPos + Vector2.right * (rect.rect.width + 50)).SetEase(Ease.OutBack, 1f));
        moveSequence.AppendInterval(3f);
        moveSequence.AppendCallback(() => JumpSequence(rect));

        var shineRect = ShineImage.GetComponent<RectTransform>();
        shineSequence = DOTween.Sequence();
        shineSequence.Append(shineRect.DOAnchorPosX(250f, 0.6f).From(new Vector2(-250f, rect.anchoredPosition.y)).SetEase(Ease.OutSine));
        shineSequence.AppendInterval(0.5f);
        shineSequence.Append(shineRect.DOAnchorPosX(250f, 0.6f).From(new Vector2(-250f, rect.anchoredPosition.y)).SetEase(Ease.OutSine));
        shineSequence.AppendInterval(2f);
        shineSequence.SetLoops(-1);
    }


    private const int UpdateWaitFrameCount = 8;
    private int currentUpdate = UpdateWaitFrameCount;
    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (UpdateWaitFrameCount - currentUpdate > 0)
        {
            currentUpdate++;
            return;
        }

        currentUpdate = 0;

        if (!LeaderboardTimerManager.PlayerJoinedTodaysRace())
        {
            TimeSpan ts = LeaderboardTimerManager.GetTimeUntil(LeaderboardTimerManager.TimeToReset);

            TimeText.text = ts.GetTimeFormatted();
        }
        else
        {
            CheckImage.enabled = true;
            TimeText.text = "#" + LeaderboardAccess.PlayerLeaderboardRank;
        }

    }

    private void JumpSequence(RectTransform rect)
    {
        if (!LeaderboardTimerManager.PlayerJoinedTodaysRace())
        {
            var startPosition = rect.anchoredPosition;
            jumpSequence = DOTween.Sequence();
            jumpSequence.Append(rect.DOAnchorPos(startPosition + Vector2.left * 18f, 0.24f));
            jumpSequence.Append(rect.DOAnchorPos(startPosition, 0.25f));
            jumpSequence.Append(rect.DOAnchorPos(startPosition + Vector2.left * 18f, 0.24f));
            jumpSequence.Append(rect.DOAnchorPos(startPosition, 0.25f));
            jumpSequence.AppendInterval(4f);
            jumpSequence.SetLoops(-1);

            DailyRaceEvents.JoinEvent += () => { jumpSequence?.Kill(true); };
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        DailyRaceEvents.SendLeaderboardShowEvent(true);
        base.OnSelect(eventData);
    }
}