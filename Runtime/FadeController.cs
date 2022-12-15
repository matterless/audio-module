using System;
using UnityEngine;

namespace Matterless.Audio
{
    internal class FadeController
    {
        private readonly AudioSource m_AudioSource;
        private Action m_OnComplete;
        private float m_StartValue;
        private float m_StartTime;
        private float m_EndTime;
        private float m_EndValue;

        private bool m_InProgress = false;

        internal FadeController(AudioSource audioSource)
        {
            m_AudioSource = audioSource;
        }

        internal void FadeIn(float duration, float target, Action onComplete = null) 
            => Fade(duration, m_AudioSource.volume, target, onComplete);

        internal void FadeOut(float duration, Action onComplete = null)
        {
            if (duration < float.Epsilon)
                onComplete?.Invoke();
            else
                Fade(duration, m_AudioSource.volume, 0, onComplete);
        }

        private void Fade(float duration, float startValue, float endValue, Action onComplete)
        {
            m_OnComplete = onComplete;
            m_StartTime = Time.time;
            m_EndTime = Time.time + duration;
            m_StartValue = startValue;
            m_EndValue = endValue;
            m_InProgress = true;
        }

        internal void Update()
        {
            if (!m_InProgress)
                return;

            float lerpValue = Mathf.InverseLerp(m_StartTime, m_EndTime, Time.time);
            // simple square
            lerpValue *= lerpValue;

            if (lerpValue >= 1 || Time.time >= m_EndTime)
            {
                m_InProgress = false;
                m_AudioSource.volume = m_EndValue;
                m_OnComplete?.Invoke();
            }

            m_AudioSource.volume = Mathf.Lerp(m_StartValue, m_EndValue, lerpValue);
        }
    }
}
