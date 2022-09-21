using System;

namespace Matterless.Audio
{
    internal interface IAudioView
    {
        ulong Play(AudioEvent audioEvent, Action onAutoStopped);
        bool Stop(ulong instnaceId, bool immediately);
    }
}