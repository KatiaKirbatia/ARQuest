using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Transform _container;
    [SerializeField] private ToggleItem _toggleItem;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Button _submitButton;

    private List<ToggleItem> _items = new List<ToggleItem>();
    private List<string> _questionsList = new List<string>();
    private string _titleName;
    private bool _canUpdate;

    public event Action<int> AnswerSubmitted; 

    public void UpdateQuestions(List<string> list)
    {
        _canUpdate = list[0] != _titleName;
        if (_canUpdate)
        {
            _questionsList = list;
            UnloadQuestions();
            LoadQuestions(_questionsList);  
        }
        ShuffleLayoutElements();
    }

    private void Start()
    {
        _submitButton.onClick.AddListener(OnSubmitted);
    }

    private void OnDestroy()
    {
        _submitButton.onClick.RemoveListener(OnSubmitted);
        UnloadQuestions();
    }

    private void OnSubmitted()
    {
        var selectedItem = _items.FirstOrDefault(x => x.IsSelected);
        if (selectedItem != null)
        {
            AnswerSubmitted?.Invoke(selectedItem.Index);
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        var lookDirection = _mainCamera.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-lookDirection, Vector3.up);
    }
    
    private void LoadQuestions(List<string> list)
    {
        _titleName = list[0];
        _title.text = list[1];
   
        for (int i = 2; i <= list.Count - 1; i++)
        {
            ToggleItem item = Instantiate(_toggleItem, _container);
            item.Init(list[i], i == 2, i);
            item.ButtonClicked += OnAnswerSelected;
            _items.Add(item);
        }
    }

    private void UnloadQuestions()
    {
        foreach (var item in _items)
        {
            item.ButtonClicked -= OnAnswerSelected;
            Destroy(item.gameObject);
        }
        _items.Clear();
    }
    
    private void ShuffleLayoutElements()
    {
        int elementCount = _items.Count;
        System.Random rng = new System.Random();

        for (int i = 0; i < elementCount - 1; i++)
        {
            int randomIndex = rng.Next(i, elementCount);
            (_items[randomIndex], _items[i]) = (_items[i], _items[randomIndex]);
        }

        for (int i = 0; i < elementCount; i++)
        {
            _items[i].transform.SetSiblingIndex(i);
        }
    }

    private void OnAnswerSelected(ToggleItem toggleItem)
    {
        foreach (var toggle in _items)
        {
            toggle.IsSelected = toggle == toggleItem;
        }
    }
}