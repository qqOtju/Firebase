using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Loading.Operations
{
    public class TestOperation : ILoadingOperation
    {
        public Action<string> OnDescriptionChange { get; set; }
        public string Description => "Testing";
        
        public async Task Load(Action<float> onProgress)
        {
            var value = 0f;
            while (value <= 1)
            {
                onProgress?.Invoke(value);
                await Task.Yield();
                value += Time.deltaTime;
            }
            onProgress?.Invoke(1);
        }
    }
}