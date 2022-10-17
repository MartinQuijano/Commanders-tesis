using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAMCTSController : MonoBehaviour
{
    public TurnManager turnManager;
    public bool canTakeAnAction;
    public UCTMCTS actionsFinder;
    public List<Action> listOfActionsToPerform;

    void Start()
    {
        canTakeAnAction = false;
    }

    public void OnTurnStart()
    {
        canTakeAnAction = true;
        actionsFinder.FindAndExecuteMoves();
    }

    public void ExecuteActions(List<Action> listOfActions)
    {
        listOfActionsToPerform = listOfActions;

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
        else turnManager.EndTurn();
    }

}
