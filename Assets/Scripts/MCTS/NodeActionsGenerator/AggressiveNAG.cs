using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveNAG : NodeActionsGenerator
{
    public PathFinder pathFinder;

    public TreeNode<ActionNodeState> rootOfTreeOfActions;

    public List<TreeNode<ActionNodeState>> nodesToProcess;
    public List<TreeNode<ActionNodeState>> nextNodesToProcess;

    public GameObject clonedObjectsContainer;

    public List<Unit> enemyUnitsToRepositionOnTiles;

    public Dictionary<Unit, int> acumulatedDmgOnUnit;

    void Start()
    {
        rangeFinder = new RangeFinder();
        pathFinder = new PathFinder();
        nodesToProcess = new List<TreeNode<ActionNodeState>>();
        nextNodesToProcess = new List<TreeNode<ActionNodeState>>();
        enemyUnitsToRepositionOnTiles = new List<Unit>();
        acumulatedDmgOnUnit = new Dictionary<Unit, int>();
    }

    public override List<List<Action>> GenerateActions(List<Unit> units)
    {
        amountOfNodeActionsRemainingToGenerate = nodeActionsToGenerate;
        nodesToProcess.Clear();
        nextNodesToProcess.Clear();

        //        Debug.Log("---------- INICIO DE ANAG ----------");
        List<List<Action>> listOfListOfActions = new List<List<Action>>();
        List<Unit> unitsThatCanTakeActions = new List<Unit>();
        List<Unit> originalUnits = new List<Unit>();

        foreach (Unit unit in units)
        {
            //     Debug.Log("La unidad es: " + unit + " at position: " + unit.standingOnTile.grid2DLocation);
            Unit unitClone = Instantiate(unit, unit.transform.position, Quaternion.identity, clonedObjectsContainer.transform);
            unitClone.standingOnTile = unit.standingOnTile;
            unitsThatCanTakeActions.Add(unit);
            originalUnits.Add(unitClone);
        }

        ActionNodeState rootState = new ActionNodeState();
        //        Debug.Log("Antes de generar todas las acciones posibles: " + unitsThatCanTakeActions.Count);
        rootState.allPosibleActionsNotExpanded = GenerateAllPosibleActions(unitsThatCanTakeActions);

        rootOfTreeOfActions = new TreeNode<ActionNodeState>(rootState);
        nodesToProcess.Add(rootOfTreeOfActions);

        ActionNodeState nodeState;
        TreeNode<ActionNodeState> currentNode;

        bool wasNodeAdded = false;

        int index = 0;
        int nextIndex;
        bool done = false;
        int indexOfRandomActionSelected;

        while (amountOfNodeActionsRemainingToGenerate != 0 && !done)
        {
            //      Debug.Log("Cantidad de nodos que faltan generar: " + amountOfNodeActionsRemainingToGenerate);
            if (nodesToProcess.Count == 0)
            {
                //        Debug.Log("Times called!");
                nodesToProcess = nextNodesToProcess;

                nextNodesToProcess = new List<TreeNode<ActionNodeState>>();
                index = 0;
                if (nodesToProcess.Count == 0)
                    done = true;
            }
            if (!done)
            {
                currentNode = nodesToProcess[index];
                //      Debug.Log("Current node: " + currentNode + " at index: " + index + " cantidad de acciones posibles: " + currentNode.Data.allPosibleActionsNotExpanded.Count);
                /*   foreach (Action action in currentNode.Data.allPosibleActionsNotExpanded)
                   {
                       Debug.Log(action.GetString());
                   }*/

                while (currentNode.Data.allPosibleActionsNotExpanded.Count > 0)
                {
                    indexOfRandomActionSelected = UnityEngine.Random.Range(0, currentNode.Data.allPosibleActionsNotExpanded.Count);
                    Action action = currentNode.Data.allPosibleActionsNotExpanded[indexOfRandomActionSelected];
                    currentNode.Data.allPosibleActionsNotExpanded.RemoveAt(indexOfRandomActionSelected);

                    nodeState = new ActionNodeState();

                    nodeState.actionsTakenToThisNode.AddRange(currentNode.Data.actionsTakenToThisNode);
                    nodeState.actionsTakenToThisNode.Add(action);
                    //   Debug.Log("Accion elegida: " + action.GetString());
                    nodeState.allPosibleActionsNotExpanded = GenerateAllPosibleActions(SimulateUnitsState(nodeState.actionsTakenToThisNode, unitsThatCanTakeActions));
                    //     Debug.Log("Cantidad de acciones del nodo creado: " + nodeState.allPosibleActionsNotExpanded.Count);
                    /*      foreach (Action action2 in nodeState.allPosibleActionsNotExpanded)
                          {
                              Debug.Log(action2.GetString());
                          }*/
                    TreeNode<ActionNodeState> newNode = currentNode.AddChild(nodeState);

                    RestoreAcumulatedDamage();
                    RestoreUnitsThatCanTakeActions(unitsThatCanTakeActions, originalUnits);
                    RestoreEnemyUnits();

                    currentNode = newNode;

                    wasNodeAdded = true;
                }

                if (wasNodeAdded)
                    listOfListOfActions.Add(currentNode.Data.actionsTakenToThisNode);

                //    Debug.Log("Index: " + index);
                //   Debug.Log("NTP size: " + nodesToProcess.Count);
                //    Debug.Log("NTP children length: " + nodesToProcess[index].Children.Length);

                if (nodesToProcess[index].Children.Length > 0)
                {
                    nextNodesToProcess.Add(nodesToProcess[index].Children[nodesToProcess[index].Children.Length - 1]);
                }
                if (nodesToProcess[index].Data.allPosibleActionsNotExpanded.Count == 0)
                {
                    nodesToProcess.RemoveAt(index);
                    index--;
                }

                nextIndex = index + 1;
                if (nextIndex >= nodesToProcess.Count)
                {
                    nextIndex = 0;
                }
                index = nextIndex;
                if (index < 0)
                    index = 0;
            }
            amountOfNodeActionsRemainingToGenerate--;
            wasNodeAdded = false;
        }

        amountOfNodeActionsRemainingToGenerate = nodeActionsToGenerate;
        //       Debug.Log("-------- Fin de ANAG --------");
        return listOfListOfActions;
    }

    public void RestoreAcumulatedDamage()
    {
        acumulatedDmgOnUnit.Clear();
    }

    public void RestoreUnitsThatCanTakeActions(List<Unit> unitsToRestore, List<Unit> originalUnits)
    {
        int index = 0;

        //  Debug.Log("Cantidad de unidades en unitsToRestore al restaurar: " + unitsToRestore.Count);
        //  Debug.Log("Cantidad de unidades en originalUnits al restaurar: " + originalUnits.Count);
        while (index < originalUnits.Count)
        {
            //    Debug.Log("Unidad de toRestore: " + unitsToRestore[index]);
            //    Debug.Log("Unidad de original: " + originalUnits[index]);
            unitsToRestore[index].attacked = originalUnits[index].attacked;
            unitsToRestore[index].moved = originalUnits[index].moved;
            //    Debug.Log("Borro lo que hay en la posicion: " + unitsToRestore[index].standingOnTile.grid2DLocation);
            //Removido luego por redundancia       
            if (MapManager.Instance.map[unitsToRestore[index].standingOnTile.grid2DLocation].characterOnTile == unitsToRestore[index])
                MapManager.Instance.map[unitsToRestore[index].standingOnTile.grid2DLocation].characterOnTile = null;
            unitsToRestore[index].standingOnTile = originalUnits[index].standingOnTile;
            //    Debug.Log("Restauro lo que habia en la posicion: " + originalUnits[index].standingOnTile.grid2DLocation);

            MapManager.Instance.map[originalUnits[index].standingOnTile.grid2DLocation].characterOnTile = unitsToRestore[index];
            //    Debug.Log("La posicion: " + originalUnits[index].standingOnTile.grid2DLocation + " ahora tiene: " + MapManager.Instance.map[originalUnits[index].standingOnTile.grid2DLocation].characterOnTile);
            index++;
        }
    }

    public void RestoreEnemyUnits()
    {
        foreach (Unit unit in enemyUnitsToRepositionOnTiles)
        {
            MapManager.Instance.map[unit.standingOnTile.grid2DLocation].characterOnTile = unit;
        }

        enemyUnitsToRepositionOnTiles.Clear();
    }

    public List<Unit> SimulateUnitsState(List<Action> actionsTakenInCurrentNode, List<Unit> unitsAtRootNode)
    {
        List<Unit> units = new List<Unit>();

        foreach (Action action in actionsTakenInCurrentNode)
        {
            action.SimulateForANAG(this);
        }

        foreach (Unit unit in unitsAtRootNode)
        {
            if (unit.CanTakeAction())
                units.Add(unit);
        }
        return units;
    }

    public List<Action> GenerateAllPosibleActions(List<Unit> unitsThatCanTakeActions)
    {
        List<Unit> listOfUnits = new List<Unit>();
        List<Action> allPosibleActions = new List<Action>();
        foreach (Unit unit in unitsThatCanTakeActions)
        {
            listOfUnits.Add(unit);
        }

        Unit selectedUnit;
        bool wasAnAttackActionGenerated = false;
        bool wasAMoveThatLeadToAttackGenerated = false;
        int previousAmountOfActions;

        while (listOfUnits.Count > 0)
        {
            selectedUnit = listOfUnits[0];

            //     Debug.Log("La unidad: " + selectedUnit + " attacked= " + selectedUnit.attacked + " moved= " + selectedUnit.moved);
            if (!selectedUnit.attacked)
            {
                previousAmountOfActions = allPosibleActions.Count;
                allPosibleActions.AddRange(GenerateAllAttackActions(selectedUnit));
                if (allPosibleActions.Count > previousAmountOfActions)
                    wasAnAttackActionGenerated = true;
            }
            if (!selectedUnit.moved)
            {
                previousAmountOfActions = allPosibleActions.Count;
                allPosibleActions.AddRange(GenerateAllMovementActionsThatLeadToAttacks(selectedUnit));
                if (allPosibleActions.Count > previousAmountOfActions)
                    wasAMoveThatLeadToAttackGenerated = true;
            }
            if (!selectedUnit.moved && !wasAnAttackActionGenerated && !wasAMoveThatLeadToAttackGenerated)
                allPosibleActions.AddRange(GenerateMovementActionTowardsAnEnemy(selectedUnit));

            listOfUnits.RemoveAt(0);
            wasAnAttackActionGenerated = false;
            wasAMoveThatLeadToAttackGenerated = false;
        }

        return allPosibleActions;
    }

    public bool CompareListsOfActions(List<Action> list1, List<Action> list2)
    {
        bool equal = true;

        if (list1.Count != list2.Count)
            equal = false;
        else
        {
            int indexOfLists = 0;
            while (equal && indexOfLists < list1.Count)
            {
                if (!AreTheActionsEqual(list1[indexOfLists], list2[indexOfLists]))
                    equal = false;
                indexOfLists++;
            }
        }

        return equal;
    }

    public bool AreTheActionsEqual(Action action1, Action action2)
    {
        bool equal = true;

        if (action1.unit != action2.unit)
            equal = false;
        else if (action1.originalUnit != action2.originalUnit)
            equal = false;
        else if (action1.standingGrid2DLocation != action2.standingGrid2DLocation)
            equal = false;
        else if (action1.targetGrid2DLocation != action2.targetGrid2DLocation)
            equal = false;
        else if (action1.GetType() != action2.GetType())
            equal = false;

        return equal;
    }

    public List<Action> GenerateAllAttackActions(Unit unit)
    {
        //     Debug.Log("-- Generando acciones de ataque --");
        List<Action> attackActions = new List<Action>();
        List<OverlayTile> tilesInRangeOfAttack;

        tilesInRangeOfAttack = rangeFinder.GetTilesInAttackRange(unit.standingOnTile, unit.attackRange);
        List<Unit> enemiesInRange = FindAllEnemiesInRangeOfAttack(tilesInRangeOfAttack);

        //    Debug.Log("Enemigos en rango encontrados: " + enemiesInRange.Count);
        foreach (Unit enemy in enemiesInRange)
        {
            Action attack = new Attack(unit, unit, unit.standingOnTile.grid2DLocation, enemy.standingOnTile.grid2DLocation);
            attackActions.Add(attack);
        }

        return attackActions;
    }

    public List<Action> GenerateMovementActionTowardsAnEnemy(Unit unit)
    {
        List<Action> movementActions = new List<Action>();
        List<OverlayTile> listOfPosibleMoves = rangeFinder.GetTilesInMovementRange(unit.standingOnTile, unit.movementRange);
        List<OverlayTile> tilesToMoveAndAttack = new List<OverlayTile>();
        List<OverlayTile> auxList = new List<OverlayTile>();

        foreach (OverlayTile tile in listOfPosibleMoves)
        {
            if (tile.characterOnTile != null)
                auxList.Add(tile);
        }
        foreach (OverlayTile tile in auxList)
        {
            listOfPosibleMoves.Remove(tile);
        }

        OverlayTile tileOfNearestEnemy = FindTileOfNearestEnemy(unit.standingOnTile);
        if (tileOfNearestEnemy != null)
        {
            OverlayTile closestTileFreeToNearestEnemy = FindClosestTileFreeToNearestEnemy(tileOfNearestEnemy, listOfPosibleMoves);
            OverlayTile tileToMoveTo = FindTileToMoveTo(closestTileFreeToNearestEnemy, listOfPosibleMoves);
            if (tileToMoveTo != null)
            {
                Action movement = new Movement(unit, unit, unit.standingOnTile.grid2DLocation, tileToMoveTo.grid2DLocation);
                movementActions.Add(movement);
            }
            else
            {
                Action movement = new Movement(unit, unit, unit.standingOnTile.grid2DLocation, unit.standingOnTile.grid2DLocation);
                movementActions.Add(movement);
            }
        }
        else
        {
            Action movement = new Movement(unit, unit, unit.standingOnTile.grid2DLocation, unit.standingOnTile.grid2DLocation);
            movementActions.Add(movement);
        }



        return movementActions;
    }

    public List<Action> GenerateAllMovementActionsThatLeadToAttacks(Unit unit)
    {
        List<Action> movementActions = new List<Action>();
        List<OverlayTile> listOfPosibleMoves = rangeFinder.GetTilesInMovementRange(unit.standingOnTile, unit.movementRange);
        List<OverlayTile> tilesToMoveAndAttack = new List<OverlayTile>();
        List<OverlayTile> auxList = new List<OverlayTile>();

        foreach (OverlayTile tile in listOfPosibleMoves)
        {
            if (tile.characterOnTile != null)
                auxList.Add(tile);
        }
        foreach (OverlayTile tile in auxList)
        {
            listOfPosibleMoves.Remove(tile);
        }
        //
        foreach (OverlayTile tile in listOfPosibleMoves)
        {
            //TODO: tile.characterOnTile == null es redundante, ya se sacan los tiles donde sucede eso en las lineas anteriores
            if (CanAttackFromThisTile(tile, unit))
            {
                tilesToMoveAndAttack.Add(tile);
            }
        }

        if (tilesToMoveAndAttack.Count > 0)
        {
            Action movement;
            foreach (OverlayTile tile in tilesToMoveAndAttack)
            {
                movement = new Movement(unit, unit, unit.standingOnTile.grid2DLocation, tile.grid2DLocation);
                movementActions.Add(movement);
            }
        }

        return movementActions;
    }

    public bool CanAttackFromThisTile(OverlayTile tile, Unit unit)
    {
        bool canAttack = false;
        List<OverlayTile> listOfAllPosibleTilesToAttack = rangeFinder.GetTilesInAttackRange(tile, unit.attackRange);
        int index = 0;
        while (index < listOfAllPosibleTilesToAttack.Count && !canAttack)
        {
            if (listOfAllPosibleTilesToAttack[index].characterOnTile != null && !listOfAllPosibleTilesToAttack[index].characterOnTile.isFromCurrentPlayingTeam)
                canAttack = true;
            index++;
        }
        return canAttack;
    }

    private OverlayTile FindTileOfNearestEnemy(OverlayTile startingTile)
    {
        bool enemyFound = false;
        Queue<OverlayTile> tilesToSearch = new Queue<OverlayTile>();
        tilesToSearch.Enqueue(startingTile);
        OverlayTile currentTile;
        OverlayTile tileOfEnemyFound = null;

        while (tilesToSearch.Count > 0 && !enemyFound)
        {
            currentTile = tilesToSearch.Dequeue();
            currentTile.visited = true;

            if (currentTile.characterOnTile != null && !currentTile.characterOnTile.isFromCurrentPlayingTeam)
            {
                enemyFound = true;
                tileOfEnemyFound = currentTile;
            }
            foreach (OverlayTile tile in MapManager.Instance.GetNeighbourTiles(currentTile, new List<OverlayTile>()))
            {
                if (!tile.visited)
                {
                    tilesToSearch.Enqueue(tile);
                }
            }
        }

        foreach (OverlayTile tile in MapManager.Instance.map.Values)
        {
            tile.visited = false;
        }

        return tileOfEnemyFound;
    }

    private OverlayTile FindClosestTileFreeToNearestEnemy(OverlayTile enemyTile, List<OverlayTile> listOfPosibleMoves)
    {
        OverlayTile freeTileOrEnemy = enemyTile;
        int lowestCostToFreeTileSoFar = 10000;
        int costToTile;

        //        Debug.Log("Tile donde se produce el error al buscar los vecinos: " + enemyTile.grid2DLocation);
        foreach (OverlayTile tile in MapManager.Instance.GetNeighbourTiles(enemyTile, new List<OverlayTile>()))
        {
            if (tile.characterOnTile == null && !tile.tileData.isTerrainBlocked)
            {
                //         Debug.Log("El tile: " + tile.grid2DLocation + " esta vacio? " + tile.characterOnTile);
                costToTile = FindLowestCostToTile(tile, listOfPosibleMoves);

                if (costToTile < lowestCostToFreeTileSoFar)
                {
                    lowestCostToFreeTileSoFar = costToTile;
                    freeTileOrEnemy = tile;
                }
            }
        }

        //        Debug.Log("FreetileOrEnemy: " + freeTileOrEnemy.grid2DLocation);
        return freeTileOrEnemy;
    }

    private int FindLowestCostToTile(OverlayTile enemyTile, List<OverlayTile> listOfPosibleMoves)
    {
        List<OverlayTile> pathToEnemy;
        int lowestPathCostSoFar = 10000;

        List<OverlayTile> allTiles = new List<OverlayTile>();
        foreach (OverlayTile tile in MapManager.Instance.map.Values)
        {
            allTiles.Add(tile);
        }

        foreach (OverlayTile tile in listOfPosibleMoves)
        {
            pathToEnemy = pathFinder.FindPath(tile, enemyTile, allTiles);
            tile.costToAnotherTile = CostOfPath(pathToEnemy);
            if (tile.costToAnotherTile < lowestPathCostSoFar)
            {
                lowestPathCostSoFar = tile.costToAnotherTile;
            }
        }

        foreach (OverlayTile tile in listOfPosibleMoves)
        {
            tile.costToAnotherTile = 0;
        }

        return lowestPathCostSoFar;
    }

    private int CostOfPath(List<OverlayTile> path)
    {
        int cost = 0;
        foreach (OverlayTile tile in path)
        {
            cost = cost + tile.tileData.terrainCost;
        }

        return cost;
    }

    private OverlayTile FindTileToMoveTo(OverlayTile enemyTile, List<OverlayTile> listOfPosibleMoves)
    {
        OverlayTile tileToMoveTo = null;
        List<OverlayTile> pathToEnemy;
        int lowestPathCostSoFar = 10000;

        List<OverlayTile> allTiles = new List<OverlayTile>();
        foreach (OverlayTile tile in MapManager.Instance.map.Values)
        {
            allTiles.Add(tile);
        }

        foreach (OverlayTile tile in listOfPosibleMoves)
        {
            pathToEnemy = pathFinder.FindPath(tile, enemyTile, allTiles);
            tile.costToAnotherTile = CostOfPath(pathToEnemy);
            if (tile.costToAnotherTile < lowestPathCostSoFar)
            {
                lowestPathCostSoFar = tile.costToAnotherTile;
                tileToMoveTo = tile;
            }
        }

        foreach (OverlayTile tile in listOfPosibleMoves)
        {
            tile.costToAnotherTile = 0;
        }

        return tileToMoveTo;
    }

    private List<Unit> FindAllEnemiesInRangeOfAttack(List<OverlayTile> tilesInRangeOfAttack)
    {
        List<Unit> enemies = new List<Unit>();

        while (tilesInRangeOfAttack.Count > 0)
        {
            OverlayTile tile = tilesInRangeOfAttack[0];
            tilesInRangeOfAttack.RemoveAt(0);

            if (tile.characterOnTile != null && !tile.characterOnTile.isFromCurrentPlayingTeam)
            {
                enemies.Add(tile.characterOnTile);
            }

            /*   if (tile.characterOnTile != null)
               {
                   Debug.Log("Cuando busca enemigos en rango, encuentra una unidad, es del mismo equipo? " + tile.characterOnTile.isFromCurrentPlayingTeam);
               }*/
        }

        return enemies;
    }
}