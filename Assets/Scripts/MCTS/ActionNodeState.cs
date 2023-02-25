using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNodeState
{
    public List<Action> actionsTakenToThisNode;
    public List<Action> allPosibleActionsNotExpanded;

    public ActionNodeState()
    {
        actionsTakenToThisNode = new List<Action>();
    }
}
