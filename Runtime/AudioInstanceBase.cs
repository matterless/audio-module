using System;
using UnityEngine;

namespace Matterless.Audio
{
    internal abstract class AudioInstanceBase : IAudioInstance
    {
        protected readonly AudioSource m_AudioSource;
        protected readonly AudioEvent m_AudioEvent;
        protected readonly Action m_OnAutoStopped;
        protected readonly Action<IAudioInstance> m_OnComplete;

        private readonly FadeController m_FadeController;
        private readonly float m_FadeOutStartTime;

        private ulong m_Id;
        private bool m_ToStop = false;
        protected bool m_ManualyStopped = false;
        private float m_Timer;

        ulong IAudioInstance.id => m_Id;
        AudioSource IAudioInstance.audioSource => m_AudioSource;

      

        protected abstract void OnAudioClipCompleted();
        protected abstract void OnStart();

        internal AudioInstanceBase(ulong id, AudioSource audioSource,
            AudioEvent audioEvent,
            Action onAutoStopped,
            Action<IAudioInstance> onComplete)
        {
            m_Id = id;
            m_AudioSource = audioSource;
            m_AudioEvent = audioEvent;
            m_OnAutoStopped = onAutoStopped;
            m_OnComplete = onComplete;
            m_FadeController = new FadeController(audioSource);
            // cache for optimisation
            m_FadeOutStartTime = m_AudioEvent.fadeOutStartTime;
        }

        void IAudioInstance.Start()
        {
            if (m_AudioEvent.hasFadeIn)
            {
                m_AudioSource.volume = 0f;
                m_FadeController.FadeIn(m_AudioEvent.fadeInDuration, m_AudioEvent.maxAmplitude);
            }
            else
                m_AudioSource.volume = m_AudioEvent.maxAmplitude;

            // start playing things
            OnStart();
        }

        void IAudioInstance.Stop()
        {
            m_ManualyStopped = true;

            m_ToStop = true;
            // start fade out and stop
            m_FadeController.FadeOut(m_AudioEvent.fadeOutDuration,
                () =>
                {
                    m_AudioSource.Stop();
                    m_OnComplete?.Invoke(this);
                });
        }

        void IAudioInstance.StopImmediately()
        {
            m_ManualyStopped = true;
            m_AudioSource.Stop();
            m_OnComplete?.Invoke(this);
        }

        void IAudioInstance.Update()
        {
            // update timer
            m_Timer += Time.deltaTime;
            // update fade controller
            m_FadeController?.Update();

            // check for fade out event in NON LOOPING events
            // we need to start the fade out x seconds prior the end of the total duration fo the event
            if (m_AudioSource.isPlaying 
                && !m_AudioEvent.isLoop 
                && m_AudioEvent.hasFadeOut 
                && !m_ToStop 
                && m_Timer >= m_FadeOutStartTime)
            {
                StartFadeOut();
            }

            // check if audio has stoped
            if (!m_AudioSource.isPlaying)
            {
                OnAudioClipCompleted();
            }
        }

        private void StartFadeOut()
        {
            m_ToStop = true;
            m_FadeController.FadeOut(m_AudioEvent.fadeOutDuration);
        }
    }
}