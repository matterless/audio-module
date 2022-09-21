using UnityEngine;

namespace Matterless.Audio
{
    [System.Serializable]
    public class Envelope
    {
        [SerializeField] AnimationCurve m_Curve;
        [SerializeField] float m_Duration;

        internal float EvaluateCurve(float t)
        {
            if (t < 0 || t > 1)
            {
                Debug.LogWarning("Audio envelope curves can not evaluate values out of [0..1]. The value auto clamped.");
                t = Mathf.Clamp01(t);
            }

            return m_Curve.Evaluate(t);
        }

        internal float duration => m_Duration;

        internal bool hasEnvelope => m_Duration > float.Epsilon;
    }
}