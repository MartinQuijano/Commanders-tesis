using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : Player
{
    public List<Action> listOfActionsToPerform;
    public LogExecutor logExecutor;

    public override void ExecuteListOfActions(List<Action> actions)
    {
        listOfActionsToPerform = actions;
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
            action.LogSimulate(this);
        }
        else
        {
            OnTurnEnd();
            logExecutor.ExecuteNextListOfActions();
        }
    }

    public override void OnTurnStart()
    {
        myTeam.MakeAllCharacterActive();
        myTeam.ChangeIsFromCurrentPlayingTeamFromCharacters();
        myTeam.RefreshCharacters();
    }

    public override void OnTurnEnd()
    {
        myTeam.MakeAllCharacterActive();
        myTeam.ChangeIsFromCurrentPlayingTeamFromCharacters();
    }

    public override bool CanUseButtons()
    {
        return false;
    }
}
