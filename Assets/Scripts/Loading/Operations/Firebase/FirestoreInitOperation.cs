using System;
using System.Threading.Tasks;
using Data;
using Firebase.Firestore;

namespace Loading.Operations.Firebase
{
    public class FirestoreInitOperation : ILoadingOperation
    {
        private readonly PlayGamesLoginOperation _playGames;
        private readonly SoUserData _userData;
        
        Action<string> ILoadingOperation.OnDescriptionChange
        {
            get => OnDescriptionChange;
            set => OnDescriptionChange = value;
        }
        private Action<string> OnDescriptionChange { get; set; }
        public string Description => "Loading data";
        
        public FirestoreInitOperation(PlayGamesLoginOperation playGames, SoUserData userData)
        {
            _playGames = playGames;
            _userData = userData;
        }
        
        public async Task Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.5f);
            var firestore = FirebaseFirestore.DefaultInstance;
            var docRef = firestore.Document($"users/{_playGames.User.UserId}");
            var snapshot = await docRef.GetSnapshotAsync();
            onProgress?.Invoke(0.7f);
            if (!snapshot.Exists)
            {
                var userData = new UserData(){Score = 0, Power = 1, UserId = _playGames.User.UserId};
                _userData.Data = userData;
                await docRef.SetAsync(userData);
            }else
            {
                var userData = snapshot.ConvertTo<UserData>();
                _userData.Data = userData;
            }
            onProgress?.Invoke(1);
        }
    }
}