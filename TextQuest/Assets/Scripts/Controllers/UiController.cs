using System.Collections;
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
    [Header("Основные элементы Frame")]
    [SerializeField] private Image bgNarrativePanel;
    [SerializeField] private Text narrativeText;
    [SerializeField] private GameObject bgCharacterName;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private MyButton[] answerButtons;
    [SerializeField] private Text[] textsButtons;

    [Space]
    [Header("Message button элементы")]
    [SerializeField] private Image messageButtonSprite;
    [SerializeField] private Sprite[] spritesMessage;
    [SerializeField] private GameObject panelNewMessage;

    [Space]
    [Header("Phone элементы")]
    [SerializeField] private GameObject[] messages;
    [SerializeField] private Text[] messagesTexts;
    [SerializeField] private Text[] nameSenderTexts;
    [SerializeField] private MyButton[] phoneAnswerButtons;
    [SerializeField] private Text[] textsAnswerButtons;

    public bool ClickButton = false;
    private Animator _animatorPanelNarrative;
    private float timeHide = .5f;
    private Color colorTextNarrative;

    private void Start()
    {
        _animatorPanelNarrative = narrativePanel.GetComponent<Animator>();
        colorTextNarrative = narrativeText.color;
    }

    #region Narrative panel
    public void ShowNarrativePanel(string text)
    {
        narrativeText.text = text;

        narrativePanel.SetActive(true);
        narrativeText.gameObject.SetActive(true);
    }

    public void ChangeNarrativeText(string text)
    {
        StartCoroutine(CoChangeNarrativeText(text));
    }

    private IEnumerator CoChangeNarrativeText(string text)
    {
        _animatorPanelNarrative.SetTrigger("hideText");
        yield return new WaitForSeconds(timeHide);
        narrativeText.text = text;
        _animatorPanelNarrative.SetTrigger("showText");
    }

    public void HideNarrativePanel()
    {
        StartCoroutine(CoHideNarrativePanel());
    }

    private IEnumerator CoHideNarrativePanel()
    {
        narrativePanel.GetComponent<Animator>().SetTrigger("hidePanel");
        yield return new WaitForSeconds(timeHide);
        narrativePanel.SetActive(false);
        bgNarrativePanel.color = Color.white;
        narrativeText.color = colorTextNarrative;
    }
    #endregion

    #region Character speaking
    public void ShowSpeakingCharacter()
    {
        Character character = FrameController.Instance.SpeakingCharacter;
        characterImage.sprite = character.Sprite;
        characterNameText.text = character.Name;

        if (FrameController.Instance.PositionSpriteleft)
            ChangePositionCharacter(false);
        else
            ChangePositionCharacter(true);

        characterImage.gameObject.SetActive(true);
        bgCharacterName.gameObject.SetActive(true);
    }

    public void HideSpeakingCharacter(bool reload)
    {
        StartCoroutine(CoHideSpeakingCharacter(reload));
    }

    private IEnumerator CoReloadCharacterSprite()
    {
        characterImage.GetComponent<Animator>().SetTrigger("hide");
        bgCharacterName.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(timeHide);
    }

    private IEnumerator CoHideSpeakingCharacter(bool reload)
    {
        characterImage.GetComponent<Animator>().SetTrigger("hide");
        bgCharacterName.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(timeHide);
        characterImage.gameObject.SetActive(false);
        bgCharacterName.gameObject.SetActive(false);

        if (reload)
            ShowSpeakingCharacter();
    }

    private void ChangePositionCharacter(bool right)
    {
        if (right)
        {
            bgCharacterName.transform.localPosition = new Vector2(125, -190);
            characterImage.transform.localPosition = new Vector2(250, 205);
            bgCharacterName.transform.localEulerAngles = new Vector2(0, 0);
            characterNameText.transform.localEulerAngles = new Vector3(0, 0);
        }
        else
        {
            bgCharacterName.transform.localPosition = new Vector2(-130, -190);
            characterImage.transform.localPosition = new Vector2(-270, 185);
            bgCharacterName.transform.localEulerAngles = new Vector2(0, 180);
            characterNameText.transform.localEulerAngles = new Vector3(0, 180);
        }
    }
    #endregion

    #region Question panel
    public void ShowQuestionText(List<string> variants)
    {
        questionPanel.SetActive(true);
        for (int i = 0; i < variants.Count; i++)
        {
            textsButtons[i].text = variants[i];
            answerButtons[i].gameObject.SetActive(true);
        }
    }

    public void HideQuestions()
    {
        questionPanel.SetActive(false);
        foreach (var question in answerButtons)
        {
            question.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Alert message
    public void ShowAlertMesage()
    {
        panelNewMessage.SetActive(true);
        messageButtonSprite.sprite = spritesMessage[1];
    }

    public void HideAlertMessage()
    {
        StartCoroutine(CoHideAlertMessage());
    }

    private IEnumerator CoHideAlertMessage()
    {
        panelNewMessage.GetComponent<Animator>().SetTrigger("hide");
        messageButtonSprite.sprite = spritesMessage[0];
        yield return new WaitForSeconds(.5f);
        panelNewMessage.SetActive(false);
    }
    #endregion

    #region Phone question
    public void ClickAlertButton()
    {
        //if (FrameController.Instance.PhoneMessage)
        //    FrameController.Instance.SetPhoneQuestion();
    }

    public void ShowPhonePanel(string[] messages, string nameSender)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            this.messages[i].SetActive(true);
            messagesTexts[i].text = messages[i];
            nameSenderTexts[i].text = nameSender;
        }
    }
    #endregion


    public void ClickAnyButton()
    {
        ClickButton = true;
    }

    public void ExitGame()
    {
        MainController.Instance.ExitGame();
    }

    public IEnumerator CoWaitShowUi(float time)
    {
        yield return new WaitForSeconds(time);
        FrameController.Instance.UiReady = true;
    }
}
