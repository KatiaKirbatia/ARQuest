using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressWindowController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _congratulationText;
    [SerializeField] private Button _resultButton;

    private int _totalQuestions;

    public event Action ResultClicked;

    public void Init(int total)
    {
        _congratulationText.gameObject.SetActive(false);
        _resultButton.gameObject.SetActive(false);
        _progressText.gameObject.SetActive(true);
        _totalQuestions = total;
        _resultButton.onClick.AddListener(OnResultClicked);
    }

    public void UpdateProgress(int count)
    {
        _progressText.text = $"Progress: {count}/{_totalQuestions}";
    }

    public void ActivateCongratulationText()
    {
        _congratulationText.text = "Congratulation!";
        _congratulationText.gameObject.SetActive(true);
        _resultButton.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _resultButton.onClick.RemoveListener(OnResultClicked);
    }

    private void OnResultClicked()
    {
        ResultClicked?.Invoke();
    }
}