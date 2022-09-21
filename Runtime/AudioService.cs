using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace Matterless.Audio
{
    public class AudioService : IAudioService
    {
        private readonly AudioDatabase m_AudioDatabase;
        private readonly IAudioView m_AudioView;
        private readonly AudioMono m_AudioMono;
        
        [Preserve]
        public AudioService(
            // arguments
            AudioDatabase audioDatabase,
            bool enableLocalEditor)
        {
            if (audioDatabase == null)
                throw new Exception("Audio database is missing.");
            
            m_AudioDatabase = audioDatabase;
            m_AudioMono = new GameObject("_AudioMono_").AddComponent<AudioMono>();
            m_AudioDatabase.Init(enableLocalEditor, GetLocalDevelopAudioClip);
            m_AudioView = new AudioView(m_AudioMono);
        }

        ~AudioService()
        {
            if (m_AudioMono != null)
                GameObject.Destroy(m_AudioMono.gameObject);
        }

        public ulong Play(string id, Action onAutoStopped = null)
        {
            if (m_AudioDatabase == null)
                throw new Exception("Audio database is missing.");

            var audio = m_AudioDatabase.GetAudioEvent(id);

            if (audio == null)
            {
                Debug.LogWarning($"Audio event is missing: {id}");
                return 0;
            }

            if (audio.audioClip == null)
            {
                Debug.LogWarning($"Audio event has null audio clip: {id}");
                return 0;
            }

            return m_AudioView.Play(audio, onAutoStopped);
        }

        public bool Stop(ulong instnaceId, bool immediately = false) => m_AudioView.Stop(instnaceId, immediately);

        public void TransitionTo(string snapshotName, float duration)
            => m_AudioDatabase.GetAudioMixerSnapshot(snapshotName).TransitionTo(duration);
        

        #region Developement Environment
        private void GetLocalDevelopAudioClip(string id, Action<AudioClip> callback)
        {
            m_AudioMono.StartCoroutine(GetLocalDevelopAudioClipAsync(id, callback));
        }

        IEnumerator GetLocalDevelopAudioClipAsync(string id, Action<AudioClip> callback)
        {
            string filename = $"{id}.mp3";
            string path = $"file://{Application.persistentDataPath}/{filename}";

            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);

            yield return www.SendWebRequest();

            if (www.responseCode != 200)
            {
                Debug.LogWarning($"Audio file {filename} is missing.");
                callback(null);
            }
            else
            {
                callback(DownloadHandlerAudioClip.GetContent(www));
            }
        }
        #endregion
    }
}
