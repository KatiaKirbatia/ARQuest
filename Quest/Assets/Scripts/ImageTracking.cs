using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracking : MonoBehaviour
{
   [SerializeField] private GameObject[] _prefabs;
   [SerializeField] private TextMeshProUGUI _debugText;
   private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();
   private ARTrackedImageManager _trackedImageManager;
   private List<string> _submittedNames = new List<string>();
   private int _markersCount;

   public int MarkersCount => _markersCount;
   public bool StartImageTracking;
   public event Action TrackingDisabled;
   public Action<string> ImageScanned;
   private string cachedImageName;
   private ARTrackedImage cachedImage;
   public event Action<ARTrackedImage> targetTransform;

   public void UpdateSubmittedQuestions(string name)
   {
      _submittedNames.Add(name);
      if (!_spawnedPrefabs.TryGetValue(name, out var prefab))
      {
         return;
      }

      Destroy(prefab.gameObject);
      _spawnedPrefabs.Remove(name);
      cachedImage = null;
   }

   public void ClearSubmittedNames()
   {
      _submittedNames.Clear();
      InitPrefabs();
   }

   private void Awake()
   {
      _trackedImageManager = GetComponent<ARTrackedImageManager>();
      _markersCount = _trackedImageManager.referenceLibrary.count;
      InitPrefabs();
   }

   private void InitPrefabs()
   {
      foreach (var prefab in _prefabs)
      {
         if (_spawnedPrefabs.ContainsKey(prefab.name))
         {
            continue;
         }

         var newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
         newPrefab.name = prefab.name;
         newPrefab.gameObject.SetActive(false);
         _spawnedPrefabs.Add(newPrefab.name, newPrefab);
      }
   }

   private void OnEnable()
   {
      _trackedImageManager.trackedImagesChanged += ImageChanged;
   }

   private void ImageChanged(ARTrackedImagesChangedEventArgs obj)
   {
      if (StartImageTracking == false)
      {
         return;
      }

      foreach (var image in obj.added)
      {
         _debugText.text = $"Image added: {image.referenceImage.name}";
         UpdateImage(image);
      }

      foreach (var image in obj.updated)
      {
         _debugText.text = $"Image updated: {image.referenceImage.name}";

         UpdateImage(image);
      }

      foreach (var image in obj.removed)
      {
         _debugText.text = $"Prefab removed: {_spawnedPrefabs[image.referenceImage.name].name}";

         _spawnedPrefabs[image.name].gameObject.SetActive(false);
      }
   }

   private void UpdateImage(ARTrackedImage image)
   {
      var name = image.referenceImage.name;
      if (!_spawnedPrefabs.TryGetValue(name, out var prefab))
      {
        // return;
      }

      if (cachedImage != null && image != cachedImage && cachedImage.trackingState == TrackingState.Tracking)
      {
         return;
      }

      string submittedName = _submittedNames.FirstOrDefault(list => list == name);
      if (submittedName != null)
      {
         return;
      }

      if (cachedImage == image)
      {
         Debug.LogWarning(cachedImage.trackingState);
         if (cachedImage.trackingState == TrackingState.Limited)
         {
            if (prefab != null) prefab.SetActive(false);
            TrackingDisabled?.Invoke();
            cachedImage = null;
            return;
         }

         return;
      }

      if (_prefabs != null && image.trackingState == TrackingState.Tracking && name != cachedImageName && prefab != null)
      {
         _debugText.text = $"Prefab active: {prefab.name}";
         prefab.SetActive(true);
         prefab.transform.position = image.transform.position;
         prefab.transform.localEulerAngles = image.transform.localEulerAngles;
         foreach (var item in _spawnedPrefabs.Values)
         {
            if (item.name != name)
            {
               item.SetActive(false);
            }
         }

         targetTransform?.Invoke(image);
         cachedImage = image;
         cachedImageName = name;
      }

      ImageScanned?.Invoke(image.referenceImage.name);
   }

   private void OnDisable()
   {
      _trackedImageManager.trackedImagesChanged -= ImageChanged;
   }
}