using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInMovementRange(OverlayTile startingTile, int range)
    {
        var inRangeTiles = new List<OverlayTile>();

        inRangeTiles.Add(startingTile);

        var tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);
        while (tileForPreviousStep.Count > 0)
        {
            var surroundingTiles = new List<OverlayTile>();

            foreach (var item in tileForPreviousStep)
            {
                foreach (var tile in MapManager.Instance.GetNeighbourTiles(item, new List<OverlayTile>()))
                {
                    if (!tile.tileData.isTerrainBlocked)
                    {
                        if (tile.characterOnTile != null && !tile.characterOnTile.isFromCurrentPlayingTeam) { }
                        else
                        {
                            if (tile.costToMoveToThisTile != 0)
                            {
                                if (tile.tileData.terrainCost + item.costToMoveToThisTile < tile.costToMoveToThisTile)
                                    tile.costToMoveToThisTile = tile.tileData.terrainCost + item.costToMoveToThisTile;
                            }
                            else
                                tile.costToMoveToThisTile = tile.tileData.terrainCost + item.costToMoveToThisTile;

                            if (range > tile.costToMoveToThisTile && !inRangeTiles.Contains(tile))
                            {
                                surroundingTiles.Add(tile);
                            }
                            else if (range == tile.costToMoveToThisTile && !inRangeTiles.Contains(tile) && tile.characterOnTile == null)
                            {
                                surroundingTiles.Add(tile);
                            }
                        }
                    }
                }
            }

            inRangeTiles.AddRange(surroundingTiles);
            tileForPreviousStep = surroundingTiles.Distinct().ToList();
        }
        foreach (OverlayTile tile in MapManager.Instance.map.Values)
        {
            tile.costToMoveToThisTile = 0;
        }

        inRangeTiles = inRangeTiles.Distinct().ToList();
        return inRangeTiles;
    }

    public List<OverlayTile> GetTilesInMovementRangeForEnemyForGridHUD(OverlayTile startingTile, int range)
    {
        var inRangeTiles = new List<OverlayTile>();

        inRangeTiles.Add(startingTile);

        var tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);
        while (tileForPreviousStep.Count > 0)
        {
            var surroundingTiles = new List<OverlayTile>();

            foreach (var item in tileForPreviousStep)
            {
                foreach (var tile in MapManager.Instance.GetNeighbourTiles(item, new List<OverlayTile>()))
                {
                    if (!tile.tileData.isTerrainBlocked)
                    {
                        if (tile.characterOnTile != null && tile.characterOnTile.isFromCurrentPlayingTeam) { }
                        else
                        {
                            if (tile.costToMoveToThisTile != 0)
                            {
                                if (tile.tileData.terrainCost + item.costToMoveToThisTile < tile.costToMoveToThisTile)
                                    tile.costToMoveToThisTile = tile.tileData.terrainCost + item.costToMoveToThisTile;
                            }
                            else
                                tile.costToMoveToThisTile = tile.tileData.terrainCost + item.costToMoveToThisTile;

                            if (range > tile.costToMoveToThisTile)
                            {
                                surroundingTiles.Add(tile);
                            }
                            else if (range == tile.costToMoveToThisTile && tile.characterOnTile == null)
                            {
                                surroundingTiles.Add(tile);
                            }
                        }
                    }
                }
            }
            foreach (var tile in inRangeTiles)
            {
                surroundingTiles.Remove(tile);
            }
            inRangeTiles.AddRange(surroundingTiles);

            tileForPreviousStep = surroundingTiles.Distinct().ToList();
        }
        foreach (OverlayTile tile in MapManager.Instance.map.Values)
        {
            tile.costToMoveToThisTile = 0;
        }

        inRangeTiles = inRangeTiles.Distinct().ToList();
        return inRangeTiles;
    }

    public List<OverlayTile> GetTilesInMovementRangeForEnemy(OverlayTile startingTile, int range)
    {
        var inRangeTiles = new List<OverlayTile>();

        inRangeTiles.Add(startingTile);

        var tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);
        while (tileForPreviousStep.Count > 0)
        {
            var surroundingTiles = new List<OverlayTile>();

            foreach (var item in tileForPreviousStep)
            {
                foreach (var tile in MapManager.Instance.GetNeighbourTiles(item, new List<OverlayTile>()))
                {
                    if (!tile.tileData.isTerrainBlocked)
                    {
                        if (tile.characterOnTile != null && !tile.characterOnTile.isFromCurrentPlayingTeam) { }
                        else
                        {
                            if (tile.costToMoveToThisTile != 0)
                            {
                                if (tile.tileData.terrainCost + item.costToMoveToThisTile < tile.costToMoveToThisTile)
                                    tile.costToMoveToThisTile = tile.tileData.terrainCost + item.costToMoveToThisTile;
                            }
                            else
                                tile.costToMoveToThisTile = tile.tileData.terrainCost + item.costToMoveToThisTile;

                            if (range > tile.costToMoveToThisTile)
                            {
                                surroundingTiles.Add(tile);
                            }
                            else if (range == tile.costToMoveToThisTile && tile.characterOnTile == null)
                            {
                                surroundingTiles.Add(tile);
                            }
                        }
                    }
                }
            }
            foreach (var tile in inRangeTiles)
            {
                surroundingTiles.Remove(tile);
            }
            inRangeTiles.AddRange(surroundingTiles);

            tileForPreviousStep = surroundingTiles.Distinct().ToList();
        }
        foreach (OverlayTile tile in MapManager.Instance.map.Values)
        {
            tile.costToMoveToThisTile = 0;
        }

        inRangeTiles = inRangeTiles.Distinct().ToList();
        return inRangeTiles;
    }

    public List<OverlayTile> GetTilesInAttackRange(OverlayTile startingTile, int range)
    {
        var inRangeTiles = new List<OverlayTile>();

        inRangeTiles.Add(startingTile);

        var tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);
        while (range > 0)
        {
            var surroundingTiles = new List<OverlayTile>();

            foreach (var item in tileForPreviousStep)
            {
                foreach (var tile in MapManager.Instance.GetNeighbourTiles(item, new List<OverlayTile>()))
                {
                    surroundingTiles.Add(tile);
                }
            }
            range--;
            inRangeTiles.AddRange(surroundingTiles);
            tileForPreviousStep = surroundingTiles.Distinct().ToList();
        }

        inRangeTiles.Remove(startingTile);
        inRangeTiles = inRangeTiles.Distinct().ToList();
        return inRangeTiles;
    }

    public List<OverlayTile> GetTilesInMovementRangeMCTS(OverlayTile startingTile, int range, List<CharacterInfo> myTeam, List<CharacterInfo> enemyTeam)
    {
        var inRangeTiles = new List<OverlayTile>();

        inRangeTiles.Add(startingTile);

        var tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);
        while (tileForPreviousStep.Count > 0)
        {
            var surroundingTiles = new List<OverlayTile>();

            foreach (var item in tileForPreviousStep)
            {
                foreach (var tile in MapManager.Instance.GetNeighbourTiles(item, new List<OverlayTile>()))
                {
                    if (!tile.tileData.isTerrainBlocked)
                    {
                        if (ThereIsCharacterFromOppositeTeamOnTileMCTS(tile, enemyTeam)) { }
                        else
                        {
                            if (tile.costToMoveToThisTile != 0)
                            {
                                if (tile.tileData.terrainCost + item.costToMoveToThisTile < tile.costToMoveToThisTile)
                                    tile.costToMoveToThisTile = tile.tileData.terrainCost + item.costToMoveToThisTile;
                            }
                            else
                                tile.costToMoveToThisTile = tile.tileData.terrainCost + item.costToMoveToThisTile;

                            if (range > tile.costToMoveToThisTile && !inRangeTiles.Contains(tile))
                            {
                                surroundingTiles.Add(tile);
                            }
                            else if (range == tile.costToMoveToThisTile && !inRangeTiles.Contains(tile) && !ThereIsCharacterOnTileMCTS(tile, myTeam, enemyTeam))
                            {
                                surroundingTiles.Add(tile);
                            }
                        }
                    }
                }
            }

            inRangeTiles.AddRange(surroundingTiles);
            tileForPreviousStep = surroundingTiles.Distinct().ToList();
        }
        foreach (OverlayTile tile in MapManager.Instance.map.Values)
        {
            tile.costToMoveToThisTile = 0;
        }

        inRangeTiles = inRangeTiles.Distinct().ToList();
        return inRangeTiles;
    }

    private bool ThereIsCharacterFromOppositeTeamOnTileMCTS(OverlayTile tile, List<CharacterInfo> enemyTeam)
    {
        bool thereIsCharacter = false;
        int index = 0;

        while (!thereIsCharacter && index < enemyTeam.Count)
        {
            if (tile == enemyTeam[index].standingOnTile)
                thereIsCharacter = true;
            index++;
        }

        return thereIsCharacter;
    }

    private bool ThereIsCharacterOnTileMCTS(OverlayTile tile, List<CharacterInfo> myTeam, List<CharacterInfo> enemyTeam)
    {
        bool thereIsCharacter = false;

        int index = 0;

        while (!thereIsCharacter && index < myTeam.Count)
        {
            if (tile == myTeam[index].standingOnTile)
                thereIsCharacter = true;
            index++;
        }

        index = 0;

        while (!thereIsCharacter && index < enemyTeam.Count)
        {
            if (tile == enemyTeam[index].standingOnTile)
                thereIsCharacter = true;
            index++;
        }

        return thereIsCharacter;
    }
}