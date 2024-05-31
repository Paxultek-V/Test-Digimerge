using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Experimental.Scripts.Leaderboard.Leagues
{
    public class LeagueIcon: MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        private Sequence _scaleSequence;
        
        private LeagueSettings Settings => LeaderboardSettings.Instance().LeagueSettings;

        public void SetIcon(bool isActive)
        {
            icon.sprite = isActive ? Settings.ActiveCrown : Settings.CompletedCrown;
        }

        public void SetScale(float scaleFactor = 1)
        {
            _scaleSequence?.Kill();
            _scaleSequence = ScaleTween(scaleFactor);
        }
        
        private Sequence ScaleTween(float scaleFactor)
        {
            var rectTransform = GetComponent<RectTransform>();
            var duration = Settings.LeagueChangeTotalDuration;
            var ease = Settings.AnimationEase;
            _scaleSequence = DOTween.Sequence();
            _scaleSequence.Append(rectTransform.DOSizeDelta(icon.rectTransform.sizeDelta * scaleFactor, duration).SetEase(ease));
            _scaleSequence.Join(icon.rectTransform.DOScale(Vector3.one * scaleFactor, duration).SetEase(ease));
            return _scaleSequence;
        }
    }
}