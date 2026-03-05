using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ScriptImageTracking : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager m_TrackedImageManager;
    [SerializeField] private GameObject trackedImagePrefab;
    GameObject sol;

    [SerializeField] private TextMeshProUGUI texto;

    private void OnEnable() => m_TrackedImageManager.trackablesChanged.AddListener(OnChanged);

    private void OnDisable() => m_TrackedImageManager.trackablesChanged.RemoveListener(OnChanged);

    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            sol = Instantiate(trackedImagePrefab, newImage.transform);
            texto.text = "Encontrado!";
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            sol.transform.SetPositionAndRotation(updatedImage.pose.position, updatedImage.pose.rotation);
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed images if necessary
        }
    }

}
