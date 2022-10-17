using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : Action
{
    public PathFinder pathFinder;
    public RangeFinder rangeFinder;
    public Movement(CharacterInfo unit, CharacterInfo originalUnit, Vector2Int standingGrid2DLocation, Vector2Int targetGrid2DLocation)
    {
        this.unit = unit;
        this.originalUnit = originalUnit;
        this.standingGrid2DLocation = standingGrid2DLocation;
        this.targetGrid2DLocation = targetGrid2DLocation;
        executed = false;
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
    }

    public override void Execute(IAMCTSController iAMCTSController)
    {
        //  Debug.Log("Ejecutando accion de movimiento de la unidad " + originalUnit);
        //  Debug.Log("Desde posicion " + standingGrid2DLocation);
        //  Debug.Log("Hasta posicion " + targetGrid2DLocation);
        // TODO: calcular el camino por el cual se tiene que mover la unidad de la IA.
        //  Debug.Log("Cantidad de tiles en rango: " + rangeFinder.GetTilesInMovementRangeForEnemy(MapManager.Instance.map[standingGrid2DLocation], originalUnit.movementRange).Count);
        List<OverlayTile> path = pathFinder.FindPath(MapManager.Instance.map[standingGrid2DLocation], MapManager.Instance.map[targetGrid2DLocation], rangeFinder.GetTilesInMovementRangeForEnemy(MapManager.Instance.map[standingGrid2DLocation], originalUnit.movementRange));
        //   Debug.Log("Longitud de camino: " + path.Count);
        originalUnit.MoveAlongPathForIATest2(path, iAMCTSController);

    }

    public override void Simulate(GameState startingState)
    {
        //TODO: cuando se simula el movimiento, es necesario actualizar los datos de donde se esta parando ahora la unidad, cambia el standingOnTile
        // recordar que no se esta copiando el tablero.
        // CharacterInfo unit = startingState.MyTeam
        //  Debug.Log("Accion de movimiento de " + unit + " desde la posicion " + standingGrid2DLocation + " a la posicion " + targetGrid2DLocation);
        unit.standingOnTile = MapManager.Instance.map[targetGrid2DLocation];
    }
}
