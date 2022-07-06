using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Matterless.Audio
{
    public enum AudioEventType { PlayOnce, Loop, PlayOnceShuffled, LoopShuffled }

    [CreateAssetMenu(fileName = "AudioEvent", menuName = "Matterless/Audio/Event", order = 2)]
    public class AudioEvent : ScriptableObject
    {
        [SerializeField] private string m_Id;
        [SerializeField] private AudioEventType m_Type = AudioEventType.PlayOnce;
        [SerializeField] private List<AudioClip> m_AudioClip;
        [SerializeField] private AudioMixerGroup m_Channel;

        public string id => m_Id;
        public AudioEventType type => m_Type;
        public List<AudioClip> audioClip { get => m_AudioClip; internal set => m_AudioClip = value; }
        public AudioMixerGroup channel => m_Channel;
    }
}