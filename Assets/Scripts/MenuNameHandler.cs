using TMPro;
using UnityEngine;

public class MenuNameHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    public void OnNameInputFinished()
    {
        PlayerPrefs.SetString("PlayerName", nameInputField.text);
        PlayerPrefs.Save();
    }
}