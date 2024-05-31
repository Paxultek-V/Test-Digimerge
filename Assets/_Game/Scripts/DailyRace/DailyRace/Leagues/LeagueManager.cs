using System;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Features.Experimental.Scripts.Leaderboard.Leagues
{
    [Serializable]
    public class LeagueManager
    {
        [SerializeField]
        private LeagueIcon[] leagueIcons;
        
        [SerializeField]
        private TMP_Text leagueTitle;

        [SerializeField]
        private RectTransform holder;
        
        private Tween _changeLeagueTween;
        private StringBuilder _stringBuilder = new ();

        public void SetLeagueIcon(int index, bool isActive, Action onTweenComplete = null)
        {
            var leagueIcon = leagueIcons[index];
            SetLeagueName(index);
            SetHolderPosition(index, onTweenComplete);
            leagueIcon.SetIcon(isActive);
            leagueIcon.SetScale(isActive ? 1.5f : 1);
        }

        private void SetHolderPosition(int index, Action onTweenComplete = null)
        {
            var height = 163;
            var holderXPosition = -(index * height);
            _changeLeagueTween?.Kill();
            _changeLeagueTween = ChangeLeagueTween(holderXPosition).OnComplete(() => onTweenComplete?.Invoke());
        }
        
        private Tween ChangeLeagueTween(float holderXPosition)
        {
            var settings = LeaderboardSettings.Instance().LeagueSettings;
            return holder.DOAnchorPosX(holderXPosition, settings.LeagueChangeTotalDuration).SetEase(settings.AnimationEase, 0.3f);
        }
        
        private void SetLeagueName(int index)
        {
            var leagueName = GetLeagueName(index);
            _stringBuilder.Clear();
            _stringBuilder.Append(leagueName);
            _stringBuilder.Append(" League");
            leagueTitle.text = _stringBuilder.ToString();
        }
        
        private string GetLeagueName(int leagueType)
        {
            return Enum.GetName(typeof(LeagueType), leagueType);
        }
    }
    
}