using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeActionsGenerator : MonoBehaviour
{
    public int nodeActionsToGenerate;
    protected int amountOfNodeActionsRemainingToGenerate;
    public RangeFinder rangeFinder;

    public abstract List<List<Action>> GenerateActions(List<Unit> units);

}