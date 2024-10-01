using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Services.Scene
{
    public sealed class SceneLoader : ISceneLoader
    {
        public async UniTask LoadSceneAsync(string name, Action onComplete = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(name, mode).ToUniTask();
            onComplete?.Invoke();
        }
    }
}