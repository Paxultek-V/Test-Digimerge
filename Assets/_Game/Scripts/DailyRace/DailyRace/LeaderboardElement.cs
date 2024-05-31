using DG.Tweening;
using Features.Leaderboard;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Experimental.Scripts.Leaderboard
{
    public class LeaderboardElement : MonoBehaviour, ICell
    {
        public static Color BaseColor = Color.white;
        public static Color PlayerColor = Color.yellow;

        [SerializeField]
        private Image rankIcon;

        [SerializeField]
        private TMP_Text rankIndex;

        [SerializeField]
        private TMP_Text PointsText;

        [field: SerializeField]
        public Image BaseImage { get; private set; }

        public int Points { get; private set; }

        [field: SerializeField]
        public bool isPlayer { get; private set; }



        [SerializeField]
        private TMP_Text playerName;

        [SerializeField]
        private RectTransform iconHolder;

        [field: SerializeField]
        public RectTransform RectTransform { get; private set; }

        [field: SerializeField]
        public TMP_InputField PlayerNameInput { get; private set; }



        private int _index;
        private int _targetIndex;

        private bool _isInit;

        private int IndexInt
        {
            get => _index;
            set
            {
                _index = value;
                rankIndex.text = $"#{(_index + 1).ToString()}";
                if (_index < 3)
                {
                    rankIcon.sprite = LeaderboardSettings.LeaderboardRanks.GetRankSprite(_index);
                }
                if (rankIcon.enabled != _index < 3)
                    rankIcon.enabled = _index < 3;
            }
        }

        private Sequence _moveSequence;

        #region LeaderboardSettings

        private LeaderboardSettings _leaderboardSettings;
        private LeaderboardSettings LeaderboardSettings => _leaderboardSettings ??= LeaderboardSettings.Instance();

        #endregion

        private void OnEnable()
        {
            DailyRaceActions.PlayPlayerSelectedAnimation += PlayAnimationByPoint;
        }

        private void OnDisable()
        {
            DailyRaceActions.PlayPlayerSelectedAnimation -= PlayAnimationByPoint;
        }

        public void SetData(LeaderboardDataPlayer leaderboardDataPlayer)
        {
            Points = leaderboardDataPlayer.Points;
            PointsText.text = Points.ToString();
            playerName.text = leaderboardDataPlayer.Name;
            SetColor(leaderboardDataPlayer.IsMainPlayer);
            PlayerNameInput.gameObject.SetActive((leaderboardDataPlayer.IsMainPlayer));
            playerName.enabled = (!leaderboardDataPlayer.IsMainPlayer);
            isPlayer = leaderboardDataPlayer.IsMainPlayer;
        }

        public void SetColor(bool isPlayer)
        {
            var color = isPlayer ? PlayerColor : BaseColor;
            playerName.color = color;
            PlayerNameInput.textComponent.color = color;
        }

        public void SetDataAndIndex(LeaderboardDataPlayer leaderboardDataPlayer, int index)
        {
            IndexInt = index;
            SetData(leaderboardDataPlayer);
        }

        private Sequence seq;

        private void PlayAnimationByPoint(int point)
        {
            if (!isPlayer) return;

            seq?.Kill();

            seq = DOTween.Sequence();
            // seq.Append(PointsText.transform.DOScale(1.2f, 0.35f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
            seq.Append(PlayerNameInput.transform.DOScale(1.2f, 0.25f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));

            seq.Play();
        }

    }
}