using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfflinePlayerUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nameInputField;

    private int playerScore;
    [SerializeField]
    private Button increaseScore;
    [SerializeField]
    private Button decreaseScore;
    [SerializeField]
    private TMP_InputField scoreInputField;

    private void Start()
    {
        AssignButton();
    }

    private void AssignButton()
    {
        nameInputField.onEndEdit.AddListener(OnNameInputFieldEdit);
        increaseScore.onClick.AddListener(OnIncreaseScoreButtonClick);
        decreaseScore.onClick.AddListener(OnDecreaseScoreButtonClick);
        scoreInputField.onEndEdit.AddListener(OnScoreInputFieldEdit);
    }

    private void OnNameInputFieldEdit(string name)
    {
        nameInputField.text = name;
    }

    private void OnIncreaseScoreButtonClick()
    {
        UpdateScore(playerScore + GameManager.Instance.questionValue);
    }

    private void OnDecreaseScoreButtonClick()
    {
        UpdateScore(playerScore - GameManager.Instance.questionValue);
    }

    private void OnScoreInputFieldEdit(string stringScore)
    {
        scoreInputField.text = stringScore;
    }

    private void UpdateScore(int newScore)
    {
        playerScore = newScore;
        scoreInputField.text = newScore.ToString();
    }

    public void InitializeUI(string name)
    {
        nameInputField.text = name;
        playerScore = 0;
    }
}
