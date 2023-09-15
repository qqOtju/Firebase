using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;

namespace Loading.Operations.Firebase
{
    public class FirebaseInitOperation : ILoadingOperation
    {
        public Action<string> OnDescriptionChange { get; set; }
        public string Description => "Firebase initialization";
        
        public async Task Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.5f);
            var task = await FirebaseApp.CheckDependenciesAsync();
            if (task == DependencyStatus.Available)
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                onProgress?.Invoke(1f);
            }
            else
            {
                OnDescriptionChange?.Invoke("Firebase is unavailable!");
                throw new Exception("Firebase error");
            }
        }
    }
}