using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    private int _counterFrames;

    private bool _endScene;
    public bool UiReady = true;

    private Dictionary<string, bool> InfluenceForGame = new Dictionary<string, bool>();

    [SerializeField] private Character[] _characters;
    private FrameQuestion _currentQuestion = null;
    private Character _mainCharacter;
    private string _prevSpeakingName;
    private PartGame _currentPart;
    public bool PhoneMessage { get; private set; }

    private void Start()
    {
        _currentPart = DataTexts.FirstPart;
        _mainCharacter = _characters[0];
    }

    public void NextPoint()
    {
        if (!UiReady || PhoneMessage)
            return;

        if (_endScene)
        {
            GoToNextScene();
            ResetValueCounterScene();
            return;
        }

        UiReady = false;

        FrameNarrative frame = _currentPart.Narrative[_counterFrames];
        bool reloadNarrative = CheckSpeakingCharacter(frame);

        if (frame is FramePhone)
            PhoneMessageNow();
        else if (frame is FrameQuestion)
        {
            SetNextNarrative(frame, reloadNarrative);
            SetNextQuestion(frame as FrameQuestion);
        }
        else
            SetNextNarrative(frame, reloadNarrative);

        _counterFrames++;
        if (_counterFrames >= _currentPart.Narrative.Length)
            _endScene = true;
    }

    private bool CheckSpeakingCharacter(FrameNarrative frame)
    {
        bool reloab = false;
        Character nowCharacter = null;
        foreach (var character in _characters)
        {
            if (character.Name == frame.Character.Name)
            {
                nowCharacter = character;
                continue;
            }
        }
        nowCharacter.ChangeState(frame.StateCharacter);

        if (_prevSpeakingName != null && nowCharacter.Name != _prevSpeakingName)        
            reloab = true;        

        bool mainCharacter = false;
        if (nowCharacter == _mainCharacter)
            mainCharacter = true;

        StartCoroutine(UiController.Instance.ShowSpeakingCharacter(nowCharacter.Sprite, nowCharacter.Name, mainCharacter, reloab));

        return reloab;
    }

    private void SetNextNarrative(FrameNarrative frame, bool reloadNarrative)
    {
        UiController uiCon = UiController.Instance;

        if (_prevSpeakingName == frame.Character.Name)
            StartCoroutine(uiCon.ChangeNarrativeText(frame.Text));
        else if(_prevSpeakingName == null)
            uiCon.ShowNarrativePanel(frame.Text);
        else        
            StartCoroutine(uiCon.ChangeNarrativeText(frame.Text));        

        _prevSpeakingName = frame.Character.Name;
    }

    private void SetNextQuestion(FrameQuestion frame)
    {
        TapController.Instance.CanTap = false;
        _currentQuestion = frame;

        List<string> variants = new List<string>();
        foreach (var question in frame.Questions)
        {
            variants.Add(question.TextQuestion);
        }
        UiController.Instance.ShowQuestionText(variants, PhoneMessage);
    }

    private void PhoneMessageNow()
    {
        PhoneMessage = true;
        UiController.Instance.ShowPhoneAlert();
    }

    public void SetPhoneQuestion()
    {
        FramePhone frame = _currentPart.Narrative[_counterFrames] as FramePhone;
        UiController.Instance.ShowPhonePanel(frame.Messages, frame.NameSender);
        SetNextQuestion(frame);
    }

    private void ResetValueCounterScene()
    {
        _counterFrames = 0;
        _endScene = false;
    }

    private void GoToNextScene()
    {

    }

    public void ChooseButton(int numberButton)
    {
        if (!UiReady)
            return;

        Question question = _currentQuestion.Questions[numberButton];

        if (question.InfluencedCharacterName != null)
        {
            _mainCharacter.ChangeCommunication(
                question.InfluencedCharacterName,
                question.InfluenceForCharacter);
        }

        if (question.InfluenceForGame != null)
        {
            InfluenceForGame.Add(question.InfluenceForGame, question.ValueInfluenceForGame);
        }

        if (_currentPart.Narrative[_counterFrames] is FrameAnswer)
        {
            FrameAnswer frame = _currentPart.Narrative[_counterFrames] as FrameAnswer;
            frame.Text = frame.Answers[numberButton];
            frame.StateCharacter = frame.States[numberButton];
        }

        UiController.Instance.HideQuestions();
        TapController.Instance.CanTap = true;

        NextPoint();
    }
}

