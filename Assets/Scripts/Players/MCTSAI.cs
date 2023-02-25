using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTSAI : AI
{
    public TurnManager turnManager;
    public bool canTakeAnAction;
    public MCTS actionsFinder;
    public List<Action> listOfActionsToPerform;

    void Start()
    {
        canTakeAnAction = false;
    }

    public override void OnTurnStart()
    {
        myTeam.MakeAllCharacterActive();
        myTeam.ChangeIsFromCurrentPlayingTeamFromCharacters();
        myTeam.RefreshCharacters();
        canTakeAnAction = true;
        actionsFinder.FindAndExecuteMoves();
    }

    public void ExecuteActions(List<Action> listOfActions)
    {
        listOfActionsToPerform = listOfActions;
        if (listOfActions.Count > 0)
            logManager.LogListOfActions(listOfActions);
        ExecuteNextAction();
    }

    public void ExecuteNextAction()
    {
        StartCoroutine(WaitAndExecuteNextAction());
    }

    IEnumerator WaitAndExecuteNextAction()
    {
        yield return new WaitForSeconds(0.5f);
        if (listOfActionsToPerform.Count > 0)
        {
            Action action = listOfActionsToPerform[0];
            listOfActionsToPerform.RemoveAt(0);
            action.Execute(this);
        }
        else
        {
            turnManager.EndTurn();
        }
    }

    public override void ExecuteListOfActions(List<Action> actions)
    {

    }

}
