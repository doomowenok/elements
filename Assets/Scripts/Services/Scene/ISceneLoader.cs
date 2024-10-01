using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Services.Scene
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(string name, Action onComplete = null, LoadSceneMode mode = LoadSceneMode.Single); 
    }
}