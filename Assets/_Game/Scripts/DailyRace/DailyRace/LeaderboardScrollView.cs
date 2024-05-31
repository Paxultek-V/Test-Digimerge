using System;
using UnityEngine;

namespace Features.Experimental.Scripts.Leaderboard
{
    [Serializable]
    public class LeaderboardScrollView
    {
        [field: SerializeField]
        public LeaderboardElement LeaderboardElementPrefab { get; private set; }
        
        [field: SerializeField]
        public RectTransform Content { get; private set; }

        [SerializeField]
        private RectTransform holder;

        [SerializeField]
        private Vector2Int contentBounds;

        [SerializeField]
        private float contentOffset = 300;
        
        public void UpdateContentPosition(RectTransform mainPlayerRectTransform)
        {
            var mainPlayerPosition = mainPlayerRectTransform.anchoredPosition.y;
            var targetPositionY = -mainPlayerPosition - contentOffset;
            targetPositionY = Mathf.Clamp(targetPositionY, contentBounds.x, contentBounds.y);
            if (Math.Abs(targetPositionY - Content.anchoredPosition.y) < 0.01f) return;
            Content.anchoredPosition = new Vector2(0, targetPositionY);
        }
        
        public LeaderboardElement GetPrefab()
        {
            return UnityEngine.Object.Instantiate(LeaderboardElementPrefab, Content);
        }
        
#if UNITY_EDITOR
        [ContextMenu(" Set Content Bounds")]
        private void SetContentBounds()
        {
            contentBounds = new Vector2Int(-(int)(LeaderboardElementPrefab.RectTransform.rect.height / 2f), Mathf.RoundToInt(GetMaxYPosition(100)));
        }
        private float GetMaxYPosition(int itemCount)
        {
            return ((itemCount - 1) * LeaderboardElementPrefab.RectTransform.rect.height) + holder.rect.height + LeaderboardSettings.Instance().SpacingY;
        }
#endif
    }
}