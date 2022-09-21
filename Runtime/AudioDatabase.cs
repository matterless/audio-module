using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Matterless.Audio
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "Matterless/Audio/Database", order = 1)]
    public class AudioDatabase : ScriptableObject
    {
        #region Inspector
        [SerializeField] private AudioEvent[] m_AudioEvents;
        [SerializeField] private AudioMixerSnapshot[] m_AudioMixerSnapshots;
        #endregion

        private Dictionary<string, AudioEvent> m_AudioDictionary;
        private Dictionary<string, AudioMixerSnapshot> m_AudioMixerSnapshotDictionary;

        internal void Init(bool isDevelop, Action<string, Action<AudioClip>> getAudioClip)
        {
            m_AudioDictionary = new Dictionary<string, AudioEvent>();

            foreach (var audioEvent in m_AudioEvents)
            {
                if (m_AudioDictionary.ContainsKey(audioEvent.id))
                    throw new Exception("Duplicate Audio Database entry: " + audioEvent.id);

                m_AudioDictionary.Add(audioEvent.id, audioEvent);

                if(isDevelop)
                {
                    for(int i = 1; i <= audioEvent.audioClip.Count; i++)
                    {
                        getAudioClip(audioEvent.id + "_" + i,
                            (clip) =>
                            {
                                // replace audio clip if it is not null
                                if (clip != null)
                                    audioEvent.audioClip[i-1] = clip;
                            });
                    }
                }
            }

            m_AudioMixerSnapshotDictionary = new Dictionary<string, AudioMixerSnapshot>();

            foreach(var snaphot in m_AudioMixerSnapshots)
            {
                if (m_AudioMixerSnapshotDictionary.ContainsKey(snaphot.name))
                    throw new Exception("Duplicate Audio Mixer Snapshot entry: " + snaphot.name);

                m_AudioMixerSnapshotDictionary.Add(snaphot.name, snaphot);
            }

        }

        internal AudioEvent GetAudioEvent(string id)
        {
            if (m_AudioDictionary.ContainsKey(id))
                return m_AudioDictionary[id];

            return null;
        }

        internal AudioMixerSnapshot GetAudioMixerSnapshot(string name)
        {
            if (m_AudioMixerSnapshotDictionary.ContainsKey(name))
                return m_AudioMixerSnapshotDictionary[name];

            return null;
        }

        public string[] GetEventIds => m_AudioDictionary.Keys.ToArray();
        public string[] GetSnapshotsIds => m_AudioMixerSnapshotDictionary.Keys.ToArray();

        private void OnValidate()
        {
            // check for duplicate ids
            for (int i = 0; i < m_AudioEvents.Length; i++)
            {
                for (int j = i + 1; j < m_AudioEvents.Length; j++)
                {
                    if (m_AudioEvents[i].id == m_AudioEvents[j].id)
                    {
                        Debug.LogError($"Audio event name duplication {m_AudioEvents[i].id}");
                        m_AudioEvents[j] = null;
                    }
                }
            }
        }

    }
}
