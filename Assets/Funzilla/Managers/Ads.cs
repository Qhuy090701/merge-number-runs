
using System;
using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
using Facebook.Unity;
#endif

namespace Funzilla
{
	internal enum RewardedVideoState
	{
#if !UNITY_EDITOR
		Closed, NotReady,
#endif
		Failed, Watched
	}

	internal class Ads : Singleton<Ads>
	{
#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")] private static extern bool isIos14();
		[DllImport("__Internal")] private static extern bool advertiserTrackingPrompted();
		[DllImport("__Internal")] private static extern void promptAdvertiserTracking();
		[DllImport("__Internal")] private static extern bool advertiserTrackingEnabled();
#endif
#if UNITY_ANDROID
		private const string InterstitialAdUnitId = "1d9573a5ead237ee";
		private const string RewardedAdUnitId = "561d4b3e443dde68";
		private const string BannerAdUnitId = "8a838d9ecbb5a3a9";
#else
		private const string InterstitialAdUnitId = "b9793012f5dd373c";
		private const string RewardedAdUnitId = "c27ae5fc233bf32c";
		private const string BannerAdUnitId = "8a838d9ecbb5a3a9";
#endif
		
#if !UNITY_EDITOR && !DEBUG_ENABLED
		internal const string SdkName = "Max";
#endif

		private enum State { NotInitialized, Initializing, Initialized }
		private State _state = State.NotInitialized;

		internal void Init()
		{
			if (_state != State.NotInitialized)
			{
				return;
			}

			_state = State.Initializing;

#if !UNITY_EDITOR && UNITY_IOS
			if (isIos14())
			{
				promptAdvertiserTracking();
			}
			else
			{
				InitSDK();
			}
#else
			InitSDK();
#endif
		}

#if !UNITY_EDITOR && UNITY_IOS
		private void Update()
		{
			if (!advertiserTrackingPrompted())
			{
				return;
			}
			InitSDK();
		}
#endif

		private void InitSDK()
		{
			enabled = false;
			Adjust.Init();
#if !UNITY_EDITOR
#if UNITY_IOS
			FB.Mobile.SetAdvertiserTrackingEnabled(advertiserTrackingEnabled());
			if (advertiserTrackingEnabled())
			{
				var userId = com.adjust.sdk.Adjust.getIdfa();
				Adjust.SetUserId(userId);
				Firebase.Analytics.FirebaseAnalytics.SetUserId(userId);
			}
#endif
#endif

			MaxSdkCallbacks.OnSdkInitializedEvent += _ =>
			{
				InitializeInterstitial();
				InitializeRewardedAds();
				InitializeBannerAds();
				_state = State.Initialized;
			};

			MaxSdk.InitializeSdk();
		}

		private static void ShowMessage(string msg)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject activity =
			new AndroidJavaClass("com.unity3d.player.UnityPlayer").
			GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject toastClass = new AndroidJavaClass("android.widget.Toast");
			toastClass.CallStatic<AndroidJavaObject>("makeText", activity, msg, toastClass.GetStatic<int>("LENGTH_SHORT")).Call("show");
#else
			Debug.LogWarning(msg);
#endif
		}

		#region Interstitial Ad Methods
		
		private bool _interstitialShown;
		private float _lastInterstitialShowTime;
		private Action _onIntersitialRequestProcessed;
		private string _interstitialPlace;
		private bool InterstitialAllowed { get; set; } = true;
		
		private bool InterstitialValid => _state == State.Initialized;
		
		private void InitializeInterstitial()
		{
			// Attach callbacks
			MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
			MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
			MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
			MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
			MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
			MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;

			LoadInterstitial();
			_lastInterstitialShowTime = Time.realtimeSinceStartup;
		}

		private void ShowReadyInterstitial(Action onFinished)
		{
			if (!InterstitialAllowed)
			{
				onFinished();
				return;
			}

			try
			{
				_onIntersitialRequestProcessed = onFinished;
				if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
				{
					MaxSdk.ShowInterstitial(InterstitialAdUnitId);
				}
			}
			catch
			{
				onFinished?.Invoke();
			}
		}

		private bool CanShowInterstitial
		{
			get
			{
				if (Profile.Vip)
				{
#if DEBUG_ENABLED
					Debug.LogError("Cannot show interstitial to VIP");
#endif
					return false;
				}

				// Check availability
				if (!InterstitialValid)
				{ // Not ready yet
#if DEBUG_ENABLED
					Debug.LogError("Interstitial is not either initialized or loaded");
#endif
					return false;
				}

				// Check capping
				if (Time.realtimeSinceStartup - _lastRewardedVideoShowTime < Config.InterstitialRewardedVideoCappingTime)
				{
#if DEBUG_ENABLED
					var t = Time.realtimeSinceStartup - _lastRewardedVideoShowTime;
					Debug.LogError("Rewarded video opened " + t + " seconds ago. Need to wait " +
						(Config.InterstitialRewardedVideoCappingTime - t) + " seconds to show interstitial");
#endif
					return false;
				}
				if (!_interstitialShown)
				{
					if (Time.realtimeSinceStartup - _lastInterstitialShowTime < Config.FirstInterstitialCappingTime)
					{
#if DEBUG_ENABLED
							var t = Time.realtimeSinceStartup - _lastInterstitialShowTime;
							Debug.LogError("Need wait " +
								(Config.FirstInterstitialCappingTime - t) + " seconds to show interstitial");
#endif
						return false;
					}
				}
				else
				{
					if (Time.realtimeSinceStartup - _lastInterstitialShowTime < Config.InterstitialCappingTime)
					{
#if DEBUG_ENABLED
							var t = Time.realtimeSinceStartup - _lastInterstitialShowTime;
							Debug.LogError("Interstitial opened " + t + " seconds ago. Need to wait " +
								(Config.InterstitialCappingTime - t) + " seconds to show interstitial");
#endif
						return false;
					}
				}
				return true;
			}
		}

		internal static bool ShowInterstitial(string place, Action onFinished = null)
		{
			if (!Instance.CanShowInterstitial)
			{
				onFinished?.Invoke();
				return false;
			}

			Instance._interstitialPlace = place;
			if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
			{
				Instance.ShowReadyInterstitial(onFinished);
				return true;
			}
			onFinished?.Invoke();
			return false;
		}

		private void LoadInterstitial()
		{
			MaxSdk.LoadInterstitial(InterstitialAdUnitId);
			if (_onIntersitialRequestProcessed == null) return;
			_onIntersitialRequestProcessed();
			_onIntersitialRequestProcessed = null;
		}

		private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			Analytics.LogInterstitialClickedEvent(_interstitialPlace);
		}

		private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			_interstitialShown = true;
			Analytics.LogInterstitialShownEvent(_interstitialPlace);
		}

		private static void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
		}

		private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errInfo)
		{
			Analytics.LogInterstitialFailedEvent(_interstitialPlace);
			LoadInterstitial();
		}

		private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
		{
			Analytics.LogInterstitialFailedEvent(_interstitialPlace);
			LoadInterstitial();
		}

		private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			LoadInterstitial();
		}

		#endregion

		#region Rewarded Ad Methods

		private int _rwLoadAttempts;
		private float _lastRewardedVideoShowTime;
		private Action<RewardedVideoState> _rewardedVideoCallback;
		private RewardedVideoState _rewardedVideoState;
		private string _rewardedVideoPlace;
		
#if UNITY_EDITOR
		internal bool RewardedVideoReady => true;
#else
		internal bool RewardedVideoReady => _state == State.Initialized && MaxSdk.IsRewardedAdReady(RewardedAdUnitId);
#endif

		private static void ShowRewardedVideoFailMessage()
		{
			ShowMessage(Application.internetReachability == NetworkReachability.NotReachable
				? "No internet connection. Try again"
				: "No video available at the moment. Try again later");
		}

		internal static void ShowRewardedVideo(string place, Action<RewardedVideoState> callback)
		{
			Instance._rewardedVideoPlace = place;
#if UNITY_EDITOR
			callback(RewardedVideoState.Watched);
#else
			if (Instance._rewardedVideoCallback != null)
			{ // Previous rewarded video request is not finished yet
				callback(RewardedVideoState.Closed);
				return;
			}
			if (!Instance.RewardedVideoReady)
			{
				Analytics.LogRewardedVideoFailedEvent(place);
				callback(RewardedVideoState.NotReady);
				ShowRewardedVideoFailMessage();
				return;
			}
			Analytics.LogRewardVideoClickedEvent(place);
			Instance._rewardedVideoState = RewardedVideoState.Closed;
			Instance._rewardedVideoCallback = callback;
			SceneManager.ShowLoading();
			if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
			{
				MaxSdk.ShowRewardedAd(RewardedAdUnitId);
			}
#endif
		}


		private void OnRewardedVideoFailed()
		{
			ShowRewardedVideoFailMessage();
			SceneManager.HideLoading();
			_rewardedVideoCallback?.Invoke(RewardedVideoState.Failed);
			_rewardedVideoCallback = null;
		}

		private void OnRewardedVideoAdClosed()
		{
			if (_rewardedVideoState == RewardedVideoState.Watched)
			{
				_lastRewardedVideoShowTime = Time.realtimeSinceStartup;
				_interstitialShown = true;
				Analytics.LogRewardVideoWatchedEvent(_rewardedVideoPlace);
			}
			SceneManager.HideLoading();
			SoundManager.Resume();
			_rewardedVideoCallback?.Invoke(_rewardedVideoState);
			_rewardedVideoCallback = null;
		}

		private void InitializeRewardedAds()
		{
			// Attach callbacks
			MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
			MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
			MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
			MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
			MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
			MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
			MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

			// Load the first RewardedAd
			LoadRewardedAd();
		}

		private void LoadRewardedAd()
		{
			MaxSdk.LoadRewardedAd(RewardedAdUnitId);
		}


		private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			_rwLoadAttempts = 0;
		}

		private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
		{
#if DEBUG_ENABLED
			Debug.LogError("Rewarded ad failed to load with error code: " + errorCode);
#endif
			_rwLoadAttempts++;
			var retryDelay = Math.Pow(2, Math.Min(6, _rwLoadAttempts));
			Invoke(nameof(LoadRewardedAd), (float) retryDelay);
		}

		private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
		{
			Analytics.LogRewardedVideoFailedEvent(_rewardedVideoPlace);
			OnRewardedVideoFailed();
			LoadRewardedAd();
		}

		private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			Analytics.LogRewardedVideoShownEvent(_rewardedVideoPlace);
			SceneManager.HideLoading();
			SoundManager.Pause();
		}

		private static void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
		}

		private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			OnRewardedVideoAdClosed();
			LoadRewardedAd();
		}

		private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
		{
			_rewardedVideoState = RewardedVideoState.Watched;
		}

		#endregion

		#region Banner Ad Methods

		private static void InitializeBannerAds()
		{
			// Attach Callbacks
			MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
			MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
			MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
			MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

			// Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
			// You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
			MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

			// Set background or background color for banners to be fully functional.
			MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.black);
			MaxSdk.ShowBanner(BannerAdUnitId);
		}

		private static void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			// Banner ad is ready to be shown.
			// If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
#if DEBUG_ENABLED
			Debug.Log("Banner ad loaded");
#endif
		}

		private static void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
		{
			// Banner ad failed to load. MAX will automatically try loading a new ad internally.
#if DEBUG_ENABLED
			Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
#endif
		}

		private static void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
#if DEBUG_ENABLED
			Debug.Log("Banner ad clicked");
#endif
		}

		private static void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			// Banner ad revenue paid. Use this callback to track user revenue.
#if DEBUG_ENABLED
			Debug.Log("Banner ad revenue paid");
#endif
		}

		#endregion
	}
}
