using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string title;
    public int numberOfCategory;
    public int numberOfQuestionTier;
    public List<QuestionPerCategory> questionList;

    public GameData(int numberOfCategory, int numberOfQuestionTier)
    {
        this.numberOfCategory = numberOfCategory;
        this.numberOfQuestionTier = numberOfQuestionTier;

        questionList = new List<QuestionPerCategory>();

        for (int i = 0; i < numberOfCategory; i++)
        {
            InitializeCategory();
        }
    }

    public string GetTitle() { return title; }

    public void EditTitle(string title) { this.title = title; }
    public int GetNumberOfCategory() { return numberOfCategory; }
    public int GetNumberOfQuestionTier() {  return numberOfQuestionTier; }

    public string GetCategoryByQuestionIndex(int questionIndex)
    {
        return questionList[questionIndex % numberOfCategory].category;
    }

    public void EditCategoryByCategoryIndex(int categoryIndex, string newCategory)
    {
        questionList[categoryIndex].category = newCategory;
    }

    public void AddNewCategory()
    {
        InitializeCategory();
        numberOfCategory++;
    }

    public void RemoveCategoryByQuestionIndex(int questionIndex)
    {
        if (numberOfCategory == 0)
            return;
        questionList.RemoveAt(questionIndex % numberOfCategory);
        numberOfCategory--;
    }

    public string GetQuestion(int questionIndex)
    {
        return questionList[questionIndex % numberOfCategory].qna[questionIndex / numberOfCategory].question;
    }

    public string GetAnswer(int questionIndex)
    {
        return questionList[questionIndex % numberOfCategory].qna[questionIndex / numberOfCategory].answer;
    }

    public void EditQuestionAndAnswer(int questionIndex, string question, string answer)
    {
        questionList[questionIndex % numberOfCategory].qna[questionIndex / numberOfCategory].question = question;
        questionList[questionIndex % numberOfCategory].qna[questionIndex / numberOfCategory].answer = answer;
    }

    public void AddQuestionTier()
    {
        for (int i = 0; i < numberOfCategory; i++)
        {
            questionList[i].qna.Add(new QuestionAndAnswer());
        }
        numberOfQuestionTier++;
    }

    public void RemoveQuestionTierByQuestionIndex(int questionIndex)
    {
        if (numberOfCategory == 0 || numberOfQuestionTier == 0)
            return;

        int tierIndex = questionIndex / numberOfCategory;

        for (int i = 0; i < numberOfCategory; i++)
        {
            questionList[i].qna.RemoveAt(tierIndex);
        }

        numberOfQuestionTier--;
    }

    private void InitializeCategory()
    {
        QuestionPerCategory category = new QuestionPerCategory("New Category");
        questionList.Add(category);
        for (int j = 0; j < numberOfQuestionTier; j++)
            category.qna.Add(new QuestionAndAnswer());
    }

    public int CalculatePointByQuestionIndex(int questionIndex)
    {
        return 100 * (questionIndex / numberOfCategory + 1);
    }

    [System.Serializable]
    public class QuestionPerCategory
    {
        public string category;
        public List<QuestionAndAnswer> qna;

        public QuestionPerCategory()
        {
            qna = new List<QuestionAndAnswer>();
        }

        public QuestionPerCategory(string category)
        {
            this.category = category;
            qna = new List<QuestionAndAnswer>();
        }

        public QuestionPerCategory(string category, List<QuestionAndAnswer> qna)
        {
            this.category = category;
            this.qna = qna;
        }
    }

    [System.Serializable]
    public class QuestionAndAnswer
    {
        public string question;
        public string answer;

        public QuestionAndAnswer()
        {
        }

        public QuestionAndAnswer(string question, string answer)
        {
            this.question = question;
            this.answer = answer;
        }
    }

}
