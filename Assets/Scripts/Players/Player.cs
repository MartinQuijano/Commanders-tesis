using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    protected TeamManager myTeam;
    protected bool canTakeAction;

    public LogManager logManager;

    public abstract void OnTurnStart();

    public abstract void OnTurnEnd();

    public abstract bool CanUseButtons();
    public abstract void ExecuteListOfActions(List<Action> actions);

    public int playerNumber;

    public void SetCanTakeAction(bool value)
    {
        canTakeAction = value;
    }

    public bool CanTakeAction()
    {
        return canTakeAction;
    }

    public void SetTeam(TeamManager team)
    {
        myTeam = team;
    }

    public TeamManager GetTeam()
    {
        return myTeam;
    }
}
