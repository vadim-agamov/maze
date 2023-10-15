using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Modules.ServiceLocator;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

namespace Modules.SoundService
{
    public class SoundService: MonoBehaviour, ISoundService
    {
        private SoundsConfig _config;
        private ObjectPool<AudioSource> _objectPool;
        private List<AudioSource> _activeSources;
        private CancellationTokenSource _cancellationToken;
        private bool _isMuted;

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            _config = await Addressables.LoadAssetAsync<SoundsConfig>("SoundsConfig").ToUniTask(cancellationToken: cancellationToken);
            
            gameObject.name = $"[{nameof(SoundService)}]";
            gameObject.AddComponent<AudioListener>();
            _objectPool = new ObjectPool<AudioSource>(OnCreateAudioSource, OnGetAudioSource, OnReleaseAudioSource, OnDestroyAudioSource);
            _activeSources = new List<AudioSource>();
            _cancellationToken = new CancellationTokenSource();
        }

        void IService.Dispose()
        {
            Addressables.Release(_config);
            _cancellationToken.Cancel();
            _objectPool.Dispose();
            Destroy(this);
        }

        void ISoundService.PlayLoop(string soundId)
        {
            var source = _objectPool.Get();
            if (source.clip == null || source.clip.name != soundId)
            {
                source.clip = _config.GetSound(soundId);
                source.clip.name = soundId;
            }

            source.loop = true;
            source.mute = _isMuted;
            source.Play();
        }

        async UniTask ISoundService.Play(string soundId)
        {
            var source = _objectPool.Get();
            if (source.clip == null || source.clip.name != soundId)
            {
                source.clip = _config.GetSound(soundId);
                source.clip.name = soundId;
            }

            source.loop = false;
            source.mute = _isMuted;
            source.Play();

            await UniTask
                .Delay(TimeSpan.FromSeconds(source.clip.length), cancellationToken: _cancellationToken.Token)
                .SuppressCancellationThrow();
            _objectPool.Release(source);
        }

        async UniTask ISoundService.Stop(string soundId)
        {
            foreach (var audioSource in _activeSources)
            {
                if (audioSource.clip.name == soundId)
                {
                    await audioSource.DOFade(0, 0.2f)
                        .ToUniTask(cancellationToken: _cancellationToken.Token)
                        .SuppressCancellationThrow();
                    audioSource.Stop();
                    _objectPool.Release(audioSource);
                    break;
                }
            }
        }

        void ISoundService.Mute()
        {
            _isMuted = true;
            foreach (var activeSource in _activeSources)
            {
                activeSource.mute = true;
            }
        }

        void ISoundService.UnMute()
        {
            _isMuted = false;
            foreach (var activeSource in _activeSources)
            {
                activeSource.mute = false;
            }
        }
        bool ISoundService.IsMuted => _isMuted;

        #region Pool

        private AudioSource OnCreateAudioSource()
        {
            var go = new GameObject("AudioSource");
            go.transform.SetParent(gameObject.transform);
            return go.AddComponent<AudioSource>();
        }

        private void OnDestroyAudioSource(AudioSource item)
        {
            item.Stop();
            Destroy(item.gameObject);
        }

        private void OnReleaseAudioSource(AudioSource item)
        {
            _activeSources.Remove(item);
            item.volume = 1f;
        }

        private void OnGetAudioSource(AudioSource item)
        {
            _activeSources.Add(item);
        }

        #endregion
    }
}