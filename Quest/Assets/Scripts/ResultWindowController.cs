using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class ResultWindowController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _totaltext;
    [SerializeField] private Transform _container;
    [SerializeField] private ResultItem _resultItem;
    [SerializeField] private Button _retakeButton;

    private int countCorrect;
    private List<ResultItem> _resultItems = new List<ResultItem>();

    public event Action RetakeClicked;

    public void Init(QuestionsDataManager questionData, Dictionary<string, int> submittedQuestions, int total)
    {
        LoadQuestions(questionData, submittedQuestions);
        _totaltext.text = $"You have {countCorrect} out of {total}!!!";
    }

    private void OnEnable()
    {
        _retakeButton.onClick.AddListener(OnRetakeClicked);
    }
    
    private void OnDisable()
    {
        foreach (var item in _resultItems)
        {
            Destroy(item.gameObject);
        }
        _resultItems.Clear();
        _retakeButton.onClick.RemoveListener(OnRetakeClicked);
    }

    private void OnRetakeClicked()
    {
        RetakeClicked?.Invoke();
    }

    private void LoadQuestions(QuestionsDataManager questionData, Dictionary<string, int> submittedQuestions)
    {
        foreach (var question in submittedQuestions)
        {
            bool isCorrectAnswer = false;
            var name = question.Key;
            var index = question.Value;
            var targetQuestion = questionData.GetQuestionData(name);
            if(index == 2)
            {
                countCorrect++;
                isCorrectAnswer = true;
            }

            var item = Instantiate(_resultItem, _container);
            item.Init(name, targetQuestion[2], targetQuestion[index], isCorrectAnswer);
            _resultItems.Add(item);
        }
    }
}