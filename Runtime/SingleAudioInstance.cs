using System;
using UnityEngine;

namespace Matterless.Audio
{
    internal class SingleAudioInstance : AudioInstanceBase
    {
        internal SingleAudioInstance(ulong id, AudioSource audioSource,
            AudioEvent audioEvent,
            Action onAutoStopped,
            Action<IAudioInstance> onComplete)
            : base(id, audioSource, audioEvent, onAutoStopped, onComplete) { }


        protected override void OnStart()
        {
            m_AudioSource.loop = m_AudioEvent.isLoop;
            m_AudioSource.clip = m_AudioEvent.audioClip[0];
            m_AudioSource.Play();
        }

        protected override void OnAudioClipCompleted()
        {
            // if not a loop and not a manual stop, this is a complete event
            if (!m_AudioEvent.isLoop && !m_ManualyStopped)
            {
                m_OnComplete?.Invoke(this);
                m_OnAutoStopped?.Invoke();
            }
        }
    }
}
