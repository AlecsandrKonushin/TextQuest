using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : Singleton<UiController>
{
    [Header("Панели")]
    [SerializeField] private GameObject narrativePanel;
    [SerializeField] private GameObject narrativeQuestionPanel;
    [SerializeField] private GameObject questionsPanel;
    [SerializeField] private GameObject phonePanel;
    [SerializeField] private GameObject phoneAnswerPanel;
    [SerializeField] private GameObject pausePanel;

    [Space]
    [Header("Narrative элементы")]
    [SerializeField] private Image bgNarrativePanel;
    [SerializeField] private Text narrativeText;
    
    [Space]
    [Header("Character элементы")]
    [SerializeField] private GameObject bgCharacterName;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Image characterImage;

    [Space]
    [Header("Questions элементы")]
    [SerializeField] private Image bgQuestionPanel;
    [SerializeField] private Text questionText;
    [SerializeField] private AnswerButton[] answerButtons;
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
    [SerializeField] private GameObject answerMessage;
    [SerializeField] private Text textAnswerMessage;

    [Space]
    [Header("Позиции")]
    [SerializeField] private Vector2 rightBgName;
    [SerializeField] private Vector2 leftBgName;
    [SerializeField] private Vector2 rightCharacterImage;
    [SerializeField] private Vector2 leftCharacterImage;


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
        yield return new WaitForSeconds(.25f);
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
            bgCharacterName.transform.localPosition = rightBgName;
            characterImage.transform.localPosition = rightCharacterImage;
            bgCharacterName.transform.localEulerAngles = new Vector2(0, 0);
            characterNameText.transform.localEulerAngles = new Vector3(0, 0);
        }
        else
        {
            bgCharacterName.transform.localPosition = leftBgName;
            characterImage.transform.localPosition = leftCharacterImage;
            bgCharacterName.transform.localEulerAngles = new Vector2(0, 180);
            characterNameText.transform.localEulerAngles = new Vector3(0, 180);
        }
    }
    #endregion

    #region Question panel
    public void ShowQuestionPanel(string text)
    {
        questionText.text = text;

        narrativeQuestionPanel.SetActive(true);
        questionText.gameObject.SetActive(true);
    }
    
    public void HideQuestionPanel()
    {
        StartCoroutine(CoHideQuestionPanel());
    }

    private IEnumerator CoHideQuestionPanel()
    {
        narrativeQuestionPanel.GetComponent<Animator>().SetTrigger("hide");
        questionsPanel.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(timeHide);
        narrativeQuestionPanel.SetActive(false);
        questionsPanel.SetActive(false);

        foreach (var text in textsButtons)
        {
            text.color = Color.white;
        }
        foreach (var button in answerButtons)
        {
            button.gameObject.GetComponent<Image>().color = Color.white;
        }
    }

    public void ShowQuestionText(List<string> variants)
    {
        questionsPanel.SetActive(true);

        for (int i = 0; i < variants.Count; i++)
        {
            textsButtons[i].text = variants[i];
            answerButtons[i].gameObject.SetActive(true);
        }
    }

    public void HideQuestions()
    {
        narrativeQuestionPanel.SetActive(false);
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

    public void DefaultSpriteButtonMessage()
    {
        messageButtonSprite.sprite = spritesMessage[0];
    }

    public void ClickButtonMessage()
    {
        panelNewMessage.SetActive(false);
        messageButtonSprite.sprite = spritesMessage[2];
    }
    #endregion

    #region Phone question
    public void ShowPhonePanel(string[] messages, string nameSender, List<string> answers)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            this.messages[i].SetActive(true);
            messagesTexts[i].text = messages[i];
            nameSenderTexts[i].text = nameSender;
        }

        for (int i = 0; i < answers.Count; i++)
        {
            answerButtons[i].gameObject.SetActive(true);
            textsAnswerButtons[i].text = answers[i];
        }

        StartCoroutine(CoShowPhonePanel());
    }

    public IEnumerator CoShowPhonePanel()
    {
        phonePanel.SetActive(true);
        yield return new WaitForSeconds(1.15f);
        phoneAnswerPanel.SetActive(true);
    }

    public void ChooseAnswerMessage(string textAnswer)
    {
        StartCoroutine(CoWaitShowUi(1f));
        textAnswerMessage.text = textAnswer;
        StartCoroutine(CoHidePhoneAnswers());
    }

    private IEnumerator CoHidePhoneAnswers()
    {
        phoneAnswerPanel.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(timeHide);
        phoneAnswerPanel.SetActive(false);
        answerMessage.SetActive(true);
        TapController.Instance.CanTap = true;
    }

    public void HidePhone()
    {
        StartCoroutine(CoHidePhone());
    }

    public IEnumerator CoHidePhone()
    {
        answerMessage.GetComponent<Animator>().SetTrigger("hide");
        phonePanel.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(1);
        phonePanel.SetActive(false);
        answerMessage.SetActive(false);
        DefaultSpriteButtonMessage();
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
