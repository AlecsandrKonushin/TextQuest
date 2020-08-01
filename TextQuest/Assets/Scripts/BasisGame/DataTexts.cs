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
                new Question[3]
                {
                    new Question( "Лужайку у дома, в котором мы жили летом у бабушки.", Characters[1].Name, 2, "treeChange", false ),
                    new Question( "Как всё в этом мире быстротечно", Characters[1].Name, 1, "treeChange", true),
                    new Question( "ТВОЮ ПРИЧЁСКУ, когда ты неудачно подстриглась в прошлом году!", Characters[1].Name, 0 ),
                }),
            new FrameAnswer(Characters[1], CharacterState.Smile, "",
                new string[3]{ "Я помню ее, она была очень доброй…", "Джейк, смотри на мир позитивнее.", "Ну ты гад! Это было ужасно!"},
                new CharacterState[3]{ CharacterState.Smile, CharacterState.Smile, CharacterState.Hate}),
            new FrameNarrative(Characters[0], CharacterState.Smile, "Ладно, какой у нас следующий урок?"),
            new FrameNarrative(Characters[2], CharacterState.Smile, "Вроде физика."),
            new FrameNarrative(Characters[3], CharacterState.Smile, "Ээхх…. Еще есть 15 минут на свободе…"),
            new FrameAlertMessage(),
            new FramePhone(Characters[4], CharacterState.Smile, "",
                new Question[3]
                {
                    new Question("Ок."),
                    new Question("Хорошо, легкой тебе смены."),
                    new Question("Спасибо мам, увидимся."),
                },
                new string[3]
                {   "Джейк, я сегодня взяла дополнительную смену в ночь,",
                    "еда в холодильнике,",
                    "не засиживайся допоздна за компьютером, целую."
                },
                "Мама")
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
    public Question[] Questions;

    public FrameQuestion(Character character, CharacterState state, string text, Question[] questions) : base(character, state, text)
    {
        Questions = questions;
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
        FrameController.Instance.CurrentQuestion = this;

        List<string> variants = new List<string>();
        foreach (var question in Questions)
        {
            variants.Add(question.TextQuestion);
        }
        UiController.Instance.ShowQuestionText(variants);
    }

    protected override void ShowPanel()
    {
        UiController.Instance.ShowQuestionPanel(Text);
    }
}

public class FrameAnswer : FrameNarrative
{
    public string[] Answers;
    public CharacterState[] States;

    public FrameAnswer(Character character, CharacterState state, string text, string[] answers, CharacterState[] states) : base(character, state, text)
    {
        Answers = answers;
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
        TapController.Instance.CanTap = true;
    }
}

public class FramePhone : FrameQuestion
{
    public string[] Messages;
    public string NameSender;

    public FramePhone(Character character, CharacterState state, string text, Question[] questions, string[] messages, string nameSender) : base(character, state, text, questions)
    {
        Messages = messages;
        NameSender = nameSender;
    }

    public override void SetData()
    {
        Debug.Log("phone quest");
    }
}

public class Question
{
    public string TextQuestion;
    public string InfluencedCharacterName;
    public int InfluenceForCharacter;
    public string InfluenceForGame;
    public bool ValueInfluenceForGame;

    public Question(string text, string influencedCharacterName = null, int influenceForCharacter = 0, string influenceForGame = null, bool valueInfluenceForGame = false)
    {
        TextQuestion = text;
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
