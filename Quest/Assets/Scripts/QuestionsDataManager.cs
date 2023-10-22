using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestionsDataManager
{
    private List<List<string>> _questions = new List<List<string>>();
    
    public List<List<string>> AllQuestions => _questions;

    public void InitQuestions()
    {
        var csvFile = Resources.Load<TextAsset>("questions");

        if (!csvFile)
        {
            return;
        }
        _questions = CSVParser.LoadFromString(csvFile.text);
           
        Resources.UnloadAsset(csvFile);
    }
    
    public List<string> GetQuestionData(string imageName)
    {
        List<string> secondList = _questions.FirstOrDefault(list => list.FirstOrDefault() == imageName);
        
        return secondList ?? null;
    }
}