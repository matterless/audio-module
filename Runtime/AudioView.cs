using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Matterless.Audio
{
    public class AudioView : IAudioView
    {
        private readonly Dictionary<AudioMixerGroup, AudioSource> m_Sources;
        private readonly Dictionary<string, AudioSource> m_LoopSources;
        private readonly GameObject m_GameObject;

        public AudioView(GameObject gameObject)
        {
            m_Sources = new Dictionary<AudioMixerGroup, AudioSource>();
            m_LoopSources = new Dictionary<string, AudioSource>();
            m_GameObject = gameObject;
        }

        public void PlayAudioClipLoop(string id, AudioClip audioClip, AudioMixerGroup audioMixerGroup)
        {
            AudioSource audioSource;

            if (m_LoopSources.ContainsKey(id))
            {
                audioSource = m_LoopSources[id];
            }
            else
            {
                audioSource = m_GameObject.AddComponent<AudioSource>();
                m_LoopSources.Add(id, audioSource);
            }

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.loop = true;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public void StopAudioClipLoop(string id)
        {
            if (m_LoopSources.ContainsKey(id))
                m_LoopSources[id].Stop();
        }

        public void PlayAudioClipOnce(AudioClip audioClip, AudioMixerGroup audioMixerGroup)
        {
            AudioSource audioSource;

            if (m_Sources.ContainsKey(audioMixerGroup))
            {
                audioSource = m_Sources[audioMixerGroup];
            }
            else
            {
                audioSource = m_GameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                m_Sources.Add(audioMixerGroup, audioSource);
            }

            audioSource.PlayOneShot(audioClip);
        }

        public void StopAudioClip(AudioMixerGroup channel)
        {
            if(m_Sources.ContainsKey(channel))
                m_Sources[channel].Stop();
        }

    }
}
