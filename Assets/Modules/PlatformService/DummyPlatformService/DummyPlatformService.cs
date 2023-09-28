using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.PlatformService.DummyPlatformService
{
    public class DummyPlatformService : MonoBehaviour, IPlatformService
    {
        UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            return UniTask.CompletedTask;
        }

        void IService.Dispose()
        {
        }

        string IPlatformService.GetUserId() => SystemInfo.deviceUniqueIdentifier;

        UniTask<string> IPlatformService.LoadPlayerProgress() => UniTask.FromResult(string.Empty);

        UniTask IPlatformService.SavePlayerProgress(string playerProgress) => UniTask.CompletedTask;

        void IPlatformService.PreloadRewardedVideo()
        {
        }

        async UniTask<bool> IPlatformService.ShowRewardedVideo()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void IPlatformService.PreloadInterstitial()
        {
        }

        async UniTask<bool> IPlatformService.ShowInterstitial()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void IPlatformService.PreloadRewardedInterstitial()
        {
        }

        async UniTask<bool> IPlatformService.ShowRewardedInterstitial()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void IPlatformService.LogEvent(string eventName, Dictionary<string, object> parameters)
        {
        }
    }
}