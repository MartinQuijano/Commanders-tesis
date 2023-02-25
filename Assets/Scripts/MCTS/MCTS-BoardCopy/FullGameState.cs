using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullGameState
{
    private List<Unit> myTeam;
    private List<Unit> enemyTeam;
    private List<Action> takenActionsToThisState;
    private int remainingAmountOfExpansionPosible;
    public int currentPlayer;
    public int extraReward;

    public Dictionary<Vector2Int, OverlayTile> map;

    public FullGameState()
    {
        myTeam = new List<Unit>();
        enemyTeam = new List<Unit>();
        takenActionsToThisState = new List<Action>();
        remainingAmountOfExpansionPosible = 50;
        currentPlayer = 2;
        extraReward = 0;
        map = new Dictionary<Vector2Int, OverlayTile>();
    }

    public List<Action> TakenActionsToThisState { get { return takenActionsToThisState; } set { takenActionsToThisState = value; } }
    public List<Unit> MyTeam { get { return myTeam; } }
    public List<Unit> EnemyTeam { get { return enemyTeam; } }
    public int RemainingAmountOfExpansionPosible { get { return remainingAmountOfExpansionPosible; } }

    public void ReduceAmountOfExpansionPosible()
    {
        remainingAmountOfExpansionPosible--;
    }

    public int GetExtraReward()
    {
        return extraReward;
    }

    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> searchableTiles)
    {
        Dictionary<Vector2Int, OverlayTile> tileToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if (searchableTiles.Count > 0)
        {
            foreach (var item in searchableTiles)
            {
                tileToSearch.Add(item.grid2DLocation, item);
            }
        }
        else
        {
            tileToSearch = map;
        }

        List<OverlayTile> neighbours = new List<OverlayTile>();

        // up
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }
        // down
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }
        // right
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }
        // left
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        return neighbours;
    }
}
