using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinder
{
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchableTiles)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closedList = new List<OverlayTile>();

        openList.Add(start);
        start.G = 0;
        start.H = 0;

        while (openList.Count > 0)
        {
            openList = openList.OrderBy(x => x.F).ToList();
            OverlayTile currentOverlayTile = openList.First();

            int posIndex = 1;
            int indexOfLowestH = 0;

            while (openList.Count > posIndex && currentOverlayTile.F == openList.ElementAt(posIndex).F)
            {
                if (openList.ElementAt(posIndex).H < currentOverlayTile.H)
                {
                    indexOfLowestH = posIndex;
                }
                posIndex++;
            }

            currentOverlayTile = openList.ElementAt(indexOfLowestH);
            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if (currentOverlayTile == end)
            {
                return GetFinishedList(start, end);
            }

            var neighbourTiles = MapManager.Instance.GetNeighbourTiles(currentOverlayTile, searchableTiles);

            foreach (var neighbour in neighbourTiles)
            {
                if (neighbour.tileData.isTerrainBlocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                neighbour.G = currentOverlayTile.G + neighbour.tileData.terrainCost;
                neighbour.H = GetManhattanDistance(end, neighbour) + neighbour.tileData.terrainCost;
                neighbour.previous = currentOverlayTile;

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
                if (neighbour == end)
                {
                    return GetFinishedList(start, end);
                }
            }
        }

        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();

        OverlayTile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }

        finishedList.Reverse();
        return finishedList;
    }

    private int GetManhattanDistance(OverlayTile start, OverlayTile end)
    {
        return Mathf.Abs(start.gridLocation.x - end.gridLocation.x) + Mathf.Abs(start.gridLocation.y - end.gridLocation.y);
    }
}