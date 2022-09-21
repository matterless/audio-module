using System;
using System.Collections.Generic;
using UnityEngine;

namespace Matterless.Audio
{
    public class AudioView : IAudioView
    {
        private readonly List<IAudioInstance> m_AudioInstances;
        private readonly Dictionary<ulong, IAudioInstance> m_AudioInstancesDictionary;
        private readonly Stack<AudioSource> m_AudioSourcePool;
        private readonly AudioMono m_AudioMono;
        private ulong m_IdCounter;

        public AudioView(AudioMono audioMono)
        {
            m_AudioSourcePool = new Stack<AudioSource>();
            m_AudioMono = audioMono;
            m_AudioMono.onUpdate += Update;
            m_IdCounter = 0;

            m_AudioInstances = new List<IAudioInstance>();
            m_AudioInstancesDictionary = new Dictionary<ulong, IAudioInstance>();

        }

        ulong IAudioView.Play(AudioEvent audioEvent, Action onAutoStopped)
        {
            // 1. Get available audiosource
            var audioSource = GetAvailableAudioSource();
            // 2. Set settings
            audioSource.outputAudioMixerGroup = audioEvent.channel;
            // 3. generate an id
            var id = ++m_IdCounter;
            // 4. create audio Instance
            var audioInstance = CreateAudioInstance(audioSource, audioEvent, id, onAutoStopped);
            // 5. register audio Instance
            m_AudioInstances.Add(audioInstance);
            m_AudioInstancesDictionary.Add(id, audioInstance);
            // 6. play
            audioInstance.Start();
            // 7. return id
            return id;
        }

        bool IAudioView.Stop(ulong instnaceId, bool immediately)
        {
            if (m_AudioInstancesDictionary.ContainsKey(instnaceId))
            {
                //Debug.Log($"Stop audio instance {instnaceId}");

                if (immediately)
                    m_AudioInstancesDictionary[instnaceId].StopImmediately();
                else
                    m_AudioInstancesDictionary[instnaceId].Stop();

                return true;
            }

            Debug.LogWarning($"Audio instance {instnaceId} does not exists");

            return false;
        }

        private IAudioInstance CreateAudioInstance(AudioSource audioSource, 
            AudioEvent audioEvent, ulong id, Action onAutoStopped)
        {
            if (audioEvent.isSignleEvent)
                return new SingleAudioInstance(id, audioSource, audioEvent,
                    onAutoStopped, OnAudioInstanceComplete);
            else
                return new MultiAudioInstance(id, audioSource, audioEvent,
                    onAutoStopped, OnAudioInstanceComplete);

            throw new Exception("Create Audio Instance unresolved.");
        }

        private void Update()
        {
            for (int i = 0; i < m_AudioInstances.Count; i++)
                m_AudioInstances[i].Update();
        }

        private void OnAudioInstanceComplete(IAudioInstance audioInstance)
        {
            //Debug.Log($"Remove audio instance {audioInstance.id}");
            //Debug.Log($"Push audio source {audioInstance.audioSource.GetInstanceID()}");
            m_AudioInstances.Remove(audioInstance);
            m_AudioInstancesDictionary.Remove(audioInstance.id);
            m_AudioSourcePool.Push(audioInstance.audioSource);
            audioInstance.audioSource.clip = null;
            audioInstance.audioSource.loop = false;
        }

        private AudioSource GetAvailableAudioSource()
        {
            if (m_AudioSourcePool.Count == 0)
            {
                var audioSource = m_AudioMono.gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                return audioSource;
            }

            return m_AudioSourcePool.Pop();
        }
    }
}
