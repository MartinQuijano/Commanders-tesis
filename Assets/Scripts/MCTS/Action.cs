using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public Vector2Int standingGrid2DLocation;
    public Vector2Int targetGrid2DLocation;

    public CharacterInfo unit;
    public CharacterInfo originalUnit;

    public bool executed;

    public virtual void Execute(IAMCTSController iAMCTSController)
    {

    }

    public virtual void Simulate(GameState startingState)
    {
    }
}
