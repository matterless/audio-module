using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Matterless.Audio
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "Matterless/Audio/Database", order = 1)]
    public class AudioDatabase : ScriptableObject
    {
        #region Inspector
        [SerializeField] private AudioEvent[] m_AudioEvents;
        #endregion

        private Dictionary<string, AudioEvent> m_AudioDictionary;
        private readonly Random m_Random = new Random();

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
        }

        internal AudioEvent GetAudioEvent(string id)
        {
            if (m_AudioDictionary.ContainsKey(id))
                return m_AudioDictionary[id];

            return null;
        }

        internal List<AudioClip> GetShuffledPlaylist(AudioEvent audioEvent)
        {
            List<AudioClip> shuffledList = audioEvent.audioClip;
            int n = shuffledList.Count;  
            while (n > 1) {  
                n--;  
                int k = m_Random.Next(n + 1);  
                (shuffledList[k], shuffledList[n]) = (shuffledList[n], shuffledList[k]);
            }
            return shuffledList;
        }
    }
}
