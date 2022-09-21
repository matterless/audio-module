using UnityEngine;
using UnityEngine.UI;
using Matterless.Audio;

public class AudioSampleScript : MonoBehaviour
{
    [SerializeField] private AudioDatabase m_AudioDatabase;
    [SerializeField] private Transform m_EventParent;
    [SerializeField] private Transform m_SnapshotParent;
    [SerializeField] private Button m_Template;

    void Awake()
    {
        // add audio listener
        this.gameObject.AddComponent<AudioListener>();
        // install audio service
        var audioService = new AudioService(m_AudioDatabase, false);
        // generate buttons based on database audio events
        foreach (var item in m_AudioDatabase.GetEventIds)
        {
            new EventButtonUi(m_EventParent, m_Template, audioService, item);
        }
        // generate buttons based on database snaphots
        foreach (var item in m_AudioDatabase.GetSnapshotsIds)
        {
            new SnapshotButtonUi(m_SnapshotParent, m_Template, audioService, item);
        }

        m_Template.gameObject.SetActive(false);
    }
}
