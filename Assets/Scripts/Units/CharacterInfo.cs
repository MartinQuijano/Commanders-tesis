using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterInfo : MonoBehaviour
{
    public OverlayTile standingOnTile;
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;
    public bool isFromCurrentPlayingTeam;

    public float speed;

    public int max_health;
    public int currentHealth;
    public int damage;

    public bool selected;
    public bool isMoving;

    public bool moved;
    public bool attacked;

    public int movementRange;
    public int attackRange;

    public List<OverlayTile> currentMovingPath;
    public List<OverlayTile> currentMovingPathForIA;
    public TeamManager myTeam;

    public TextMeshPro healthpoints;

    public RangeFinder rangeFinder;

    public IAMCTSController iAMCTSController;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthpoints = GetComponentInChildren<TextMeshPro>();
        rangeFinder = new RangeFinder();
        selected = false;
        isMoving = false;

    }

    void Start()
    {
        moved = false;
        attacked = false;
        currentHealth = max_health;
        healthpoints.text = currentHealth.ToString();
    }

    void Update()
    {
        if (selected)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!moved)
                {
                    moved = true;
                    GridHUDDisplayer.Instance.ClearMovementTilesInRangeDisplayed();
                    GridHUDDisplayer.Instance.DisplayAttackRange(standingOnTile, attackRange);
                }
                else if (!attacked)
                {
                    attacked = true;
                    ChangeSpriteToUnactive();
                    GridHUDDisplayer.Instance.ClearAttackTilesInRangeDisplayed();
                }
            }
        if (currentMovingPath.Count > 0)
            MoveAlongPath(currentMovingPath);
        if (currentMovingPathForIA.Count > 0 && iAMCTSController != null)
            MoveAlongPathForIATest2(currentMovingPathForIA, iAMCTSController);
        if (currentHealth <= 0)
        {
            myTeam.RemoveCharacter(this);
            Destroy(this.gameObject);
        }
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0);
        standingOnTile = tile;
    }

    public void SwapIsFromCurrentPlayingTeam()
    {
        isFromCurrentPlayingTeam = !isFromCurrentPlayingTeam;
    }

    public void ChangeSpriteToActive()
    {
        spriteRenderer.sprite = sprites[0];
    }

    public void ChangeSpriteToUnactive()
    {
        spriteRenderer.sprite = sprites[1];
    }

    public virtual void MoveAlongPath(List<OverlayTile> path)
    {
        isMoving = true;
        moved = true;
        var step = speed * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, step);

        if (Vector2.Distance(transform.position, path[0].transform.position) < 0.0001f)
        {
            if (path.Count == 1)
            {
                standingOnTile.characterOnTile = null;
                PositionCharacterOnTile(path[0]);
                path[0].SetCharacterStandingOnTile(this);
            }
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            GridHUDDisplayer.Instance.ClearMovementTilesInRangeDisplayed();
            GridHUDDisplayer.Instance.DisplayAttackRange(standingOnTile, attackRange);

            isMoving = false;
        }

        currentMovingPath = path;
    }

    public virtual void MoveAlongPathForIATest(List<OverlayTile> path)
    {
        if (path.Count > 0)
        {
            isMoving = true;
            var step = speed * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, step);

            if (Vector2.Distance(transform.position, path[0].transform.position) < 0.0001f)
            {
                if (path.Count == 1)
                {
                    standingOnTile.characterOnTile = null;
                    PositionCharacterOnTile(path[0]);
                    path[0].SetCharacterStandingOnTile(this);
                }
                path.RemoveAt(0);
            }

            if (path.Count == 0)
            {
                GridHUDDisplayer.Instance.ClearMovementTilesInRangeDisplayed();

                isMoving = false;
            }

            currentMovingPathForIA = path;
        }
        moved = true;
    }

    public virtual void MoveAlongPathForIATest2(List<OverlayTile> path, IAMCTSController iAMCTSController)
    {
        this.iAMCTSController = iAMCTSController;
        if (path.Count > 0)
        {
            isMoving = true;
            var step = speed * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, step);

            if (Vector2.Distance(transform.position, path[0].transform.position) < 0.0001f)
            {
                if (path.Count == 1)
                {
                    standingOnTile.characterOnTile = null;
                    PositionCharacterOnTile(path[0]);
                    path[0].SetCharacterStandingOnTile(this);
                }
                path.RemoveAt(0);
            }

            if (path.Count == 0)
            {
                GridHUDDisplayer.Instance.ClearMovementTilesInRangeDisplayed();

                isMoving = false;
                iAMCTSController.ExecuteNextAction();
                moved = true;
            }


        }
        else
        if (path.Count == 0) { iAMCTSController.ExecuteNextAction(); moved = true; }

        currentMovingPathForIA = path;
    }

    public void OnSelected()
    {
        selected = true;
        if (!moved)
        {
            GridHUDDisplayer.Instance.DisplayMovementRange(standingOnTile, movementRange);
        }
        else if (!attacked)
        {
            GridHUDDisplayer.Instance.DisplayAttackRange(standingOnTile, attackRange);
        }
    }

    public void OnDeselected()
    {
        GridHUDDisplayer.Instance.ClearMovementTilesInRangeDisplayed();
        GridHUDDisplayer.Instance.ClearAttackTilesInRangeDisplayed();
        GridHUDDisplayer.Instance.ClearPath();
        selected = false;
    }

    public virtual void AttackEnemy(CharacterInfo enemy)
    {
        enemy.TakeDamage(damage);
        attacked = true;
        ChangeSpriteToUnactive();
        GridHUDDisplayer.Instance.ClearAttackTilesInRangeDisplayed();
    }

    public virtual void OnMouseHold()
    {
        GridHUDDisplayer.Instance.DisplayMovementAndAttackRangeTiles(standingOnTile, movementRange, attackRange);
    }

    public void OnMouseUp()
    {
        GridHUDDisplayer.Instance.ClearEnemyMovementAndAttackTilesInRangeDisplayed();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthpoints.text = currentHealth.ToString();
    }

    public void SetMyTeam(TeamManager team)
    {
        myTeam = team;
    }

    public List<Action> GenerateAllPosibleActions()
    {
        List<Action> allPosibleActions = new List<Action>();

        List<OverlayTile> allPosibleTilesToMoveTo = rangeFinder.GetTilesInMovementRange(standingOnTile, movementRange);
        List<OverlayTile> positionsBlockedByOwnUnits = new List<OverlayTile>();

        if (!moved)
        {
            foreach (OverlayTile tile in allPosibleTilesToMoveTo)
            {
                if (tile != standingOnTile && tile.characterOnTile != null)
                {
                    positionsBlockedByOwnUnits.Add(tile);
                }
            }

            foreach (OverlayTile tile in positionsBlockedByOwnUnits)
            {
                allPosibleTilesToMoveTo.Remove(tile);
            }

            //        Debug.Log("Amount of posible moves: " + allPosibleTilesToMoveTo.Count);

            foreach (OverlayTile tileOfMovementInRange in allPosibleTilesToMoveTo)
            {
                //Debug.Log("Tile position: " + tileOfMovementInRange.grid2DLocation);
                allPosibleActions.Add(new Movement(this, null, standingOnTile.grid2DLocation, tileOfMovementInRange.grid2DLocation));
            }
        }

        if (!attacked)
        {
            List<OverlayTile> allPosibleTilesToAttack = rangeFinder.GetTilesInAttackRange(standingOnTile, attackRange);

            foreach (OverlayTile tileOfAttackInRange in allPosibleTilesToAttack)
            {
                if (tileOfAttackInRange.characterOnTile != null && !tileOfAttackInRange.characterOnTile.isFromCurrentPlayingTeam)
                    allPosibleActions.Add(new Attack(this, null, standingOnTile.grid2DLocation, tileOfAttackInRange.grid2DLocation));
            }
        }

        //Debug.Log("Unit: " + gameObject + " with position: " + standingOnTile.grid2DLocation + " Amount of posible actions: " + allPosibleActions.Count);
        return allPosibleActions;
    }

    public bool CanTakeAction()
    {
        return (!moved || !attacked);
    }

    public virtual Action SelectRandomActionMCTS(CharacterInfo originalCharacter, List<CharacterInfo> myTeam, List<CharacterInfo> enemyTeam)
    {
        Action actionSelected = null;

        if (!moved && !attacked)
        {
            int randomNumber = Random.Range(0, 100);

            if (randomNumber < 20)
            {
                actionSelected = GenerateMovementAction(originalCharacter, myTeam, enemyTeam);
            }
            else
            {
                List<OverlayTile> allPosibleTilesToAttackTo = rangeFinder.GetTilesInAttackRange(standingOnTile, attackRange);
                List<OverlayTile> tilesWhereThereAreEnemiesToAttack = new List<OverlayTile>();

                foreach (OverlayTile tile in allPosibleTilesToAttackTo)
                {
                    if (ThereIsEnemyCharacterOnTileMCTS(tile, enemyTeam))
                        tilesWhereThereAreEnemiesToAttack.Add(tile);
                }
                if (tilesWhereThereAreEnemiesToAttack.Count > 0)
                {
                    int randomIndexForPosition = Random.Range(0, tilesWhereThereAreEnemiesToAttack.Count);
                    OverlayTile tileToAttackTo = tilesWhereThereAreEnemiesToAttack[randomIndexForPosition];

                    actionSelected = new Attack(this, originalCharacter, standingOnTile.grid2DLocation, tileToAttackTo.grid2DLocation);
                    attacked = true;
                    moved = true;
                }
                else actionSelected = GenerateMovementAction(originalCharacter, myTeam, enemyTeam);
            }
        }
        else if (moved && !attacked)
        {
            List<OverlayTile> allPosibleTilesToAttackTo = rangeFinder.GetTilesInAttackRange(standingOnTile, attackRange);
            List<OverlayTile> tilesWhereThereAreEnemiesToAttack = new List<OverlayTile>();

            foreach (OverlayTile tile in allPosibleTilesToAttackTo)
            {
                if (ThereIsEnemyCharacterOnTileMCTS(tile, enemyTeam))
                    tilesWhereThereAreEnemiesToAttack.Add(tile);
            }
            if (tilesWhereThereAreEnemiesToAttack.Count > 0)
            {
                int randomIndexForPosition = Random.Range(0, tilesWhereThereAreEnemiesToAttack.Count);
                OverlayTile tileToAttackTo = tilesWhereThereAreEnemiesToAttack[randomIndexForPosition];

                actionSelected = new Attack(this, originalCharacter, standingOnTile.grid2DLocation, tileToAttackTo.grid2DLocation);
            }
            attacked = true;
        }

        return actionSelected;
    }

    private Action GenerateMovementAction(CharacterInfo originalCharacter, List<CharacterInfo> myTeam, List<CharacterInfo> enemyTeam)
    {
        Action actionSelected = null;

        List<OverlayTile> allPosibleTilesToMoveTo = rangeFinder.GetTilesInMovementRangeMCTS(standingOnTile, movementRange, myTeam, enemyTeam);
        List<OverlayTile> positionsBlockedByOwnUnits = new List<OverlayTile>();

        foreach (OverlayTile tile in allPosibleTilesToMoveTo)
        {
            if (tile != standingOnTile && ThereIsCharacterOnTileMCTS(tile, myTeam, enemyTeam))
            {
                positionsBlockedByOwnUnits.Add(tile);
            }
        }

        foreach (OverlayTile tile in positionsBlockedByOwnUnits)
        {
            allPosibleTilesToMoveTo.Remove(tile);
        }
        int randomIndexForPosition = Random.Range(0, allPosibleTilesToMoveTo.Count);
        OverlayTile tileToMoveTo = allPosibleTilesToMoveTo[randomIndexForPosition];

        actionSelected = new Movement(this, originalCharacter, standingOnTile.grid2DLocation, tileToMoveTo.grid2DLocation);
        moved = true;

        return actionSelected;
    }

    protected bool ThereIsCharacterOnTileMCTS(OverlayTile tile, List<CharacterInfo> myTeam, List<CharacterInfo> enemyTeam)
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

    protected bool ThereIsEnemyCharacterOnTileMCTS(OverlayTile tile, List<CharacterInfo> enemyTeam)
    {
        bool thereIsEnemyCharacter = false;

        int index = 0;

        while (!thereIsEnemyCharacter && index < enemyTeam.Count)
        {
            if (tile == enemyTeam[index].standingOnTile)
                thereIsEnemyCharacter = true;
            index++;
        }
        return thereIsEnemyCharacter;
    }
}