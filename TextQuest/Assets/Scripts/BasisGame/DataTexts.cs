using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTexts
{
    public static Character[] Characters = new Character[4] { new Character("Jake"), new Character("Hanna"), new Character("Matthew"), new Character("Lila") };

    public static PartGame FirstPart = new PartGame(1,
        new FrameGame[10] {
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
            new FramePhone(Characters[1], CharacterState.Smile, "",
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

        UiController uiCon = UiController.Instance;
        GameController gameCon = GameController.Instance;

        if (gameCon.PrevSpeakingCharacter == null)
            uiCon.ShowNarrativePanel(Text);
        else
            uiCon.ChangeNarrativeText(Text);

        gameCon.PrevSpeakingCharacter = gameCon.SpeakingCharacter;
    }

    protected void CheckSpeakingCharacter()
    {
        GameController gameCon = GameController.Instance;
        gameCon.ReloadNarrative = false;

        gameCon.SpeakingCharacter = null;
        foreach (var character in gameCon.Characters)
        {
            if (character.Name == Character.Name)
            {
                gameCon.SpeakingCharacter = character;
                continue;
            }
        }
        gameCon.SpeakingCharacter.ChangeState(StateCharacter);

        if (gameCon.PrevSpeakingCharacter != null && gameCon.SpeakingCharacter.Name != gameCon.PrevSpeakingCharacter.Name)
            gameCon.ReloadNarrative = true;

        gameCon.PositionSpriteleft = false;
        if (gameCon.SpeakingCharacter == gameCon.MainCharacter)
            gameCon.PositionSpriteleft = true;

        Character nowCharacter = gameCon.SpeakingCharacter;
        UiController.Instance.ShowSpeakingCharacter(nowCharacter, gameCon.PositionSpriteleft, gameCon.ReloadNarrative);
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

public class FramePhone : FrameQuestion
{
    public string[] Messages;
    public string NameSender;

    public FramePhone(Character character, CharacterState state, string text, Question[] questions, string[] messages, string nameSender) : base(character, state, text, questions)
    {
        Messages = messages;
        NameSender = nameSender;
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
