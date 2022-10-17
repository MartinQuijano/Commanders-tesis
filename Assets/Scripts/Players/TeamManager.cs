using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<CharacterInfo> characters;

    void Awake()
    {
        characters = new List<CharacterInfo>();
    }

    public void AddCharacter(CharacterInfo character)
    {
        characters.Add(character);
        character.SetMyTeam(this);
    }

    public void RemoveCharacter(CharacterInfo character)
    {
        characters.Remove(character);
    }

    public void RefreshCharacters()
    {
        foreach (CharacterInfo character in characters)
        {
            character.moved = false;
            character.attacked = false;
            character.selected = false;
            character.ChangeSpriteToActive();
        }
    }

    public void MakeAllCharacterUnactive()
    {
        foreach (CharacterInfo character in characters)
        {
            character.ChangeSpriteToUnactive();
        }
    }

    public void MakeAllCharacterActive()
    {
        foreach (CharacterInfo character in characters)
        {
            character.ChangeSpriteToActive();
        }
    }

    public void ChangeIsFromCurrentPlayingTeamFromCharacters()
    {
        foreach (CharacterInfo character in characters)
        {
            character.SwapIsFromCurrentPlayingTeam();
        }
    }
}