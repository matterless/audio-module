using UnityEngine;
using UnityEngine.UI;
using Matterless.Audio;
public class EventButtonUi
{
    private readonly IAudioService m_AudioService;
    private readonly Text m_Text;
    private readonly string m_EventId;
    private bool m_IsPlaying;
    private ulong m_CurrentInstanceId;

    public EventButtonUi(Transform parent, Button template, IAudioService audioService, string eventId)
    {
        var button = GameObject.Instantiate(template, parent);
        button.onClick.AddListener(OnButtonClicked);
        m_AudioService = audioService;
        m_Text = button.GetComponentInChildren<Text>();
        m_EventId = eventId;
        Reset();
    }

    void Play()
    {
        m_IsPlaying = true;
        m_CurrentInstanceId = m_AudioService.Play(m_EventId, Reset);
        m_Text.text = $"Stop {m_EventId} : {m_CurrentInstanceId}";
        Debug.Log($"Playing instance {m_CurrentInstanceId}");
    }

    void Reset()
    {
        //Debug.Log("Reset");
        m_IsPlaying = false;
        m_Text.text = $"Play {m_EventId}";
    }

    void Stop()
    {
        m_IsPlaying = false;
        Debug.Log($"Stopping instance {m_CurrentInstanceId}");
        m_AudioService.Stop(m_CurrentInstanceId);
        m_Text.text = $"Play {m_EventId}";
    }

    void OnButtonClicked()
    {
        if (m_IsPlaying)
            Stop();
        else
            Play();
    }
}