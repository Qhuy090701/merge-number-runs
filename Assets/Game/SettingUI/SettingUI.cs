
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class SettingUI : Popup
	{
#if UNITY_IOS
		private const string TermsUrl = "https://www.apple.com/legal/internet-services/itunes/dev/stdeula/";
#else
		private const string TermsUrl = "https://funzilla.io/terms";
#endif
		[SerializeField] private ToggleButton sfxToggle;
		[SerializeField] private ToggleButton musicToggle;
		[SerializeField] private ToggleButton vibrateToggle;
		[SerializeField] private Button rateButton;
		[SerializeField] private Button privacyPolicyButton;
		[SerializeField] private Button termOfUseButton;
		[SerializeField] private Button closeButton;
		[SerializeField] private Text versionText;

		private void Awake()
		{
			versionText.text = "v" + Application.version;
		}

		private void OnEnable()
		{
#if UNITY_IOS
			rateButton.onClick.AddListener(() => UnityEngine.iOS.Device.RequestStoreReview());
#else
			rateButton.onClick.AddListener(() => Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier));
#endif
			privacyPolicyButton.onClick.AddListener(() => Application.OpenURL("https://www.funzilla.io/games/privacy"));
			termOfUseButton.onClick.AddListener(() => Application.OpenURL(TermsUrl));
			sfxToggle.Init(Preference.SfxOn, active => Preference.SfxOn = active);
			musicToggle.Init(Preference.MusicOn, active => Preference.MusicOn = active);
			vibrateToggle.Init(Preference.VibrationOn, active => Preference.VibrationOn = active);
			closeButton.onClick.AddListener(SceneManager.ClosePopup);
		}

		internal override void OnBackButtonPressed()
		{
			SceneManager.ClosePopup();
		}
	}
}