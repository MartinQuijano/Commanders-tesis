using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : Action
{
    public PathFinder pathFinder;
    public RangeFinder rangeFinder;
    public Movement(Unit unit, Unit originalUnit, Vector2Int standingGrid2DLocation, Vector2Int targetGrid2DLocation)
    {
        this.unit = unit;
        this.originalUnit = originalUnit;
        this.standingGrid2DLocation = standingGrid2DLocation;
        this.targetGrid2DLocation = targetGrid2DLocation;
        executed = false;
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
    }

    public override void Execute(MCTSAI iAMCTSController)
    {
        /*  Debug.Log("Ejecutando accion de movimiento de la unidad " + originalUnit);
          Debug.Log("Desde posicion " + standingGrid2DLocation);
          Debug.Log("Hasta posicion " + targetGrid2DLocation);*/
        //  Debug.Log("Cantidad de tiles en rango: " + rangeFinder.GetTilesInMovementRangeForEnemy(MapManager.Instance.map[standingGrid2DLocation], originalUnit.movementRange).Count);
        List<OverlayTile> path = pathFinder.FindPath(MapManager.Instance.map[standingGrid2DLocation], MapManager.Instance.map[targetGrid2DLocation], rangeFinder.GetTilesInMovementRangeForEnemy(MapManager.Instance.map[standingGrid2DLocation], originalUnit.movementRange));
        //   Debug.Log("Longitud de camino: " + path.Count);
        originalUnit.MoveAlongPathForAIMCTS(path, iAMCTSController);

    }

    public override void SimulateExpansion(GameState startingState, int playerNumber)
    {
        //      Debug.Log("SimulateExp");
        //      Debug.Log(" >>>>>>>>> Accion de movimiento de " + unit + " desde la posicion " + standingGrid2DLocation + " a la posicion " + targetGrid2DLocation);
        unit.standingOnTile = MapManager.Instance.map[targetGrid2DLocation];
    }

    public override void Simulate(GameState startingState, int playerNumber, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        //     Debug.Log("Simulate");
        //     Debug.Log(" >>>>>>>>> Accion de movimiento de " + unit + " desde la posicion " + standingGrid2DLocation + " a la posicion " + targetGrid2DLocation);
        unit.standingOnTile = MapManager.Instance.map[targetGrid2DLocation];
    }

    public override void SimulateForANAG(AggressiveNAG anag)
    {
        unit.SimulateMovementNAG();
        MapManager.Instance.map[standingGrid2DLocation].characterOnTile = null;
        unit.standingOnTile = MapManager.Instance.map[targetGrid2DLocation];
        MapManager.Instance.map[targetGrid2DLocation].characterOnTile = unit;
    }

    public override void LogSimulate(Simulator simulator)
    {
        List<OverlayTile> path = pathFinder.FindPath(MapManager.Instance.map[standingGrid2DLocation], MapManager.Instance.map[targetGrid2DLocation], rangeFinder.GetTilesInMovementRangeForEnemy(MapManager.Instance.map[standingGrid2DLocation], originalUnit.movementRange));
        originalUnit.MoveAlongPathForAISim(path, simulator);
        //  simulator.ExecuteNextAction();
    }

    public override string GetLogName()
    {
        return "Movement";
    }

    public override string GetString()
    {
        return "Movement action from position: " + standingGrid2DLocation + " to position: " + targetGrid2DLocation;
    }

}
