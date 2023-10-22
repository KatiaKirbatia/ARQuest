using TMPro;
using UnityEngine;

public class ResultItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _question;
    [SerializeField] private TextMeshProUGUI _correctAnswer;
    [SerializeField] private TextMeshProUGUI _yourAnswer;
    [SerializeField] private Color _correctColour;
    [SerializeField] private Color _inCorrectColour;
    

    public void Init(string name, string correctAnswer, string playerAnswer, bool isCorrect)
    {
        _question.text = name;
        _correctAnswer.text = correctAnswer;
        _yourAnswer.text = playerAnswer;
        SetColor(isCorrect);
    }

    private void SetColor(bool isCorrect)
    {
        _question.color = isCorrect ? _correctColour : _inCorrectColour;
        _correctAnswer.color = isCorrect ? _correctColour : _inCorrectColour;
        _yourAnswer.color = isCorrect ? _correctColour : _inCorrectColour;
    }
}