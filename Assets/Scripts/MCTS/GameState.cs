using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    private List<CharacterInfo> myTeam;
    private List<CharacterInfo> enemyTeam;
    private List<Action> takenActionsToThisState;
    private int remainingAmountOfExpansionPosible;
    public int currentPlayer;
    public int extraReward;

    public GameState()
    {
        myTeam = new List<CharacterInfo>();
        enemyTeam = new List<CharacterInfo>();
        takenActionsToThisState = new List<Action>();
        remainingAmountOfExpansionPosible = 30;
        currentPlayer = 2;
        extraReward = 0;
    }

    public List<Action> TakenActionsToThisState { get { return takenActionsToThisState; } set { takenActionsToThisState = value; } }
    public List<CharacterInfo> MyTeam { get { return myTeam; } }
    public List<CharacterInfo> EnemyTeam { get { return enemyTeam; } }
    public int RemainingAmountOfExpansionPosible { get { return remainingAmountOfExpansionPosible; } }

    public void ReduceAmountOfExpansionPosible()
    {
        remainingAmountOfExpansionPosible--;
    }

    public int GetExtraReward()
    {
        return extraReward;
    }
}
