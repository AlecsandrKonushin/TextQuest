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
    public List<Answer> CurrentAnswers;
    public bool PositionSpriteleft;
    public bool ReloadNarrative;
    public bool NewTypeFrame;

    private PartGame _currentPart;
    private int _counterFrames;
    private bool _endScene;
    private Dictionary<string, bool> InfluenceForGame = new Dictionary<string, bool>();
    
    private void Start()
    {
        _currentPart = DataTexts.Instance.FirstPart;
        MainCharacter = Characters[0];
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

        UiReady = false;
        ReloadNarrative = false;

        CurrentFrame = _currentPart.Frames[_counterFrames];
        if (_counterFrames > 0 && _currentPart.Frames[_counterFrames - 1].GetType() != _currentPart.Frames[_counterFrames].GetType())
            StartCoroutine(CoHidePrevFrame());
        else
        {
            StartCoroutine(UiController.Instance.CoWaitShowUi(.5f));
            CurrentFrame.SetData();
        }

        _counterFrames++;
        if (_counterFrames >= _currentPart.Frames.Length)
            _endScene = true;
    }

    private IEnumerator CoHidePrevFrame()
    {
        StartCoroutine(UiController.Instance.CoWaitShowUi(1f));
        NewTypeFrame = true;
        _currentPart.Frames[_counterFrames - 1].HideData();
        yield return new WaitForSeconds(.5f);
        CurrentFrame.SetData();
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

        Answer answer = CurrentAnswers[numberButton];

        if (answer.InfluencedCharacterName != null)
        {
            MainCharacter.ChangeCommunication(
                answer.InfluencedCharacterName,
                answer.InfluenceForCharacter);
        }

        if (answer.InfluenceForGame != null)
        {
            InfluenceForGame.Add(answer.InfluenceForGame, answer.ValueInfluenceForGame);
        }

        if (_currentPart.Frames[_counterFrames - 1] is FrameQuestion)
        {

            FrameAfterAnswer frame = _currentPart.Frames[_counterFrames] as FrameAfterAnswer;
            frame.Text = frame.AfterAnswers[numberButton];
            frame.StateCharacter = frame.States[numberButton];
            UiController.Instance.HideQuestions();
            NextPoint();
        }
        else if (_currentPart.Frames[_counterFrames - 1] is FramePhone)
        {
            FramePhone frame = _currentPart.Frames[_counterFrames - 1] as FramePhone;
            UiController.Instance.ChooseAnswerMessage(frame.Answers[numberButton].TextAnswer);
        }


        TapController.Instance.CanTap = true;
    }

}
