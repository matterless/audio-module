using UnityEngine;
using UnityEngine.Audio;

namespace Matterless.Audio
{
    public interface IAudioView
    {
        void PlayAudioClipOnce(AudioClip audioClip, AudioMixerGroup audioMixerGroup);
        void PlayAudioClipLoop(string id, AudioClip audioClip, AudioMixerGroup audioMixerGroup);
        void StopAudioClipLoop(string id);
        void StopAudioClip(AudioMixerGroup channel);
    }
}