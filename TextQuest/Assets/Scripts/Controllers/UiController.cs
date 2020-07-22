﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : Singleton<UiController>
{
    [Header("Панели")]
    [SerializeField] private GameObject narrativePanel;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject phonePanel;

    [Space]
    [Header("Основные элементы UI")]
    [SerializeField] private Text narrativeText;
    [SerializeField] private GameObject bgCharacterName;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private MyButton[] myButtoms;
    [SerializeField] private Text[] textsButtons;

    [Space]
    [Header("Дополнительные элементы UI")]
    [SerializeField] private GameObject pauseButton;

    [Space]
    [Header("Phone элементы")]
    [SerializeField] private SpriteRenderer messageSpriteRen;
    [SerializeField] private Sprite[] messageSprites;
    [SerializeField] private GameObject alertMessage;


    private Animator _animatorPanelNarrative;
    private float timeChange = .5f;

    private void Start()
    {
        _animatorPanelNarrative = narrativePanel.GetComponent<Animator>();
    }

    public void ShowNarrativeText(string text, Character character, bool mainCharacter)
    {
        narrativePanel.SetActive(false);
        
        characterImage.sprite = character.Sprite;
        characterNameText.text = character.Name;
        narrativeText.text = text;

        if (mainCharacter)
            ChangePositionCharacter(false);
        else
            ChangePositionCharacter(true);

        narrativePanel.SetActive(true);
        StartCoroutine(CoWaitShowUi());
    }

    private void ChangePositionCharacter(bool right)
    {
        if (right)
        {
            bgCharacterName.transform.localPosition = new Vector2(115, 205);
            characterImage.transform.localPosition = new Vector2(250,450);
            bgCharacterName.transform.localEulerAngles = new Vector2(0, 0);
            characterNameText.transform.localEulerAngles = new Vector3(0, 0);
        }
        else
        {
            bgCharacterName.transform.localPosition = new Vector2(-115, 205);
            characterImage.transform.localPosition = new Vector2(-250, 450);
            bgCharacterName.transform.localEulerAngles = new Vector2(0, 180);
            characterNameText.transform.localEulerAngles = new Vector3(0, 180);
        }
    }

    public IEnumerator ChangeNarrativeText(string text)
    {
        _animatorPanelNarrative.SetTrigger("hideText");
        yield return new WaitForSeconds(timeChange);
        narrativeText.text = text;
        _animatorPanelNarrative.SetTrigger("showText");
        StartCoroutine(CoWaitShowUi());
    }

    public void ShowQuestionText(List<string> variants)
    {
        questionPanel.SetActive(true);
        for (int i = 0; i < variants.Count; i++)
        {
            textsButtons[i].text = variants[i];
            myButtoms[i].gameObject.SetActive(true);
        }

        StartCoroutine(CoWaitShowUi());
    }

    public void HideQuestions()
    {
        questionPanel.SetActive(false);
        foreach (var question in myButtoms)
        {
            question.gameObject.SetActive(false);
        }
    }

    public void ShowPhoneMessage()
    {
        narrativePanel.SetActive(false);
        HideQuestions();

        alertMessage.SetActive(true);
    }

    public void ClickMessageButton()
    {
        if (GameController.Instance.PhoneMessage)
            GameController.Instance.SetPhoneQuestion();
    }

    public void ShowPhonePanel()
    {
        
    }

    public void PlayGame()
    {
        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
        MainController.Instance.PlayGame();
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
        MainController.Instance.PauseGame();
    }

    public void ExitGame()
    {
        MainController.Instance.ExitGame();
    }

    private IEnumerator CoWaitShowUi()
    {
        yield return new WaitForSeconds(.5f);
        GameController.Instance.UiReady = true;
    }
}
