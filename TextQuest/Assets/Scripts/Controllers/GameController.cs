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

        if (_currentPart.Narrative[_counterFrames] is FrameQuestion)
            SetNextQuestion();
        else if (_currentPart.Narrative[_counterFrames] is FramePhone)
            PhoneMessageNow();
        else
            SetNextNarrative();
        _counterFrames++;

        if (_counterFrames >= _currentPart.Narrative.Length)
            _endScene = true;
    }

    private void SetNextNarrative()
    {
        FrameNarrative frame = _currentPart.Narrative[_counterFrames];
        SetNextTextAndSprite(frame);
    }

    private void SetNextQuestion()
    {
        UiController uiCon = UiController.Instance;
        uiCon.HideQuestions();
        TapController.Instance.CanTap = false;
        FrameQuestion frame = _currentPart.Narrative[_counterFrames] as FrameQuestion;
        _currentQuestion = frame;
        SetNextTextAndSprite(frame);
        SetNextQuestion(frame);
    }

    private void SetNextTextAndSprite(FrameNarrative frame)
    {
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

        if (nowCharacter.Name == _prevSpeakingName)
            StartCoroutine(UiController.Instance.ChangeNarrativeText(frame.Text));
        else
        {
            bool mainCharacter = false;
            if (nowCharacter == _mainCharacter)
                mainCharacter = true;
            UiController.Instance.ShowNarrativeText(frame.Text, nowCharacter, mainCharacter);
        }

        _prevSpeakingName = frame.Character.Name;
    }

    private void SetNextQuestion(FrameQuestion frame)
    {
        List<string> variants = new List<string>();
        foreach (var question in frame.Questions)
        {
            variants.Add(question.TextQuestion);
        }
        UiController.Instance.ShowQuestionText(variants);
    }

    private void PhoneMessageNow()
    {
        PhoneMessage = true;
        UiController.Instance.ShowPhoneMessage();
    }

    public void SetPhoneQuestion()
    {
        // Задаёт сообщение на телефоне, затем вызывает его отображение через uiCon
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

