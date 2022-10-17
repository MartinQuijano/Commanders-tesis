using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;

    public Tilemap tileMap;

    [SerializeField]
    private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        map = new Dictionary<Vector2Int, OverlayTile>();
        tileMap.CompressBounds();
        BoundsInt bounds = tileMap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var tileLocation = new Vector3Int(x, y, 0);
                var tileKey = new Vector2Int(x, y);

                if (tileMap.HasTile(tileLocation))
                {
                    var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                    var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                    overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                    overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder + 1;
                    overlayTile.gridLocation = tileLocation;
                    overlayTile.tileData = dataFromTiles[tileMap.GetTile(tileLocation)];
                    overlayTile.spriteFromTileBelow = tileMap.GetSprite(tileLocation);
                    overlayTile.visited = false;
                    map.Add(tileKey, overlayTile);
                }
            }
        }
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
