using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Matterless.Audio;

public class AudioSampleScript : MonoBehaviour
{
    [SerializeField] private Button m_StartButton;
    [SerializeField] private Button m_StopButton;
    [SerializeField] private AudioListener m_AudioListener;
    [SerializeField] private AudioDatabase m_AudioDatabase;

    private AudioService m_AudioService;
    private AudioView m_AudioView;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioView = new AudioView(m_AudioListener.gameObject);
        m_AudioService = new AudioService(m_AudioView, m_AudioDatabase, m_AudioListener, true);
        m_StartButton.onClick.AddListener(Play);
        m_StopButton.onClick.AddListener(Stop);
    }

    private void Stop()
    {
        m_AudioService.StopAudio("SampleAudio");
    }

    private void Play()
    {
        m_AudioService.Play("SampleAudio");
    }
}
