using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImageTracking : MonoBehaviour
{

    [SerializeField] List<GameObject> prefabsToSpawn = new List<GameObject>();

    [SerializeField] private ARTrackedImageManager m_TrackedImageManager;
    [SerializeField] private ARAnchorManager m_AnchorManager;

    private Dictionary<string, GameObject> arObjects;

    private void Start()
    {
        arObjects = new Dictionary<string, GameObject>();
        SetupElements();
    }

    private void OnEnable() => m_TrackedImageManager.trackablesChanged.AddListener(OnChanged);

    private void OnDisable() => m_TrackedImageManager.trackablesChanged.RemoveListener(OnChanged);

    private void SetupElements()
    {
        foreach (var prefab in prefabsToSpawn)
        {
            var arObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            arObject.name = prefab.name;
            arObject.gameObject.SetActive(false);
            arObjects.Add(arObject.name, arObject);
            Debug.Log(arObject.name);
        }
    }

    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            UpdateTrackedImages(newImage);
            CreateAnchorAsync(newImage);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            //UpdateTrackedImages(updatedImage);
        }

        foreach (var removedImage in eventArgs.removed)
        {
            //Nada por ahora
        }
    }

    private void UpdateTrackedImages(ARTrackedImage trackedImage)
    {
        if(trackedImage == null) return;
        // if (trackedImage.trackingState is TrackingState.Limited or TrackingState.None)
        // {
        //     arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
        //     return;
        // }

        //Debug.Log(trackedImage.referenceImage.name);

        arObjects[trackedImage.referenceImage.name].gameObject.SetActive(true);
        arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
        arObjects[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;

        // if (arObjects[trackedImage.referenceImage.name].gameObject.GetComponent<ARAnchor>() == null)
        // {
        //     arObjects[trackedImage.referenceImage.name].gameObject.AddComponent<ARAnchor>();
        // }
    } 

    async void CreateAnchorAsync(ARTrackedImage trackedImage)
    {
        var pose = new Pose(trackedImage.transform.position, trackedImage.transform.rotation);
        Debug.Log(pose);

        var result = await m_AnchorManager.TryAddAnchorAsync(pose);
        if (result.status.IsSuccess())
        {
            var anchor = result.value;
            Debug.Log(anchor.GetType());
            arObjects[trackedImage.referenceImage.name].transform.SetParent(anchor.transform);
        }
    }
}
