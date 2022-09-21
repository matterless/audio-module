using UnityEngine;

namespace Matterless.Audio
{
    internal interface IAudioInstance
    {
        ulong id { get; }
        AudioSource audioSource { get; }

        void Start();
        void Update();
        void Stop();
        void StopImmediately();
    }
}
