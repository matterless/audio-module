using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace Matterless.Audio
{
    public class AudioService : IAudioService
    {
        private readonly AudioDatabase m_AudioDatabase;
        private readonly IAudioView m_AudioView;
        private AudioListener m_AudioListener;
        private AudioMono m_audioMono;

        private Dictionary<string, int> m_PlayQueues = new Dictionary<string, int>();
        private Dictionary<string, List<AudioClip>> m_PlayLists = new Dictionary<string, List<AudioClip>>();

        private Dictionary<string, Coroutine> m_RunningLoops = new Dictionary<string, Coroutine>();
        
        [Preserve]
        public AudioService(
            // dependencies
            IAudioView audioView, 
            // arguments
            AudioDatabase audioDatabase, AudioListener audioListener, bool enableLocalEditor)
        {
            if (audioView == null)
                throw new Exception("Audio view is missing.");

            if (audioDatabase == null)
                throw new Exception("Audio database is missing.");
            
            if (audioListener == null)
                throw new Exception("Audio listener is missing.");

            m_AudioDatabase = audioDatabase;
            m_AudioView = audioView;
            m_AudioListener = audioListener;
            m_audioMono = m_AudioListener.gameObject.AddComponent<AudioMono>();
            m_AudioDatabase.Init(enableLocalEditor, GetLocalDevelopAudioClip);
        }

        public AudioListener audioListener => m_AudioListener;

        public void Play(Enum id) => Play(id.ToString());
        public void StopLoop(Enum id) => StopLoop(id.ToString());

        public void Play(string id)
        {
            if (m_AudioDatabase == null)
                throw new Exception("Audio database is missing.");

            var audio = m_AudioDatabase.GetAudioEvent(id);

            if(audio == null)
            {
                Debug.LogWarning($"Audio event is missing: {id}");
                return;
            }    

            if(audio.audioClip == null)
            {
                Debug.LogWarning($"Audio event has null audio clip: {id}");
                return;
            }

            Debug.Log($"Play audio event: {id}");

            //Play directly if there is only 1 clip
            if(audio.audioClip.Count == 1){
                switch (audio.type)
                {
                    case AudioEventType.PlayOnce:
                    case AudioEventType.PlayOnceShuffled:
                        m_AudioView.PlayAudioClipOnce(audio.audioClip[0], audio.channel);
                        break;
                    case AudioEventType.Loop:
                    case AudioEventType.LoopShuffled:
                        m_AudioView.PlayAudioClipLoop(audio.id, audio.audioClip[0], audio.channel);
                        break;
                }
            }
            else //Check for playlist and get one if there is none/expired
            {
                switch (audio.type)
                {
                    case AudioEventType.PlayOnce:
                    case AudioEventType.Loop:
                        if (!m_PlayQueues.ContainsKey(id)) 
                        {
                            m_PlayQueues.Add(id,0);
                            m_PlayLists.Add(id,audio.audioClip);
                        }
                        if (m_PlayQueues[id] >= m_PlayLists[id].Count)
                        {
                            m_PlayQueues[id] = 0;
                        }
                        break;
                    case AudioEventType.PlayOnceShuffled:
                    case AudioEventType.LoopShuffled:
                        if (!m_PlayQueues.ContainsKey(id)) 
                        {
                            m_PlayQueues.Add(id,0);
                            m_PlayLists.Add(id,m_AudioDatabase.GetShuffledPlaylist(audio));
                        }
                        if (m_PlayQueues[id] >= m_PlayLists[id].Count)
                        {
                            m_PlayQueues[id] = 0;
                            m_PlayLists[id] = m_AudioDatabase.GetShuffledPlaylist(audio);
                        }
                        break;
                }
                
                //Play the playlist
                switch (audio.type)
                {
                    case AudioEventType.PlayOnce:
                    case AudioEventType.PlayOnceShuffled:
                        m_AudioView.PlayAudioClipOnce(m_PlayLists[id][m_PlayQueues[id]], audio.channel);
                        m_PlayQueues[id]++;
                        break;
                    case AudioEventType.Loop:
                    case AudioEventType.LoopShuffled:
                        if(m_RunningLoops.ContainsKey(id)){
                            m_audioMono.StopCoroutine(m_RunningLoops[id]);
                            m_AudioView.StopAudioClip(audio.channel);
                        }
                        else
                        {
                            m_RunningLoops.Add(id, null);
                        }
                        m_RunningLoops[id] = m_audioMono.StartCoroutine(PlaylistLoop(id, audio, audio.channel));
                        break;
                }
            }
        }

        public void StopAudio(string id)
        {
            var audio = m_AudioDatabase.GetAudioEvent(id);
            m_AudioView.StopAudioClip(audio.channel);
        }

        public void StopLoop(string id)
        {
            if (m_RunningLoops.ContainsKey(id))
            {
                m_audioMono.StopCoroutine(m_RunningLoops[id]);
                m_AudioView.StopAudioClip(m_AudioDatabase.GetAudioEvent(id).channel);
            }
            else
            {
                m_AudioView.StopAudioClipLoop(id);
            }
        }

        IEnumerator PlaylistLoop(string id, AudioEvent audio, AudioMixerGroup channel)
        {
            while (true)
            {
                if (m_PlayQueues[id] >= m_PlayLists[id].Count)
                {
                    m_PlayQueues[id] = 0;
                    m_PlayLists[id] = m_AudioDatabase.GetShuffledPlaylist(audio);
                }

                var clip = m_PlayLists[id][m_PlayQueues[id]];
                m_AudioView.PlayAudioClipOnce(clip, channel);
                m_PlayQueues[id]++;
                yield return new WaitForSeconds(clip.length);
            }
        }

        #region Developement Environment
        private void GetLocalDevelopAudioClip(string id, Action<AudioClip> callback)
        {
            m_audioMono.StartCoroutine(GetLocalDevelopAudioClipAsync(id, callback));
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
