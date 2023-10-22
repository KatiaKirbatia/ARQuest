using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackedImageInfo : MonoBehaviour
{
   [SerializeField] private ARTrackedImageManager _trackedImageManager;
   [SerializeField] private TextMeshProUGUI _text;

   void OnEnable()
   { 
      _trackedImageManager.trackedImagesChanged += OnChanged;
   } 

   void OnDisable()
   {
      _trackedImageManager.trackedImagesChanged -= OnChanged;
   }

   private void OnButtonClicked()
   {
      ListAllImages();
   }

   void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
   {
      foreach (var newImage in eventArgs.added)
      {
         _text.text = $"Image added " + newImage.referenceImage.name;
      }

      foreach (var updatedImage in eventArgs.updated)
      {
         _text.text = "Image Updated " + updatedImage.referenceImage.name;
      }

      foreach (var removedImage in eventArgs.removed)
      {
         _text.text = "Image Removed " + removedImage.referenceImage.name;
      }
   }
   
   void ListAllImages()
   {
      Debug.Log(
         $"There are {_trackedImageManager.trackables.count} images being tracked.");
      _text.text = $"There are {_trackedImageManager.trackables.count} images being tracked.";

      foreach (var trackedImage in _trackedImageManager.trackables)
      {
         Debug.Log($"Image: {trackedImage.referenceImage.name} is at " +
                   $"{trackedImage.transform.position}");
      }
   }

   private void Update()
   {
      if (Input.GetKeyUp(KeyCode.Space))
      {
         ListAllImages();
      }
   }
}
