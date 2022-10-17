using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MouseController : MonoBehaviour
{
    public CharacterInfo selectedCharacter;
    private CharacterInfo clickedCharacter;
    private CharacterInfo pressedCharacter;

    private OverlayTile hoveredOverlayTile;
    private OverlayTile lastSelectedTile;

    private RaycastHit2D[] focusedTileHit;

    public Image terrainImage;
    public Text terrainMovCost;

    public Image unitImage;
    public Text unitDamage;
    public Text unitMovement;
    public Text unitAttack;

    public TurnManager turnManager;

    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        focusedTileHit = GetFocusedOnTile();
        if (focusedTileHit != null)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            hoveredOverlayTile = focusedTileHit[0].collider.gameObject.GetComponent<OverlayTile>();
            transform.position = hoveredOverlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = hoveredOverlayTile.GetComponent<SpriteRenderer>().sortingOrder;

            SetHoveredTerrainDetails(hoveredOverlayTile);
            if (focusedTileHit.Length == 2)
                SetHoveredUnitDetails();

            if (turnManager.teamPlaying == 1 || turnManager.ia == null)
            {
                if (selectedCharacter != null && selectedCharacter.isMoving) { }
                else
                {
                    LeftButton();

                    GridHUDDisplayer.Instance.DisplayArrowPath(selectedCharacter, hoveredOverlayTile);

                    RightButton();
                }
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }

    public void LeftButton()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (focusedTileHit.Length == 2)
            {
                clickedCharacter = focusedTileHit[1].collider.gameObject.GetComponent<CharacterInfo>();

                if (clickedCharacter.isFromCurrentPlayingTeam)
                {
                    if (selectedCharacter != null)
                        selectedCharacter.OnDeselected();
                    selectedCharacter = clickedCharacter;
                    selectedCharacter.OnSelected();
                    List<Action> asd = selectedCharacter.GenerateAllPosibleActions();
                }
                else
                {
                    if (GridHUDDisplayer.Instance.inRangeOfAttackTiles.Contains(hoveredOverlayTile))
                        selectedCharacter.AttackEnemy(clickedCharacter);
                }
            }
            else if (focusedTileHit.Length < 2 && selectedCharacter != null)
            {
                if (GridHUDDisplayer.Instance.path.Contains(hoveredOverlayTile))
                    if (GridHUDDisplayer.Instance.path.Count > 0)
                    {
                        selectedCharacter.MoveAlongPath(GridHUDDisplayer.Instance.path);
                    }
            }
        }
    }

    public void RightButton()
    {
        if (Input.GetMouseButton(1))
        {
            if (selectedCharacter != null)
            {
                selectedCharacter.OnDeselected();
                selectedCharacter = null;
            }
            if (focusedTileHit.Length == 2)
            {
                clickedCharacter = focusedTileHit[1].collider.gameObject.GetComponent<CharacterInfo>();
                if (clickedCharacter != null && !clickedCharacter.isFromCurrentPlayingTeam)
                {

                    if ((pressedCharacter != null && clickedCharacter != pressedCharacter))
                    {
                        pressedCharacter.OnMouseUp();
                    }
                    if (pressedCharacter == null)
                    {
                        pressedCharacter = clickedCharacter;
                    }
                    clickedCharacter.OnMouseHold();
                }
            }
            else
            {
                if (pressedCharacter != null)
                {
                    pressedCharacter.OnMouseUp();
                    pressedCharacter = null;
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (focusedTileHit.Length == 2)
            {
                if (!clickedCharacter.isFromCurrentPlayingTeam)
                {
                    pressedCharacter = null;
                    clickedCharacter.OnMouseUp();
                }
            }
        }
    }

    public void SetHoveredTerrainDetails(OverlayTile hoveredOverlayTile)
    {
        terrainImage.sprite = hoveredOverlayTile.spriteFromTileBelow;
        terrainMovCost.text = "Mov. cost: " + hoveredOverlayTile.tileData.terrainCost;
    }

    public void SetHoveredUnitDetails()
    {
        CharacterInfo character = focusedTileHit[1].collider.gameObject.GetComponent<CharacterInfo>();
        if (character != null)
        {
            unitImage.sprite = character.sprites[0];
            unitDamage.text = "Damage: " + character.damage;
            unitMovement.text = "Mov.: " + character.movementRange;
            unitAttack.text = "Attack range: " + character.attackRange;
        }
    }

    public RaycastHit2D[] GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);
        if (hits.Length > 0)
        {
            return hits.Reverse().ToArray();
        }
        return null;
    }
}