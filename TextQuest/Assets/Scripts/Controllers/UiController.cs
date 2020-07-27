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
    [Header("Phone элементы")]
    [SerializeField] private SpriteRenderer messageButtonSpriteRen;
    [SerializeField] private Sprite[] messageButtonSprites;
    [SerializeField] private GameObject alertMessage;
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

        StartCoroutine(CoWaitShowUi());
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
        StartCoroutine(CoWaitShowUi());
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
    public void ShowSpeakingCharacter(Character character, bool mainCharacter, bool reload)
    {
        StartCoroutine(CoShowSpeakingCharacter(character.Sprite, character.Name, mainCharacter, reload));
    }

    public IEnumerator CoShowSpeakingCharacter(Sprite spriteCharacter, string nameCharacter, bool mainCharacter, bool reload)
    {
        if (reload)
        {
            StartCoroutine(CoHideSpeakingCharacter());
            yield return new WaitForSeconds(timeHide);
        }

        characterImage.sprite = spriteCharacter;
        characterNameText.text = nameCharacter;

        if (mainCharacter)
            ChangePositionCharacter(false);
        else
            ChangePositionCharacter(true);

        characterImage.gameObject.SetActive(true);
        bgCharacterName.gameObject.SetActive(true);
    }

    public void HideSpeakingCharacter()
    {
        StartCoroutine(CoHideSpeakingCharacter());
    }

    private IEnumerator CoHideSpeakingCharacter()
    {
        characterImage.GetComponent<Animator>().SetTrigger("hide");
        bgCharacterName.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(timeHide);
        characterImage.gameObject.SetActive(false);
        bgCharacterName.gameObject.SetActive(false);
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

        StartCoroutine(CoWaitShowUi());
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

    public void ShowPhonePanel()
    {
    }


    public void ShowPhoneAlert()
    {
        HideQuestions();

        alertMessage.SetActive(true);
    }

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

    public void ClickAnyButton()
    {
        ClickButton = true;
    }

    public void ExitGame()
    {
        MainController.Instance.ExitGame();
    }

    private IEnumerator CoWaitShowUi()
    {
        yield return new WaitForSeconds(.5f);
        FrameController.Instance.UiReady = true;
    }
}
