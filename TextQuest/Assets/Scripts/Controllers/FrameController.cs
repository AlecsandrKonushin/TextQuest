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
            StartCoroutine(UiController.Instance.CoWaitShowUi(CurrentFrame.TimeWaitForNextClick));
            CurrentFrame.SetData();
        }

        if (CurrentFrame is FrameAudio)
            StartCoroutine(CoFrameAudioHide());

        _counterFrames++;
        if (_counterFrames >= _currentPart.Frames.Length)
            _endScene = true;
    }

    private IEnumerator CoHidePrevFrame()
    {
        FrameGame frame = _currentPart.Frames[_counterFrames - 1];
        StartCoroutine(UiController.Instance.CoWaitShowUi(frame.TimeHide + CurrentFrame.TimeWaitForNextClick));
        NewTypeFrame = true;
        frame.HideData();
        yield return new WaitForSeconds(frame.TimeHide);
        CurrentFrame.SetData();
    }

    private IEnumerator CoFrameAudioHide()
    {
        yield return new WaitForSeconds(CurrentFrame.TimeHide);
        CurrentFrame.HideData();
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

        if (_currentPart.Frames[_counterFrames - 1] is FrameAnswer)
        {

            FrameAfterAnswer frame = _currentPart.Frames[_counterFrames] as FrameAfterAnswer;
            frame.Text = frame.AfterAnswers[numberButton];
            frame.StateCharacter = frame.States[numberButton];
            UiController.Instance.HideAnswers();
            NextPoint();
            TapController.Instance.CanTap = true;
        }
        else if (_currentPart.Frames[_counterFrames - 1] is FramePhone)
        {
            FramePhone frame = _currentPart.Frames[_counterFrames - 1] as FramePhone;
            UiController.Instance.ChooseAnswerMessage(frame.Answers[numberButton].TextAnswer);
        }
    }

    public void NexfFrameAfterAudio(float time)
    {
        StartCoroutine(CoNextFrameAfterAudio(time));
    }

    private IEnumerator CoNextFrameAfterAudio(float time)
    {
        yield return new WaitForSeconds(time);
        TapController.Instance.CanTap = true;
        NextPoint();
    }

}
