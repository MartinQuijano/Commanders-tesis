using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    public Vector2Int standingGrid2DLocation;
    public Vector2Int targetGrid2DLocation;
    public Unit unit;
    public Unit originalUnit;
    public bool executed;

    public abstract void Execute(MCTSAI iAMCTSController);
    public abstract void SimulateExpansion(GameState startingState, int playerNumber);
    public abstract void Simulate(GameState startingState, int playerNumber, Dictionary<Unit, int> dicOfDmgTakenByUnits);
    public abstract void SimulateForANAG(AggressiveNAG anag);
    public abstract void LogSimulate(Simulator simulator);
    public abstract string GetLogName();
    public abstract string GetString();
}