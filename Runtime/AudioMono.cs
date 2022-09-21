using System;
using UnityEngine;

namespace Matterless.Audio
{
    public class AudioMono : MonoBehaviour
    {
        internal event Action onUpdate;

        internal void Update() => onUpdate?.Invoke();
    }
}