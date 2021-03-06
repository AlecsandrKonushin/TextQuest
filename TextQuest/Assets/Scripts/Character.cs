﻿using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string Name;
    public CharacterState State;
    public Sprite Sprite;
    public Sprite SmileSprite;
    public Sprite HateSprite;
    public Sprite ConfusedSprite;

    private Dictionary<string, int> Communications = new Dictionary<string, int>();
    
    public Character(string name)
    {
        Name = name;
    }

    public void ChangeState(CharacterState state)
    {
        if (state == CharacterState.Smile)
            Sprite = SmileSprite;
        else if (state == CharacterState.Hate)
            Sprite = HateSprite;
        else if (state == CharacterState.Confused)
            Sprite = ConfusedSprite;
    }

    public void ChangeCommunication(string characterName, int interaction)
    {
        if (Communications == null)
            Communications = new Dictionary<string, int>();

        if (Communications.Count !=0 && Communications.ContainsKey(characterName))
            Communications[characterName] += interaction;
        else
            Communications.Add(characterName, interaction);


        foreach (var comm in Communications)
        {
            Debug.Log("Связь с " + comm.Key + " = " + comm.Value);

        }
    }
}
