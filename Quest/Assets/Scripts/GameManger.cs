using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameManger : MonoBehaviour
{
   [SerializeField] private Button _startButton;
   [SerializeField] private QuestionManager _questionsPanel;
   [SerializeField] private ImageTracking _imageTracking;
   [SerializeField] private ProgressWindowController _progressController;
   [SerializeField] private ResultWindowController _resultController;

   private QuestionsDataManager _questionsData;
   private int _totalQuestions;
   private int _totalMarkers;
   private Dictionary<string, int> _submittedQuestions = new Dictionary<string, int>();
   private List<List<string>> _listOfQuestions = new List<List<string>>();

   public event Action QuizzEnded;

   private void Awake()
   {
      _questionsData = new QuestionsDataManager();
      _questionsData.InitQuestions();
      _listOfQuestions = _questionsData.AllQuestions;
      _totalQuestions = _listOfQuestions.Count;
      _totalMarkers = _imageTracking.MarkersCount;
      if (_totalQuestions != _totalMarkers)
      {
         Debug.LogWarning("Number of images is not the same as a number of questions");
      }
   }

   private void OnEnable()
   {
      UpLoadPlayerData();
      foreach (var question in _submittedQuestions)
      {
         _imageTracking.UpdateSubmittedQuestions(question.Key); 
      }
      
      _startButton.onClick.AddListener(OnStartClicked);
      _imageTracking.targetTransform += OnTransformChanged;
      _imageTracking.TrackingDisabled += OnTrackingDisabled;
      _questionsPanel.AnswerSubmitted += OnAnswerSubmitted;
      _progressController.ResultClicked += OnResultClicked;
      _resultController.RetakeClicked += OnRetakeClicked;
   }

   private void OnDisable()
   {
      SavePlayerData();
      
      _activeQuestion.Clear();
      
      _startButton.onClick.RemoveListener(OnStartClicked);
      _imageTracking.targetTransform -= OnTransformChanged;
      _imageTracking.TrackingDisabled -= OnTrackingDisabled;
      _questionsPanel.AnswerSubmitted -= OnAnswerSubmitted;
      _progressController.ResultClicked -= OnResultClicked;
      _resultController.RetakeClicked -= OnRetakeClicked;
   }

   private void OnRetakeClicked()
   {
      _resultController.gameObject.SetActive(false);
      PlayerPrefs.DeleteAll();
      PlayerPrefs.Save();
      _submittedQuestions.Clear();
      _imageTracking.ClearSubmittedNames();
      StartOver(true);
   }

   private void OnResultClicked()
   {
      _progressController.gameObject.SetActive(false);
      _resultController.gameObject.SetActive(true);
      LoadResults();
   }

   private void LoadResults()
   {
      _resultController.Init(_questionsData, _submittedQuestions, _totalMarkers);
   }

   private void OnAnswerSubmitted(int index)
   {
      var name = _activeQuestion[0];
      _imageTracking.UpdateSubmittedQuestions(name);
      _submittedQuestions.Add(name, index);
      var count = _submittedQuestions.Count;
      _progressController.UpdateProgress(count);

      if (count == _totalMarkers)
      {
         SavePlayerData();
         _imageTracking.StartImageTracking = false;
         _progressController.ActivateCongratulationText();
         QuizzEnded?.Invoke();
      }
   }

   private void UpLoadPlayerData()
   {
      foreach (var data in _questionsData.AllQuestions)
      {
         var name = data[0];
         int index = PlayerPrefs.GetInt(name);
         if (index != 0)
         {
            _submittedQuestions.Add(name, index);
         }
      }
   }

   private void SavePlayerData()
   {
      foreach (var question in _submittedQuestions)
      {
         PlayerPrefs.SetInt(question.Key, question.Value);
      }
   }
   
   private void OnTrackingDisabled()
   {
      _questionsPanel.gameObject.SetActive(false);
   }

   private List<string> _activeQuestion = new List<string>();
   private void OnTransformChanged(ARTrackedImage trackedImage)
   {
      _questionsPanel.gameObject.SetActive(true);
      _questionsPanel.transform.position = trackedImage.transform.position + new Vector3(0,0.2f, 0.05f);
      _activeQuestion = _questionsData.GetQuestionData(trackedImage.referenceImage.name);
      _questionsPanel.UpdateQuestions(_activeQuestion);
   }

   private void OnStartClicked()
   {
      StartOver(false);
      _progressController.gameObject.SetActive(true);
      _progressController.Init(_totalMarkers);
      _progressController.UpdateProgress(_submittedQuestions.Count);
   }

   private void StartOver(bool isActive)
   {
      _startButton.gameObject.SetActive(isActive);
      _imageTracking.StartImageTracking = !isActive;
   }
}