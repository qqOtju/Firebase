using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace AdMob
{
    public class AdsController : MonoBehaviour
    {
        //my:   AdBlockId
        //test: ca-app-pub-3940256099942544/5224354917
        private readonly string _rewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
        
        private RewardedAd _rewardedAd;
        private bool _first;

        private void Start()
        {
            _first = true;
            LoadRewardedAd();
        }
        
        private void OnEnable()
        {
            if(!_first) LoadRewardedAd();
        }
        
        public void ShowRewardedAd(Action onUserRewardEarn)
        {
            _rewardedAd.Show((Reward reward) =>
            {
                onUserRewardEarn?.Invoke();
                LoadRewardedAd();
            });
        }
        
        private void LoadRewardedAd()
        {
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }
            var adRequest = new AdRequest.Builder().Build();
            RewardedAd.Load(_rewardedUnitId, adRequest, AdLoadCallback);
        }

        private void AdLoadCallback(RewardedAd ad, LoadAdError error)
        {
            if (error != null || ad == null) return;
            _rewardedAd = ad;
            _rewardedAd.OnAdFullScreenContentClosed += LoadRewardedAd;
            _rewardedAd.OnAdFullScreenContentFailed += _ => LoadRewardedAd();
        }
    }
}