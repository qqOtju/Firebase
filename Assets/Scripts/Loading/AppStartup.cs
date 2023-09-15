using System.Collections.Generic;
using Data;
using Loading.Operations;
using Loading.Operations.Firebase;
using UnityEngine;

namespace Loading
{
    public class AppStartup : MonoBehaviour
    {
        [Header("Scene")]
        [SerializeField] private string _scene;
        [Header("Data")]
        [SerializeField] private SoUserData _data;
        
        private async void Awake()
        {
            var loadingOperations = new Queue<ILoadingOperation>();
            var playGames = new PlayGamesLoginOperation();
            loadingOperations.Enqueue(playGames);
            loadingOperations.Enqueue(new AdInitOperation());
            loadingOperations.Enqueue(new FirebaseInitOperation());
            loadingOperations.Enqueue(new FirestoreInitOperation(playGames, _data));
            loadingOperations.Enqueue(new LoadingSceneOperation(_scene));
            await new LoadingScreenProvider().LoadAndDestroy(loadingOperations);
        }
    }
}