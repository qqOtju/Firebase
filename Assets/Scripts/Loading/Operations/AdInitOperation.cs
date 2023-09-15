using System;
using System.Threading.Tasks;
using GoogleMobileAds.Api;

namespace Loading.Operations
{
    public class AdInitOperation : ILoadingOperation
    {
        public Action<string> OnDescriptionChange { get; set; }
        public string Description => "Loading ads";

        public Task Load(Action<float> onProgress)
        {
            onProgress?.Invoke(.5f);
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            var requestConfiguration = new RequestConfiguration.Builder().SetSameAppKeyEnabled(true).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.Initialize(_=>{});
            onProgress?.Invoke(1);
            return Task.CompletedTask;
        }
    }
}