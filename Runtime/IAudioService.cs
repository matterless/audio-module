namespace Matterless.Audio
{
    public interface IAudioService
    {
        void Play(System.Enum id);
        void StopLoop(System.Enum id);
        void Play(string id);
        void StopLoop(string id);
        void StopAudio(string id);
    }
}