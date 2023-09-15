using System;
using System.Threading.Tasks;
using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Loading.Operations
{
    public class PlayGamesLoginOperation : ILoadingOperation
    {
        private readonly TaskCompletionSource<FirebaseUser> _tcsUser = new();
        private Action<float> _onProgress;

        public Action<string> OnDescriptionChange { get; set; }
        public FirebaseUser User { get; private set; }
        public string Description => "Authentication";
        
        public async Task Load(Action<float> onProgress)
        {
            _onProgress = onProgress;
            _onProgress?.Invoke(0.3f);
            PlayGamesPlatform.Instance.Authenticate(Authenticate);
            await _tcsUser.Task;
            _onProgress?.Invoke(1f);
        }

        private async void Authenticate(SignInStatus status)
        {
            switch (status)
            {
                case SignInStatus.Success:
                    _onProgress?.Invoke(0.5f);
                    await AccessRequest();
                    break;
                case SignInStatus.InternalError:
                case SignInStatus.Canceled:
                default:
                    OnDescriptionChange?.Invoke("Authentication failed");
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
        
        private async Task AccessRequest()
        {
            var code = "";
            var tcs = new TaskCompletionSource<string>();
            PlayGamesPlatform.Instance.RequestServerSideAccess
            (true, c =>
            {
                code = c;
                tcs.SetResult(c);
            });
            await tcs.Task;
            await RequestServerSideAccess(code);
        }
        
        private async Task RequestServerSideAccess(string code)
        {
            var auth = FirebaseAuth.DefaultInstance;
            var credential = PlayGamesAuthProvider.GetCredential(code);
            _onProgress?.Invoke(0.7f);
            User = await auth.SignInWithCredentialAsync(credential);
            _tcsUser.SetResult(User);
        }
    }
}