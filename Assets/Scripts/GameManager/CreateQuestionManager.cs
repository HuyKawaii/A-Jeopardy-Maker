using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreateQuestionManager : MonoBehaviour
{
    private GameData gameData;
    public static CreateQuestionManager Instance;
    public const int defaultNumberOfCategory = 5;
    public const int defaultNumberOfQuestionTier = 5;

    public delegate void QuestionStructureUpdateDelegate();
    public QuestionStructureUpdateDelegate onQuestionStructureUpdateCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }

        if (GameManager.Instance.GetGameData() == null) 
            gameData = new GameData(defaultNumberOfCategory, defaultNumberOfQuestionTier);
        else
            gameData = GameManager.Instance.GetGameData();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetNumberOfCategory()
    {
        return gameData.GetNumberOfCategory();
    }

    public int GetNumberOfQuestionTier()
    {
        return gameData.GetNumberOfQuestionTier();
    }

    public string GetCategoryName(int questionIndex)
    {
        return gameData.GetCategoryByQuestionIndex(questionIndex);
    }

    public void EditCategory(int index, string categoryName)
    {
        gameData.EditCategoryByCategoryIndex(index, categoryName);
    }

    public void AddCategory()
    {
        gameData.AddNewCategory();
        onQuestionStructureUpdateCallback();
    }

    public void RemoveCategory(int questionIndex)
    {
        gameData.RemoveCategoryByQuestionIndex(questionIndex);
        onQuestionStructureUpdateCallback();
    }

    public string ViewQuestion(int questionIndex)
    {
        return gameData.GetQuestion(questionIndex);
    }

    public string ViewAnswer(int questionIndex)
    {
        return gameData.GetAnswer(questionIndex);
    }

    public void EditQuestionAndAnswer(int questionIndex, string question, string answer)
    {
        gameData.EditQuestionAndAnswer(questionIndex, question, answer); 
    }

    public void AddQuestionTier()
    {
        gameData.AddQuestionTier();
        onQuestionStructureUpdateCallback();
    }

    public void RemoveQuestionTier(int questionIndex)
    {
        gameData.RemoveQuestionTierByQuestionIndex(questionIndex);
        onQuestionStructureUpdateCallback();
    }

    public string GetTitle()
    {
        return gameData.GetTitle();
    }

    public void EditTitle(string newTitle)
    {
        gameData.EditTitle(newTitle);
    }

    public int CalculatePoint(int questionIndex)
    {
        return gameData.CalculatePointByQuestionIndex(questionIndex);
    }

    public bool SaveGameData()
    {
        return SaveSystem.SaveData(gameData);
    }
}
