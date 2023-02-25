using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveAI : AI
{
    public TurnManager turnManager;

    public RangeFinder rangeFinder;
    private PathFinder pathFinder;

    public List<Unit> charactersWithActions;
    public bool canTakeAnAction;
    public bool waitingForNextAction;

    public Unit selectedCharacter;

    void Start()
    {
        canTakeAnAction = false;
        waitingForNextAction = false;
        rangeFinder = new RangeFinder();
        pathFinder = new PathFinder();
    }

    void Update()
    {
        if (charactersWithActions.Count > 0)
        {
            if (canTakeAnAction)
            {
                canTakeAnAction = false;

                int randomIndex = Random.Range(0, charactersWithActions.Count);
                selectedCharacter = charactersWithActions[randomIndex];

                List<OverlayTile> tilesInRangeOfAttack = rangeFinder.GetTilesInAttackRange(selectedCharacter.standingOnTile, selectedCharacter.attackRange);
                Unit enemyInRangeOfAttack = FindOneEnemyInRange(tilesInRangeOfAttack);
                if (enemyInRangeOfAttack != null && !selectedCharacter.attacked)
                {
                    enemyInRangeOfAttack = FindOneEnemyInRange(tilesInRangeOfAttack);
                    if (enemyInRangeOfAttack != null)
                    {
                        selectedCharacter.AttackEnemy(enemyInRangeOfAttack);
                        enemyInRangeOfAttack = null;
                    }
                }
                else
                {
                    if (selectedCharacter.moved && FindOneEnemyInRange(tilesInRangeOfAttack) == null)
                    {
                        selectedCharacter.attacked = true;
                    }
                    else
                    {
                        enemyInRangeOfAttack = FindOneEnemyInRange(tilesInRangeOfAttack);
                        if (enemyInRangeOfAttack == null && !selectedCharacter.moved)
                        {
                            List<OverlayTile> rangeOfMoves = rangeFinder.GetTilesInMovementRange(selectedCharacter.standingOnTile, selectedCharacter.movementRange);
                            List<OverlayTile> listOfPosibleMoves = rangeFinder.GetTilesInMovementRange(selectedCharacter.standingOnTile, selectedCharacter.movementRange);
                            List<OverlayTile> toRemove = new List<OverlayTile>();

                            foreach (OverlayTile tile in listOfPosibleMoves)
                            {
                                if (tile.characterOnTile != null)
                                {
                                    toRemove.Add(tile);
                                }
                            }
                            foreach (OverlayTile tile in toRemove)
                            {
                                listOfPosibleMoves.Remove(tile);
                            }

                            OverlayTile tileOfNearestEnemy = FindTileOfNearestEnemy(selectedCharacter.standingOnTile);
                            OverlayTile closestTileFreeToNearestEnemy = FindClosestTileFreeToNearestEnemy(tileOfNearestEnemy, listOfPosibleMoves);
                            OverlayTile tileToMoveTo = FindTileToMoveTo(closestTileFreeToNearestEnemy, listOfPosibleMoves);
                            List<OverlayTile> path = pathFinder.FindPath(selectedCharacter.standingOnTile, tileToMoveTo, rangeOfMoves);
                            selectedCharacter.MoveAlongPathForAI(path);
                        }
                        else
                        {
                            selectedCharacter.moved = true;
                        }
                    }
                }
                if (selectedCharacter.moved && selectedCharacter.attacked)
                {
                    charactersWithActions.RemoveAt(randomIndex);
                }
            }
            else
            {
                if (!selectedCharacter.isMoving && !waitingForNextAction)
                {
                    if (!canTakeAnAction)
                        StartCoroutine(WaitToSimulatePauseBeforeAction());
                }
            }
        }
        else
        {
            if (selectedCharacter != null && !selectedCharacter.isMoving)
            {
                canTakeAnAction = true;
            }
            if (canTakeAnAction)
            {
                selectedCharacter = null;
                canTakeAnAction = false;
                StartCoroutine(WaitToSimulatePauseBeforeEndTurn());

            }
        }
    }

    private OverlayTile FindClosestTileFreeToNearestEnemy(OverlayTile enemyTile, List<OverlayTile> listOfPosibleMoves)
    {
        OverlayTile freeTileOrEnemy = enemyTile;
        int lowestCostToFreeTileSoFar = 10000;
        int costToTile;

        foreach (OverlayTile tile in MapManager.Instance.GetNeighbourTiles(enemyTile, new List<OverlayTile>()))
        {
            if (tile.characterOnTile == null && !tile.tileData.isTerrainBlocked)
            {
                costToTile = FindLowestCostToTile(tile, listOfPosibleMoves);

                if (costToTile < lowestCostToFreeTileSoFar)
                {
                    lowestCostToFreeTileSoFar = costToTile;
                    freeTileOrEnemy = tile;
                }
            }
        }

        return freeTileOrEnemy;
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

    private Unit FindOneEnemyInRange(List<OverlayTile> tilesInRangeOfAttack)
    {
        bool enemyFound = false;
        Unit enemy = null;
        List<OverlayTile> tilesInRangeOfAttackAux = new List<OverlayTile>();
        foreach (OverlayTile tile in tilesInRangeOfAttack)
        {
            tilesInRangeOfAttackAux.Add(tile);
        }

        while (!enemyFound && tilesInRangeOfAttackAux.Count > 0)
        {
            OverlayTile tile = tilesInRangeOfAttackAux[0];
            tilesInRangeOfAttackAux.RemoveAt(0);

            if (tile.characterOnTile != null && !tile.characterOnTile.isFromCurrentPlayingTeam)
            {
                enemyFound = true;
                enemy = tile.characterOnTile;
            }
        }

        return enemy;
    }

    private OverlayTile TileToNearestEnemy(List<OverlayTile> listOfPosibleMoves, OverlayTile nearestEnemy)
    {
        OverlayTile tileWithMinimumDistance = null;
        int currentMinDistance = 10000;
        int distance;

        foreach (OverlayTile tile in listOfPosibleMoves)
        {
            distance = GetManhattanDistance(tile, nearestEnemy);
            if (distance < currentMinDistance)
            {
                currentMinDistance = distance;
                tileWithMinimumDistance = tile;
            }
        }

        return tileWithMinimumDistance;
    }

    private int GetManhattanDistance(OverlayTile start, OverlayTile end)
    {
        return Mathf.Abs(start.gridLocation.x - end.gridLocation.x) + Mathf.Abs(start.gridLocation.y - end.gridLocation.y);
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

    IEnumerator WaitToSimulatePauseBeforeAction()
    {
        waitingForNextAction = true;
        yield return new WaitForSeconds(0.5f);
        canTakeAnAction = true;
        waitingForNextAction = false;
    }

    IEnumerator WaitToSimulatePauseBeforeEndTurn()
    {
        yield return new WaitForSeconds(0.75f);
        turnManager.EndTurn();
    }

    public override void OnTurnStart()
    {
        myTeam.MakeAllCharacterActive();
        myTeam.ChangeIsFromCurrentPlayingTeamFromCharacters();
        myTeam.RefreshCharacters();
        foreach (Unit character in myTeam.characters)
        {
            charactersWithActions.Add(character);
        }

        canTakeAnAction = true;
    }

    public override void ExecuteListOfActions(List<Action> actions)
    {

    }
}
