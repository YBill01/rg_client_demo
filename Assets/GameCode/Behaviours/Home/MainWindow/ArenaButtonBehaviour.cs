using Legacy.Database;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class ArenaButtonBehaviour : MonoBehaviour, IParticleReciever
    {
        [SerializeField]
        private TextMeshProUGUI ArenaName;
        [SerializeField]
        private TextMeshProUGUI RatingText;
        [SerializeField]
        private ProgressBarChangeValueBehaviour ProgressBar;
        [SerializeField]
        private AlertBoxBehaviour alertBox;
        [SerializeField]
        private GameObject cupIcon;
        [SerializeField]
        private LegacyButton button;

        [Space]
        [SerializeField] private ProgressBarChangeValueBehaviour progressBatBehaviour;

        private ProfileInstance profile;
        private bool _ratingInited = false;

        internal void SetRating(ushort rating)
        {
            if (profile.IsBattleTutorial)
            {
                RatingText.text = LegacyHelpers.FormatByDigits(profile.HardTutorialState.ToString());
                ProgressBar.Set(profile.HardTutorialState, true, (uint)Tutorial.Instance.TotalCount());
                ArenaName.text = Locales.Get("locale:1981");
            }
            else
            {
                if (Settings.Instance.Get<ArenaSettings>().RatingBattlefield(rating, out BinaryBattlefields binaryArena))
                {
                    var data = Settings.Instance.Get<ArenaSettings>().GetArenaData(rating);
                    InitTextOfRating();
                    ProgressBar.SetOnlyBar(data.rating, binaryArena.rating);
                    ArenaName.text = Locales.Get(binaryArena.title);
                }
            }
        }

        private void Start()
        {
            profile = ClientWorld.Instance.Profile;

            if (profile.IsBattleTutorial)
            {
                button.gameObject.SetActive(false);
              /*  button.interactable = false;
                button.isLocked = true;
                button.localeAlert = Locales.Get("locale:1483");*/
            }
            else
            {
                profile.PlayerProfileUpdated.AddListener(UpdateAlertBox);
                UpdateAlertBox();
            }
        }

        private void UpdateAlertBox()
        {
            var arenaSettings = Settings.Instance.Get<ArenaSettings>();

            int countToCollect = 0;
            int offsetReting = 0;

            for (byte i = 0; i <= profile.CurrentArena.number; i++)
            {
                var arenaIndex = arenaSettings.queue[i];
                if (Battlefields.Instance.Get(arenaIndex, out BinaryBattlefields binaryArena))
                {
                    for (byte j = 0; j < binaryArena.rewards.Count; j++)
                    {
                        int RealRettingReward = offsetReting + binaryArena.rewards[j].rating;
                        if ((RealRettingReward <= profile.Rating.max) && (!profile.Rating.HasReward(arenaIndex, j)))
                        {
                            countToCollect++;
                        }
                    }
                }
                offsetReting += binaryArena.rating;
            }

            alertBox.HideAll();
            cupIcon.SetActive(true);

            if (countToCollect != 0)
            {
                alertBox.ShowGreenAlert(countToCollect.ToString(), true);
            }
        }

        private void InitTextOfRating()
        {
            var holdValue = progressBatBehaviour.GetHoldValue();
            if (holdValue > 0 && !_ratingInited)
            {
                var tempRating = profile.Rating.current - holdValue;
                RatingText.text = LegacyHelpers.FormatByDigits(tempRating.ToString());
            }
            else
            {
                RatingText.text = LegacyHelpers.FormatByDigits(profile.Rating.current.ToString());
            }
            _ratingInited = true;
        }

        public void ParticleCame(float percentageComplete)
        {
            var currnetRating   = profile.Rating.current;
            var holdValue       = progressBatBehaviour.GetHoldValue();
            var difference      = currnetRating - holdValue;

            var ratingToShow    = (int)Mathf.Lerp(difference, currnetRating, percentageComplete);
            RatingText.text     = LegacyHelpers.FormatByDigits(ratingToShow.ToString());
        }

        public void ChangeWithParticles() { }
    }
}
