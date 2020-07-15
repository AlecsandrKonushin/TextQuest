using System.Collections;
using System.Collections.Generic;

public static class DataTexts
{
    public static Character[] Characters = new Character[2] { new Character("Jake"), new Character("Hanna") };

    public static PartGame FirstPart = new PartGame(1,
        new FrameNarrative[5] {
            new FrameNarrative(Characters[0], CharacterState.Smile, "Это был обычный летний день, когда почти все экзамены сданы, и можно немного расслабиться на перемене."),
            new FrameNarrative(Characters[0], CharacterState.Smile, "Я Джейк Сандерс, а это мои одноклассники и по совместительству лучшие друзья: Ханна, Лилла и Мэтью."),
            new FrameNarrative(Characters[0], CharacterState.Smile, "Мы достаточно разные, но нам хорошо вместе и очень весело."),
            new FrameNarrative(Characters[1], CharacterState.Smile, "Джейк! Ты чего опять задумался?"),
            new FrameNarrative(Characters[1], CharacterState.Smile, "Пилик-пилик (здесь должно быть изображение телефона)")
        },
        new FrameQuestion[1] {
            new FrameQuestion(Characters[0], CharacterState.Smile, "А? Да я засмотрелся на этот одуванчик... он напоминает мне..." ,
                new Question[3]
                {
                    new Question( "Лужайку у дома, в котором мы жили летом у бабушки.", Characters[1].Name, 2, "treeChange", false ),
                    new Question( "Как всё в этом мире быстротечно", Characters[1].Name, 1, "treeChange", true),
                    new Question( "ТВОЮ ПРИЧЁСКУ, когда ты неудачно подстриглась в прошлом году!", Characters[1].Name, 0 ),
                })
        });
}


public class PartGame
{
    public int NumberPart;
    public FrameNarrative[] Narrative;
    public FrameQuestion[] Question;

    public PartGame(int numberPart, FrameNarrative[] narrative, FrameQuestion[] question)
    {
        NumberPart = numberPart;
        Narrative = narrative;
        Question = question;
    }
}

public class FrameNarrative
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
}

public class FrameQuestion : FrameNarrative
{
    public Question[] Questions;

    public FrameQuestion(Character character, CharacterState state, string text, Question[] questions) : base(character, state, text)
    {
        Questions = questions;
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
        if(influenceForGame != null)
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
