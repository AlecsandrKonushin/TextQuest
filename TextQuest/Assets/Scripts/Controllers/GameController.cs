using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    private int _counterNarrative;
    private int _counterQuestions = 0;
    private int _counterParts = 0;

    private bool _endScene;
    public bool UiReady = true;

    private Dictionary<string, bool> InfluenceForGame = new Dictionary<string, bool>();
    private bool[] _narrativeFirstScene = new bool[8] { true, true, true, true, false, true, true, true };
    private bool _nowNarrative = true;

    [SerializeField] private Character[] _characters;
    private Question[] _currentQuestions = null;
    private Character _mainCharacter;
    private string _prevSpeakingName;
    private PartGame _currentPart;

    private void Start()
    {
        _currentPart = DataTexts.FirstPart;
        _mainCharacter = _characters[0];
    }

    public void NextPoint()
    {
        if (!UiReady)
            return;

        if (_endScene)
        {
            GoToNextScene();
            ResetValueCounterScene();
            return;
        }

        _nowNarrative = _narrativeFirstScene[_counterParts];
        UiReady = false;

        if (_narrativeFirstScene[_counterParts])
            SetNextNarrative();
        else
            SetNextQuestion();

        _counterParts++;
        if (_counterParts >= _narrativeFirstScene.Length)
            _endScene = true;
    }

    private void SetNextNarrative()
    {
        FrameNarrative frame = _currentPart.Narrative[_counterNarrative];
        SetNextTextAndSprite(frame);
        _counterNarrative++;
    }

    private void SetNextQuestion()
    {
        UiController uiCon = UiController.Instance;
        uiCon.HideQuestions();
        TapController.Instance.CanTap = false;
        FrameQuestion frame = _currentPart.Question[_counterQuestions];
        _currentQuestions = frame.Questions;
        SetNextTextAndSprite(frame);
        SetNextQuestion(frame);
        _counterQuestions++;
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

    private void ResetValueCounterScene()
    {
        _counterParts = 0;
        _counterNarrative = 0;
        _counterQuestions = 0;
        _endScene = false;
    }

    private void GoToNextScene()
    {

    }

    public void ChooseButton(int numberButton)
    {
        if (!UiReady)
            return;

        Question question = _currentQuestions[numberButton];

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

        if (_currentPart.Narrative[_counterNarrative] is FrameAnswer)
        {
            FrameAnswer frame = _currentPart.Narrative[_counterNarrative] as FrameAnswer;
            frame.Text = frame.Answers[numberButton];
            frame.StateCharacter = frame.States[numberButton];
        }

        UiController.Instance.HideQuestions();
        TapController.Instance.CanTap = true;

        NextPoint();
    }
}

