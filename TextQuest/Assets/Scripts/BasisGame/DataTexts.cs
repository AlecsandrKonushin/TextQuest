using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTexts : Singleton<DataTexts>
{
    public Character[] Characters;
    public PartGame FirstPart;

    public void Start()
    {
        Characters = FrameController.Instance.Characters;
        CreateFirstPart();
    }

    private void CreateFirstPart()
    {
        FirstPart = new PartGame(1,
            new FrameGame[11] {

            new FrameNarrative(Characters[0], CharacterState.Smile, "Это был обычный летний день, когда почти все экзамены сданы, и можно немного расслабиться на перемене."),
            new FrameNarrative(Characters[0], CharacterState.Smile, "Я Джейк Сандерс, а это мои одноклассники и по совместительству лучшие друзья: Ханна, Лилла и Мэтью."),
            new FrameNarrative(Characters[0], CharacterState.Smile, "Мы достаточно разные, но нам хорошо вместе и очень весело."),
            new FrameNarrative(Characters[1], CharacterState.Smile, "Джейк! Ты чего опять задумался?"),
             new FrameQuestion(Characters[0], CharacterState.Smile, "А? Да я засмотрелся на этот одуванчик... он напоминает мне..." ,
                new List<Answer>
                {
                    new Answer( "Лужайку у дома, в котором мы жили летом у бабушки.", Characters[1].Name, 2, "treeChange", false ),
                    new Answer( "Как всё в этом мире быстротечно", Characters[1].Name, 1, "treeChange", true),
                    new Answer( "ТВОЮ ПРИЧЁСКУ, когда ты неудачно подстриглась в прошлом году!", Characters[1].Name, 0 ),
                }),
            new FrameAfterAnswer(Characters[1], CharacterState.Smile, "",
                new string[3]{ "Я помню ее, она была очень доброй…", "Джейк, смотри на мир позитивнее.", "Ну ты гад! Это было ужасно!"},
                new CharacterState[3]{ CharacterState.Smile, CharacterState.Smile, CharacterState.Hate}),
            new FrameNarrative(Characters[0], CharacterState.Smile, "Ладно, какой у нас следующий урок?"),
            new FrameNarrative(Characters[2], CharacterState.Smile, "Вроде физика."),
            new FrameNarrative(Characters[3], CharacterState.Smile, "Ээхх…. Еще есть 15 минут на свободе…"),
            new FrameAlertMessage(),
            new FramePhone(
                new string[3]{
                    "Джейк, я сегодня взяла дополнительную смену в ночь,",
                    "еда в холодильнике,",
                    "не засиживайся допоздна за компьютером, целую."},
                Characters[4].name,
                new List<Answer>
                {
                    new Answer("Ок", Characters[4].name, 0),
                    new Answer("Хорошо, легкой тебе смены.", Characters[4].name, 1),
                    new Answer("Спасибо мам, увидимся.", Characters[4].name, 2)
                })


            });
    }
}

public class PartGame
{
    public int NumberPart;
    public FrameGame[] Frames;

    public PartGame(int numberPart, FrameGame[] frames)
    {
        NumberPart = numberPart;
        Frames = frames;
    }
}

public abstract class FrameGame : MonoBehaviour
{
    public abstract void SetData();
    public abstract void HideData();
}

public class FrameNarrative : FrameGame
{
    public Character Character;
    public CharacterState StateCharacter = CharacterState.Smile;
    public string Text;

    public FrameNarrative(Character character, CharacterState state, string text)
    {
        Character = character;
        StateCharacter = state;
        Text = text;
    }

    public override void SetData()
    {
        CheckSpeakingCharacter();
        ShowPanel();

        FrameController frameCon = FrameController.Instance;

        frameCon.PrevSpeakingCharacter = frameCon.SpeakingCharacter;
        frameCon.NewTypeFrame = false;
    }

    public override void HideData()
    {
        UiController uiCon = UiController.Instance;
        uiCon.HideNarrativePanel();
        uiCon.HideSpeakingCharacter(false);
    }

    protected virtual void ShowPanel()
    {
        UiController uiCon = UiController.Instance;
        FrameController frameCon = FrameController.Instance;

        if (frameCon.PrevSpeakingCharacter == null || frameCon.NewTypeFrame)
            uiCon.ShowNarrativePanel(Text);
        else
            uiCon.ChangeNarrativeText(Text);
    }

    protected void CheckSpeakingCharacter()
    {
        FrameController frameCon = FrameController.Instance;
        frameCon.ReloadNarrative = false;

        frameCon.SpeakingCharacter = null;
        foreach (var character in frameCon.Characters)
        {
            if (character.Name == Character.Name)
            {
                frameCon.SpeakingCharacter = character;
                continue;
            }
        }
        frameCon.SpeakingCharacter.ChangeState(StateCharacter);

        if (frameCon.PrevSpeakingCharacter != null && frameCon.SpeakingCharacter.Name != frameCon.PrevSpeakingCharacter.Name)
            frameCon.ReloadNarrative = true;

        frameCon.PositionSpriteleft = false;
        if (frameCon.SpeakingCharacter == frameCon.MainCharacter)
            frameCon.PositionSpriteleft = true;

        Character nowCharacter = frameCon.SpeakingCharacter;
        if (frameCon.ReloadNarrative && !frameCon.NewTypeFrame)
            UiController.Instance.HideSpeakingCharacter(true);
        else
            UiController.Instance.ShowSpeakingCharacter();
    }
}

public class FrameQuestion : FrameNarrative
{
    public List<Answer> Answers;

    public FrameQuestion(Character character, CharacterState state, string text, List<Answer> answers) : base(character, state, text)
    {
        Answers = answers;
    }

    public override void SetData()
    {
        base.SetData();
        SetNextQuestion();
    }

    public override void HideData()
    {
        UiController uiCon = UiController.Instance;
        uiCon.HideSpeakingCharacter(false);
        uiCon.HideQuestionPanel();
        uiCon.HideQuestions();
    }

    private void SetNextQuestion()
    {
        TapController.Instance.CanTap = false;
        FrameController.Instance.CurrentAnswers = Answers;

        List<string> variants = new List<string>();
        foreach (var question in Answers)
        {
            variants.Add(question.TextAnswer);
        }
        UiController.Instance.ShowQuestionText(variants);
    }

    protected override void ShowPanel()
    {
        UiController.Instance.ShowQuestionPanel(Text);
    }
}

public class FrameAfterAnswer : FrameNarrative
{
    public string[] AfterAnswers;
    public CharacterState[] States;

    public FrameAfterAnswer(Character character, CharacterState state, string text, string[] afterAnswers, CharacterState[] states) : base(character, state, text)
    {
        AfterAnswers = afterAnswers;
        States = states;
    }
}

public class FrameAlertMessage : FrameGame
{
    public override void SetData()
    {
        TapController.Instance.CanTap = false;
        UiController.Instance.ShowAlertMesage();
    }

    public override void HideData()
    {
    }
}

public class FramePhone : FrameGame
{
    public string[] Messages;
    public string NameSender;
    public List<Answer> Answers;

    public FramePhone(string[] messages, string nameSender, List<Answer> answers)
    {
        Messages = messages;
        NameSender = nameSender;
        Answers = answers;
    }
    
    public override void SetData()
    {
        List<string> textsAnswers = new List<string>();
        foreach (var answer in Answers)
        {
            textsAnswers.Add(answer.TextAnswer);
        }

        UiController.Instance.ShowPhonePanel(Messages, NameSender, textsAnswers);
    }

    public override void HideData()
    {
    }
}

public class Answer
{
    public string TextAnswer;
    public string InfluencedCharacterName;
    public int InfluenceForCharacter;
    public string InfluenceForGame;
    public bool ValueInfluenceForGame;

    public Answer(string text, string influencedCharacterName = null, int influenceForCharacter = 0, string influenceForGame = null, bool valueInfluenceForGame = false)
    {
        TextAnswer = text;
        InfluencedCharacterName = influencedCharacterName;
        InfluenceForCharacter = influenceForCharacter;
        if (influenceForGame != null)
        {
            InfluenceForGame = influenceForGame;
            ValueInfluenceForGame = valueInfluenceForGame;
        }
    }
}

public enum CharacterState
{
    Smile,
    Hate
}
