using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArrowTranslator;

public class OverlayTile : MonoBehaviour
{
    public int G;
    public int H;
    public int F { get { return G + H; } }
    public OverlayTile previous;
    public Vector3Int gridLocation;
    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }
    public TileData tileData;
    public Sprite spriteFromTileBelow;
    public CharacterInfo characterOnTile;

    public List<Sprite> arrows;

    public bool visited;

    public int costToMoveToThisTile = 0;
    public int costToAnotherTile = 0;

    void Start()
    {
        visited = false;
    }

    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void ShowRedTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
    }

    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetArrowSprite(ArrowDirection.None);
    }

    public void SetArrowSprite(ArrowDirection d)
    {
        var arrow = GetComponentsInChildren<SpriteRenderer>()[1];
        if (d == ArrowDirection.None)
        {
            arrow.color = new Color(1, 1, 1, 0);
        }
        else
        {
            arrow.color = new Color(1, 1, 1, 1);
            arrow.sprite = arrows[(int)d];
            arrow.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }

    public void SetCharacterStandingOnTile(CharacterInfo characterInfo)
    {
        characterOnTile = characterInfo;
    }
}
