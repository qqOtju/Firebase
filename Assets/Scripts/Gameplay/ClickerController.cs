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
        [Header("Data")] 
        [SerializeField] private SoUserData _userData;

        private const float TimeBetweenSaves = 60f;
        
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
            if(pauseStatus) SaveGame();
        }

        private void OnApplicationQuit() =>
            SaveGame();

        private void OnApplicationFocus(bool hasFocus)
        {
            if(!hasFocus) SaveGame();
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
            SaveGame();
        }

        private void ChangeScoreText() =>
            _scoreText.text = $"{_userData.Data.Score}";

        private async void SaveGame()
        {
            if(_docRef == null) return;
            PlayGamesPlatform.Instance.ReportScore(_userData.Data.Score, GPGSIds.leaderboard_leaderboard, success =>
            {
                Debug.Log($"Leaderboard Report: {success}");
            });
            await _docRef.SetAsync(_userData.Data);
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