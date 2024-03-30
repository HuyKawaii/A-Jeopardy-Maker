using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionButton : MonoBehaviour
{
    public bool isAnswered;
    public int index
    { private set; get; }

    public int score
    { private set; get; }
    public string question
    { private set; get; }

    public string answer
    { private set; get; }


    void Start()
    {
        isAnswered = false;
    }

    public void AssignScore(int score)
    {
        this.score = score;
        GetComponentInChildren<TextMeshProUGUI>().text = score.ToString();
    }

    public void AssignAnswer(string answer)
    {
        this.answer = answer;
    }

    public void AssignQuestion(string question)
    {
        this.question = question;
    }

    public void AssignIndex(int index)
    {
        this.index = index;
    }
}
