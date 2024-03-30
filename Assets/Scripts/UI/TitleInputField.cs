using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleInputField : MonoBehaviour
{
   TMP_InputField inputField;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(OnEditComplete);
    }

    private void OnEditComplete(string input)
    {
        CreateQuestionManager.Instance.EditTitle(input);
    }
}
