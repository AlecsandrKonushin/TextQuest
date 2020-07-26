using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : Singleton<FrameController>
{
    public bool UiReady = true;
    public Character[] Characters;
    public Character MainCharacter { get; private set; }
    public Character SpeakingCharacter;
    public Character PrevSpeakingCharacter;
    public FrameGame CurrentFrame;
    public FrameQuestion CurrentQuestion;
    public bool PositionSpriteleft;
    public bool ReloadNarrative;

    private PartGame _currentPart;
    private int _counterFrames;
    private bool _endScene;
    private Dictionary<string, bool> InfluenceForGame = new Dictionary<string, bool>();

    public bool PhoneMessage { get; private set; }

    private void Start()
    {
        _currentPart = DataTexts.Instance.FirstPart;
        MainCharacter = Characters[0];
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
        ReloadNarrative = false;

        CurrentFrame = _currentPart.Frames[_counterFrames];
        if (_counterFrames > 0 && _currentPart.Frames[_counterFrames - 1].GetType() != _currentPart.Frames[_counterFrames].GetType())
            StartCoroutine(CoHidePrevFrame());
        else
            CurrentFrame.SetData();

        _counterFrames++;
        if (_counterFrames >= _currentPart.Frames.Length)
            _endScene = true;
    }

    private IEnumerator CoHidePrevFrame()
    {
        _currentPart.Frames[_counterFrames - 1].HideData();
        yield return new WaitForSeconds(.5f);
        CurrentFrame.SetData();
    }

    //private void PhoneMessageNow()
    //{
    //    PhoneMessage = true;
    //    UiController.Instance.ShowPhoneAlert();
    //}

    //public void SetPhoneQuestion()
    //{
    //    FramePhone frame = _currentPart.Frames[_counterFrames] as FramePhone;
    //    UiController.Instance.ShowPhonePanel(frame.Messages, frame.NameSender);
    //    SetNextQuestion(frame);
    //}

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

        Question question = CurrentQuestion.Questions[numberButton];

        if (question.InfluencedCharacterName != null)
        {
            MainCharacter.ChangeCommunication(
                question.InfluencedCharacterName,
                question.InfluenceForCharacter);
        }

        if (question.InfluenceForGame != null)
        {
            InfluenceForGame.Add(question.InfluenceForGame, question.ValueInfluenceForGame);
        }

        if (_currentPart.Frames[_counterFrames] is FrameAnswer)
        {
            FrameAnswer frame = _currentPart.Frames[_counterFrames] as FrameAnswer;
            frame.Text = frame.Answers[numberButton];
            frame.StateCharacter = frame.States[numberButton];
        }

        UiController.Instance.HideQuestions();
        TapController.Instance.CanTap = true;

        NextPoint();
    }

}
