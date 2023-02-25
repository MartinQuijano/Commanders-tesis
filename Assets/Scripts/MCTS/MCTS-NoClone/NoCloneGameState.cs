/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCloneGameState
{
    public List<int> teamOne_health;
    public List<Vector2Int> teamOne_position;
    public List<int> teamOne_damage;
    public List<int> teamOne_moveRange;
    public List<int> teamOne_attackRange;

    public List<int> teamTwo_health;
    public List<Vector2Int> teamTwo_position;
    public List<int> teamTwo_damage;
    public List<int> teamTwo_moveRange;
    public List<int> teamTwo_attackRange;

    private List<Action> takenActionsToThisState;
    private int remainingAmountOfExpansionPosible;
    public int currentPlayer;
    public int extraReward;

    public NoCloneGameState()
    {
        teamOne_health = new List<int>();
        teamOne_position = new List<Vector2Int>();
        teamOne_damage = new List<int>();
        teamOne_moveRange = new List<int>();
        teamOne_attackRange = new List<int>();

        teamTwo_health = new List<int>();
        teamTwo_position = new List<Vector2Int>();
        teamTwo_damage = new List<int>();
        teamTwo_moveRange = new List<int>();
        teamTwo_attackRange = new List<int>();

        takenActionsToThisState = new List<Action>();
        remainingAmountOfExpansionPosible = 50;
        currentPlayer = 2;
        extraReward = 0;
    }

    public List<Action> TakenActionsToThisState { get { return takenActionsToThisState; } set { takenActionsToThisState = value; } }
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

*/