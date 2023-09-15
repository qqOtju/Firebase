using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using Utils.Constants;

namespace Loading
{
    public class LoadingScreenProvider : LocalAssetLoader
    {
        public async Task LoadAndDestroy(Queue<ILoadingOperation> loadingOperations)
        {
            var loadingScreen = await Load<LoadingScreen>(Constants.LoadingScreen);
            await loadingScreen.Load(loadingOperations);
            Unload();
        }
    }
}