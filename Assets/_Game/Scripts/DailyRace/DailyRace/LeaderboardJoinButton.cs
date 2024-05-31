using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Features.Experimental.Scripts.Leaderboard;
using Features.Leaderboard;
using Features.Leaderboard.Recyclable_Scroll_Rect;
using Features.Leaderboard.WeeklyRace;
using Features.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeaderboardJoinButton : Button
{

    [field: SerializeField]
    public CanvasGroup Panel { get; private set; }
    
    protected override void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        DebugData();
        //transform.DOScale(1f, 0.25f).From(0f).SetEase(Ease.OutBack,0.25f).SetDelay(1f);
        Panel.gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        Panel.alpha = 1;
        base.OnEnable(); 
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        LeaderboardTimerManager.SetTodayAs(WeeklyRaceStatus.Joined);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOScale(0f, 0.12f).From(1f).SetEase(Ease.InBack,0.35f));
        seq.Join(Panel.DOFade(0f,0.12f).From(1f));
        seq.AppendCallback(() =>
        {
            
            DailyRaceEvents.SendLeaderboardJoinEvent();
            LeaderboardTimerManager.SetPlayerLastRecordedDate(DateTime.Now);
            Panel.gameObject.SetActive(false);
            gameObject.SetActive(false);
            DailyRaceActions.PlayPlayerSelectedAnimation?.Invoke(0);
            base.OnSelect(eventData);
        });
        
    }

    public void DebugData()
    {
        Debug.Log("Daily Login Check // Player Joined Today's race : " + LeaderboardTimerManager.PlayerJoinedTodaysRace());
    }
}
