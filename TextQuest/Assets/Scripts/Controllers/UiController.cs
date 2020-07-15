using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : Singleton<UiController>
{
    [SerializeField] private GameObject narrativePanel;
    [SerializeField] private GameObject questionPanel;

    [SerializeField] private Text narrativeText;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private MyButton[] myButtoms;
    [SerializeField] private Text[] textsButtons;

    public void ShowNarrativeText(string text, Character character)
    {
        ShowNarrativePanel();
        characterNameText.text = character.Name;
        characterImage.sprite = character.Sprite;
        narrativeText.text = text;
        narrativeText.gameObject.SetActive(true);
    }

    public void ShowQuestionText(List<string> variants) 
    {
        questionPanel.SetActive(true);
        for (int i = 0; i < variants.Count; i++)
        {
            textsButtons[i].text = variants[i];
            myButtoms[i].gameObject.SetActive(true);
        }
    }

    public void HideQuestions()
    {
        questionPanel.SetActive(false);
        foreach (var question in myButtoms)
        {
            question.gameObject.SetActive(false);
        }
    }

    public void HideNarrativeText()
    {
        narrativeText.gameObject.SetActive(false);
    }

    private void ShowNarrativePanel()
    {
        narrativePanel.SetActive(true);
    }
}
