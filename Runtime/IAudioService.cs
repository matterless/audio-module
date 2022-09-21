using System;

namespace Matterless.Audio
{
    public interface IAudioService
    {
        ulong Play(string id, Action onAutoStopped = null);
        ulong Play(Enum id, Action onAutoStopped = null);
        bool Stop(ulong instnaceId, bool immediately = false);
        void TransitionTo(string snapshotName, float duration);
    }
}