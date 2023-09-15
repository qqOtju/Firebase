using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Loading.Operations
{
    public class LoadingSceneOperation : ILoadingOperation
    {
        private readonly string _sceneToLoad;
        
        public Action<string> OnDescriptionChange { get; set; }
        public string Description => "Loading scene";

        public LoadingSceneOperation(string sceneName)
        {
            _sceneToLoad = sceneName;
        }
        
        public async Task Load(Action<float> onProgress)
        {
            onProgress?.Invoke(.5f);
            var op = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
            op.allowSceneActivation = false;
            var completionSource = new TaskCompletionSource<bool>();
            async Task LoadScene()
            {
                while (!op.isDone)
                {
                    onProgress?.Invoke(op.progress);
                    if (op.progress >= 0.9f)
                    {
                        op.allowSceneActivation = true;
                    }
                    await Task.Yield();
                }
                completionSource.SetResult(true);
            }
            _ = LoadScene();
            await completionSource.Task;
            onProgress?.Invoke(1f);
        }
    }
}