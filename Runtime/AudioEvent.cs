using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Matterless.Audio
{
    //public enum AudioEventType { PlayOnce, Loop, PlayOnceShuffled, LoopShuffled }
    public enum SequenceType { Order = 0, Shuffle, Random, SignleRandom }

    [CreateAssetMenu(fileName = "AudioEvent", menuName = "Matterless/Audio/Event", order = 2)]
    public class AudioEvent : ScriptableObject
    {
        [Header("General Settings")]
        [SerializeField] private string m_Id;
        [SerializeField] private AudioMixerGroup m_Channel;
        [SerializeField] private SequenceType m_SequenceType;
        [SerializeField] private bool m_Loop;

        [Header("Sounds")]
        [SerializeField, FormerlySerializedAs("m_AudioClip")] private List<AudioClip> m_AudioClips;

        [Header("Envelope")]
        [SerializeField, Range(0f, 1f)] private float m_MaxAmplitude = 1;
        [SerializeField] private Envelope m_Attack;
        [SerializeField] private Envelope m_Release;

        private void OnValidate()
        {
            if(!m_Loop)
            {
                if (fadeOutStartTime < duration * 0.5f)
                    Debug.LogError("Fade out can not be less than the 50% of the total duration of the event");
                if(m_Attack.duration > duration * 0.5f)
                    Debug.LogError("Fade in can not be greater than the 50% of the total duration of the event");
            }
        }

        internal float duration
        {
            get
            {
                float totalDuration = 0;

                foreach (var clip in m_AudioClips)
                    totalDuration = clip.length;

                return totalDuration;
            }
        }
        internal float fadeOutStartTime => duration - m_Release.duration;
        internal string id => m_Id;
        internal List<AudioClip> audioClip => m_AudioClips;
        internal AudioMixerGroup channel => m_Channel;
        internal bool isLoop => m_Loop;
        internal SequenceType sequenceType => m_SequenceType;
        internal float fadeInDuration => m_Attack.duration;
        internal float fadeOutDuration => m_Release.duration;
        internal bool hasFadeIn => m_Attack.hasEnvelope;
        internal bool hasFadeOut => m_Release.hasEnvelope;
        internal float maxAmplitude => m_MaxAmplitude;

        // signle event
        internal bool isSignleEvent => m_AudioClips.Count == 1 || m_SequenceType == SequenceType.SignleRandom;
        // multi event
        internal bool isMultiEvent => m_AudioClips.Count > 1;
        // clips count
        internal int count => m_AudioClips.Count;
    }
}