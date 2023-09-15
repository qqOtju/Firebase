using System;
using System.Collections;
using AdMob;
using Data;
using Firebase.Firestore;
using GooglePlayGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class ClickerController : MonoBehaviour
    {
        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI _scoreText;
        [Header("Buttons")] 
        [SerializeField] private Button _clickArea;
        [SerializeField] private Button _adButton;
        [SerializeField] private Button _leaderboardButton;
        [Header("References")] 
        [SerializeField] private AdsController _adsController;
        [SerializeField] private Image _saveGameIcon;
        [Header("Data")] 
        [SerializeField] private SoUserData _userData;

        private const float TimeBetweenSaves = 120f;
        
        private DocumentReference _docRef;

        private void Awake()
        {
            _clickArea.onClick.AddListener(OnClick);
            _adButton.onClick.AddListener(ShowAd);
            _leaderboardButton.onClick.AddListener(ShowLeaderboard);
        }

        private void Start()
        { 
            _docRef = FirebaseFirestore.DefaultInstance.Document($"users/{_userData.Data.UserId}");
            ChangeScoreText();
            StartCoroutine(SaveCoroutine(TimeBetweenSaves));
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus)
                SaveGame(false);
        }

        private void OnApplicationQuit()
        {
            SaveGame(false);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(!hasFocus)
                SaveGame(false);
        }

        private void OnClick()
        {
            var userDataData = _userData.Data;
            userDataData.Score += userDataData.Power;
            _userData.Data = userDataData;
            ChangeScoreText();
        }

        private void ShowAd() =>
            _adsController.ShowRewardedAd(OnUserRewardEarn);

        private void OnUserRewardEarn()
        {
            var userDataData = _userData.Data;
            userDataData.Power++;
            _userData.Data = userDataData;
            SaveGame();
        }

        private void ShowLeaderboard()
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_leaderboard);
            SaveGame(false);
        }

        private void ChangeScoreText() =>
            _scoreText.text = $"{_userData.Data.Score}";

        private async void SaveGame(bool anim = true)
        {
            if(_docRef == null) return;
            PlayGamesPlatform.Instance.ReportScore(_userData.Data.Score, GPGSIds.leaderboard_leaderboard, _ => {});
            if(anim) SaveAnimation();
            await _docRef.SetAsync(_userData.Data);
        }

        private void SaveAnimation()
        {
            var colorA = _saveGameIcon.color;
            var colorB = colorA;
            colorB.a = 0;
            StartCoroutine(SaveAnimation(colorA, colorB));
        }

        private IEnumerator SaveAnimation(Color colorA, Color colorB)
        {
            var value = 0f;
            while (value <= 1)
            {
                value += Time.deltaTime;
                _saveGameIcon.color = Color.Lerp(colorA, colorB, value);
                yield return null;
            }

            value = 1f;
            while (value >= 0)
            {
                value += Time.deltaTime;
                _saveGameIcon.color = Color.Lerp(colorA, colorB, value);
                yield return null;
            }
        }

        private IEnumerator SaveCoroutine(float time)
        {
            var wfs = new WaitForSeconds(time);
            while (true)
            {
                yield return wfs;
                SaveGame();
            }
        }
    }
}