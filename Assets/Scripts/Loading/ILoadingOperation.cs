using System;
using System.Threading.Tasks;

namespace Loading
{
    public interface ILoadingOperation
    {
        public Action<string> OnDescriptionChange { get; set; }
        public string Description { get; }

        public Task Load(Action<float> onProgress);
    }
}