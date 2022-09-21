using System;
using UnityEngine;

namespace Matterless.Audio
{
    internal class MultiAudioInstance : AudioInstanceBase
    {
        private readonly System.Random m_Random;
        private int m_ClipCount;
        private int[] m_Indexes;

        internal MultiAudioInstance(ulong id, AudioSource audioSource,
            AudioEvent audioEvent,
            Action onAutoStopped,
            Action<IAudioInstance> onComplete) : base(id, audioSource, audioEvent, onAutoStopped, onComplete) 
        {
            // random generator
            if (m_AudioEvent.sequenceType != SequenceType.Order)
            {
                m_Random = new System.Random();
            }

            // create index array and fill with integers
            if (m_AudioEvent.sequenceType == SequenceType.Shuffle)
            {
                m_Indexes = new int[audioEvent.count];
                for (int i = 0; i < m_Indexes.Length; i++)
                    m_Indexes[i] = i;
            }
        }

        protected override void OnStart()
        {
            // if suffle, suffle indexes
            if (m_AudioEvent.sequenceType == SequenceType.Shuffle)
            {
                ShuffleIndexes(ref m_Indexes);
            }

            m_ClipCount = -1;
            PlayNext();
        }

        protected override void OnAudioClipCompleted()
        {
            if (!PlayNext())
            {
                // loop
                if (m_AudioEvent.isLoop)
                {
                    OnStart();
                }
                // auto complete
                else if(!m_ManualyStopped)
                {
                    m_OnComplete?.Invoke(this);
                    m_OnAutoStopped?.Invoke();
                }
            }
        }

        private void ShuffleIndexes(ref int[] array)
        {
            int n = array.Length;

            while (n > 1)
            {
                n--;
                int k = m_Random.Next(n + 1);
                (array[k], array[n]) = (array[n], array[k]);
            }
        }

        private int index
        {
            get
            {
                switch(m_AudioEvent.sequenceType)
                {
                    case SequenceType.Order:
                        return m_ClipCount;
                    case SequenceType.Random:
                        return m_Random.Next(m_AudioEvent.count);
                    case SequenceType.Shuffle:
                        return m_Indexes[m_ClipCount];
                }

                return m_ClipCount;
            }
        }

        private bool PlayNext()
        {
            m_ClipCount++;

            if (m_ClipCount >= m_AudioEvent.count)
                return false;

            //Debug.Log($"Play {m_AudioEvent.audioClip[index].name}");

            m_AudioSource.clip = m_AudioEvent.audioClip[index];
            m_AudioSource.Play();

            return true;
        }
    }
}