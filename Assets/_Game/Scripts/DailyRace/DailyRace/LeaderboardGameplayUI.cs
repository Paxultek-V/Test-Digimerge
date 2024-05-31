using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Features.Experimental.Scripts.Leaderboard
{
    public class LeaderboardGameplayUI : MonoBehaviour
    {
        
        [field: SerializeField]
        public RectTransform StarObject { get; private set; }

        [field: SerializeField]
        public StarTweenData TweenData { get; private set; }

        
        private Queue<RectTransform> _starQueue = new Queue<RectTransform>();
        
        private void OnEnable()
        {
            DailyRaceEvents.PointEarnedEvent += OnPointsEarned;
        }
        
        private void OnDisable()
        {
            DailyRaceEvents.PointEarnedEvent -= OnPointsEarned;
        }

        Tween MarioStar(RectTransform star)
        {
            return star.DOJumpAnchorPos(star.anchoredPosition + Vector2.up * 90f,0.03f * Screen.height,1,TweenData.Duration);
        }

        private void OnPointsEarned(PointsEarnedData data)
        {
            //var UIPosition = Camera.main.WorldToScreenPoint(data.StartPosition);
            var UIPosition = new Vector2(data.StartPosition.x,data.StartPosition.y % Screen.height);
            SendStars(UIPosition, data.PointCount);
        }

        void SendStars(Vector2 startPosition, int amount)
        {
            var newStar = GetStar();
            Debug.Log("Spawned Star Name : " + newStar.gameObject.name);
            startPosition.y = 2.2f;
            newStar.position = startPosition;
            var sequence = DOTween.Sequence();
            newStar.sizeDelta = TweenData.GetScale(amount);
            newStar.localScale = Vector3.one * 0.25f; 
            sequence.Append(MarioStar(newStar));
            sequence.Join(ScaleStar(newStar, 1f, 0.2f).SetEase(Ease.OutBack,0.4f));
            sequence.Append(ScaleStar(newStar,0f,0.25f).SetEase(Ease.InBack,0.4f));
            sequence.AppendCallback(() => _starQueue.Enqueue(newStar));
        }

        private RectTransform GetStar()
        {
            return _starQueue.Count > 0 ? _starQueue.Dequeue() : Instantiate(StarObject, transform);
        }

        private Tween ScaleStar(RectTransform newStar,float scale,float duration)
        {
            return newStar.DOScale(Vector3.one * scale,duration);
        }
    }

    [Serializable]
    public class StarTweenData
    {
        public float Duration = 1f;
        public AnimationCurve PointCountToStarScale;

        public Vector2 GetScale(int pointCount)
        {
            return PointCountToStarScale.Evaluate(pointCount) * Vector2.one;
        }
    }
    
}