using UnityEngine;
using UnityEngine.UI;
using Matterless.Audio;
public class SnapshotButtonUi
{
    public SnapshotButtonUi(Transform parent, Button template, IAudioService audioService, string snapshotName)
    {
        var button = GameObject.Instantiate(template, parent);
        button.GetComponentInChildren<Text>().text = $"transition to {snapshotName}";
        button.onClick.AddListener(()=>
        {
            Debug.Log($"Transitioning to {snapshotName} for 1s");
            audioService.TransitionTo(snapshotName, 1f);
        });
    }
}
