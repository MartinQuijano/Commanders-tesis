using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<Unit> characters;

    void Awake()
    {
        characters = new List<Unit>();
    }

    public void AddCharacter(Unit character)
    {
        characters.Add(character);
        character.SetMyTeam(this);
    }

    public void RemoveCharacter(Unit character)
    {
        characters.Remove(character);
    }

    public void RefreshCharacters()
    {
        foreach (Unit character in characters)
        {
            character.moved = false;
            character.attacked = false;
            character.selected = false;
            character.ChangeSpriteToActive();
        }
    }

    public void MakeAllCharacterUnactive()
    {
        foreach (Unit character in characters)
        {
            character.ChangeSpriteToUnactive();
        }
    }

    public void MakeAllCharacterActive()
    {
        foreach (Unit character in characters)
        {
            character.ChangeSpriteToActive();
        }
    }

    public void ChangeIsFromCurrentPlayingTeamFromCharacters()
    {
        foreach (Unit character in characters)
        {
            character.SwapIsFromCurrentPlayingTeam();
        }
    }

    public int GetTeamHealth()
    {
        int teamHealth = 0;

        foreach (Unit unit in characters)
        {
            teamHealth += unit.currentHealth;
        }

        return teamHealth;
    }
}