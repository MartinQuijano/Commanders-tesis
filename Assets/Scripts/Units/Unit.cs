using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Unit : MonoBehaviour
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
    public List<OverlayTile> currentMovingPathForIASim;
    public TeamManager myTeam;

    public TextMeshPro healthpoints;

    public RangeFinder rangeFinder;

    public MCTSAI iAMCTSController;
    public Simulator simulator;

    public int numericLogValue;

    public GameObject attackAnimationPrefab;

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
        if (currentMovingPathForIA.Count > 0 && iAMCTSController == null)
            MoveAlongPathForAI(currentMovingPathForIA);
        if (currentMovingPathForIA.Count > 0 && iAMCTSController != null)
            MoveAlongPathForAIMCTS(currentMovingPathForIA, iAMCTSController);
        if (currentMovingPathForIASim.Count > 0)
            MoveAlongPathForAISim(currentMovingPathForIASim, simulator);
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

    public virtual void MoveAlongPathForAI(List<OverlayTile> path)
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

    public virtual void MoveAlongPathForAIMCTS(List<OverlayTile> path, MCTSAI iAMCTSController)
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
        if (path.Count == 0)
        {
            iAMCTSController.ExecuteNextAction();
            moved = true;
        }

        currentMovingPathForIA = path;
    }

    public virtual void MoveAlongPathForAISim(List<OverlayTile> path, Simulator simulator)
    {
        this.simulator = simulator;
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
                simulator.ExecuteNextAction();
                moved = true;
            }
        }
        else
        if (path.Count == 0)
        {
            simulator.ExecuteNextAction();
            moved = true;
        }

        currentMovingPathForIASim = path;
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

    public virtual void AttackEnemy(Unit enemy)
    {
        Instantiate(attackAnimationPrefab, enemy.transform.position, Quaternion.identity);
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

    public bool CanTakeAction()
    {
        return (!moved || !attacked);
    }

    public virtual Action SelectRandomActionMCTS(Unit originalCharacter, List<Unit> myTeam, List<Unit> enemyTeam, int movePercentage, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        Action actionSelected = null;

        if (!moved && !attacked)
        {
            int randomNumber = Random.Range(0, 100);

            if (randomNumber < movePercentage)
            {
                actionSelected = GenerateMovementAction(originalCharacter, myTeam, enemyTeam);
            }
            else
            {
                actionSelected = GenerateAttackAction(originalCharacter, enemyTeam, dicOfDmgTakenByUnits);
                if (actionSelected == null)
                    actionSelected = GenerateMovementAction(originalCharacter, myTeam, enemyTeam);
            }
        }
        else if (moved && !attacked)
        {
            actionSelected = GenerateAttackAction(originalCharacter, enemyTeam, dicOfDmgTakenByUnits);
            attacked = true;
        }

        return actionSelected;
    }

    public virtual List<Action> SelectMoveAndAttackOrRandomActionMCTS(Unit originalCharacter, List<Unit> myTeam, List<Unit> enemyTeam, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        List<Action> actionsSelected = new List<Action>();

        actionsSelected.AddRange(GenerateMoveAndAttackAction(originalCharacter, myTeam, enemyTeam, dicOfDmgTakenByUnits));

        //  Debug.Log("En SelectMoveAndAttackOrRandomActionMCTS se generaron: " + actionsSelected.Count + " acciones.");
        return actionsSelected;
    }

    protected virtual List<Action> GenerateMoveAndAttackAction(Unit originalCharacter, List<Unit> myTeam, List<Unit> enemyTeam, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        List<Action> actionsSelected = new List<Action>();
        //      Debug.Log("GenerateMoveAndAttackAction---------");
        // Buscar el tile del enemigo mas cercano
        OverlayTile tileOfNearestEnemy = FindTileOfNearestEnemy(standingOnTile, enemyTeam);
        //        Debug.Log("tileOfNearestEnemy: " + tileOfNearestEnemy);
        if (tileOfNearestEnemy != null)
        {
            // A partir de ese tile y con el rango de ataque esta unidad, calcular todos los tiles cercanos en ese rango desde el cual la unidad puede atacar al enemigo
            List<OverlayTile> tilesWhereICanAttackTheEnemy;
            RangeFinder rangeFinder = new RangeFinder();
            tilesWhereICanAttackTheEnemy = rangeFinder.GetTilesInAttackRange(tileOfNearestEnemy, attackRange);
            // Eliminar los tiles que estan ocupados por otras unidades
            List<OverlayTile> tilesOccupied = new List<OverlayTile>();

            foreach (OverlayTile tile in tilesWhereICanAttackTheEnemy)
            {
                if (ThereIsCharacterOnTileMCTS(tile, myTeam, enemyTeam))
                    tilesOccupied.Add(tile);
            }

            foreach (OverlayTile tile in tilesOccupied)
            {
                tilesWhereICanAttackTheEnemy.Remove(tile);
            }
            // Buscar los tiles a los que se puede mover esta unidad
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
            // Armar una lista con los tiles a los que se puede mover que coinciden con los tiles obtenidos anteriormente
            List<OverlayTile> tilesWhereICanMoveToAttack = new List<OverlayTile>();
            int index;
            bool tileMatches = false;
            foreach (OverlayTile tile in tilesWhereICanAttackTheEnemy)
            {
                index = 0;
                while (!tileMatches && index < allPosibleTilesToMoveTo.Count)
                {
                    if (tile == allPosibleTilesToMoveTo[index])
                    {
                        tileMatches = true;
                        tilesWhereICanMoveToAttack.Add(tile);
                    }
                    index++;
                }
                tileMatches = false;
            }
            //      Debug.Log("tilesWhereICanMoveToAttack.Count: " + tilesWhereICanMoveToAttack.Count);
            if (tilesWhereICanMoveToAttack.Count > 0)
            {
                // Seleccionar aleatoriamente un tile de esa lista
                int randomIndexForPosition = Random.Range(0, tilesWhereICanMoveToAttack.Count);
                OverlayTile tileToMoveTo = tilesWhereICanMoveToAttack[randomIndexForPosition];
                // Generar accion de movimiento hacia ese tile
                actionsSelected.Add(new Movement(this, originalCharacter, standingOnTile.grid2DLocation, tileToMoveTo.grid2DLocation));
                // Generar accion de ataque desde ese tile hasta el tile del enemigo mas cercano obtenido inicialmente
                actionsSelected.Add(new Attack(this, originalCharacter, tileToMoveTo.grid2DLocation, tileOfNearestEnemy.grid2DLocation));

                attacked = true;
                moved = true;
                //    Debug.Log(this + " se movio y ataco!");
            }
        }
        //    Debug.Log("Para la unidad: " + this + " GenerateMoveAndAttackAction genero " + actionsSelected.Count + " acciones!");
        return actionsSelected;
    }

    private OverlayTile FindTileOfNearestEnemy(OverlayTile startingTile, List<Unit> enemyTeam)
    {
        bool enemyFound = false;
        Queue<OverlayTile> tilesToSearch = new Queue<OverlayTile>();
        tilesToSearch.Enqueue(startingTile);
        OverlayTile currentTile;
        OverlayTile tileOfEnemyFound = null;
        int index;

        while (tilesToSearch.Count > 0 && !enemyFound)
        {
            currentTile = tilesToSearch.Dequeue();
            currentTile.visited = true;
            index = 0;
            while (!enemyFound && index < enemyTeam.Count)
            {
                if (currentTile == enemyTeam[index].standingOnTile)
                {
                    enemyFound = true;
                    tileOfEnemyFound = currentTile;
                }
                index++;
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

    private Action GenerateMovementAction(Unit originalCharacter, List<Unit> myTeam, List<Unit> enemyTeam)
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

    protected Action GenerateAttackAction(Unit originalCharacter, List<Unit> enemyTeam, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        Action actionSelected = null;

        List<OverlayTile> allPosibleTilesToAttackTo = rangeFinder.GetTilesInAttackRange(standingOnTile, attackRange);
        List<OverlayTile> tilesWhereThereAreEnemiesToAttack = new List<OverlayTile>();

        foreach (OverlayTile tile in allPosibleTilesToAttackTo)
        {
            bool thereIsEnemyCharacter = false;

            int index = 0;

            while (!thereIsEnemyCharacter && index < enemyTeam.Count)
            {
                //           Debug.Log("Enemy team unit: " + enemyTeam[index] + " en la posicion: " + enemyTeam[index].standingOnTile.grid2DLocation);
                if (tile == enemyTeam[index].standingOnTile)
                {
                    if (dicOfDmgTakenByUnits.ContainsKey(enemyTeam[index]))
                    {
                        if (enemyTeam[index].currentHealth > dicOfDmgTakenByUnits[enemyTeam[index]])
                            tilesWhereThereAreEnemiesToAttack.Add(tile);
                    }
                    else tilesWhereThereAreEnemiesToAttack.Add(tile);
                }
                index++;
            }
        }

        if (tilesWhereThereAreEnemiesToAttack.Count > 0)
        {
            //               Debug.Log("Tile with enemy: " + tilesWhereThereAreEnemiesToAttack[0].grid2DLocation);
            int randomIndexForPosition = Random.Range(0, tilesWhereThereAreEnemiesToAttack.Count);
            OverlayTile tileToAttackTo = tilesWhereThereAreEnemiesToAttack[randomIndexForPosition];

            actionSelected = new Attack(this, originalCharacter, standingOnTile.grid2DLocation, tileToAttackTo.grid2DLocation);
            attacked = true;
            moved = true;
        }
        /* if (actionSelected != null)
             Debug.Log("En GenerateAttackAction se genero la accion " + actionSelected.GetString());
         else Debug.Log("En GenerateAttackAction se genero la accion " + actionSelected);*/
        return actionSelected;
    }

    protected bool ThereIsCharacterOnTileMCTS(OverlayTile tile, List<Unit> myTeam, List<Unit> enemyTeam)
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

    public bool ThereIsEnemyCharacterOnTileMCTS(OverlayTile tile, List<Unit> enemyTeam)
    {
        bool thereIsEnemyCharacter = false;

        int index = 0;

        while (!thereIsEnemyCharacter && index < enemyTeam.Count)
        {
            //           Debug.Log("Enemy team unit: " + enemyTeam[index] + " en la posicion: " + enemyTeam[index].standingOnTile.grid2DLocation);
            if (tile == enemyTeam[index].standingOnTile)
                thereIsEnemyCharacter = true;
            index++;
        }
        return thereIsEnemyCharacter;
    }

    public abstract string GetTypeOfUnit();

    public virtual void SimulateAttackNAG()
    {
        attacked = true;
        moved = true;
    }

    public virtual void SimulateMovementNAG()
    {
        moved = true;
    }

    public virtual int GetAmountOfUnitsInAttackRange(List<Unit> enemyTeam)
    {
        int unitsInAttackRange = 0;

        List<OverlayTile> allPosibleTilesToAttackTo = rangeFinder.GetTilesInAttackRange(standingOnTile, attackRange);

        foreach (OverlayTile tile in allPosibleTilesToAttackTo)
        {
            if (ThereIsEnemyCharacterOnTileMCTS(tile, enemyTeam))
                unitsInAttackRange++;
        }

        return unitsInAttackRange;
    }

    public virtual int GetAmountOfUnitsThatCanKill(List<Unit> myTeam, List<Unit> enemyTeam)
    {
        int unitsThatCanKillMe = 0;
        bool thereIsEnemyCharacter = false;
        int index = 0;

        List<OverlayTile> allPosibleTilesToMoveTo = rangeFinder.GetTilesInMovementRangeMCTS(standingOnTile, movementRange, myTeam, enemyTeam);
        List<OverlayTile> positionsBlockedByOwnUnits = new List<OverlayTile>();
        List<OverlayTile> tilesAlreadyConsidered = new List<OverlayTile>();
        List<OverlayTile> allPosibleTilesToAttackTo;

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

        foreach (OverlayTile tile in allPosibleTilesToMoveTo)
        {

            allPosibleTilesToAttackTo = rangeFinder.GetTilesInAttackRange(tile, attackRange);


            foreach (OverlayTile tileInAttackRange in allPosibleTilesToAttackTo)
            {
                index = 0;
                thereIsEnemyCharacter = false;

                while (!thereIsEnemyCharacter && index < enemyTeam.Count)
                {
                    if (tileInAttackRange == enemyTeam[index].standingOnTile)
                    {
                        thereIsEnemyCharacter = true;
                    }
                    index++;
                }

                if (thereIsEnemyCharacter && EnemyAtTileCanKillMe(enemyTeam[index - 1]))
                    unitsThatCanKillMe++;
            }
        }

        return unitsThatCanKillMe;
    }

    public bool EnemyAtTileCanKillMe(Unit unit)
    {
        bool canKillMe = false;

        if (damage >= unit.currentHealth)
        {
            canKillMe = true;
        }

        return canKillMe;
    }

    public virtual int GetAmountOfUnitsExclusivelyInMoveAndAttackRange(List<Unit> myTeam, List<Unit> enemyTeam)
    {
        int unitsInAttackRange = 0;

        List<OverlayTile> allPosibleTilesToMoveTo = rangeFinder.GetTilesInMovementRangeMCTS(standingOnTile, movementRange, myTeam, enemyTeam);
        List<OverlayTile> positionsBlockedByOwnUnits = new List<OverlayTile>();
        List<OverlayTile> tilesAlreadyConsidered = new List<OverlayTile>();
        List<OverlayTile> allPosibleTilesToAttackTo;

        List<OverlayTile> allPosibleTilesToAttackToPreMove = rangeFinder.GetTilesInAttackRange(standingOnTile, attackRange);

        foreach (OverlayTile tile in allPosibleTilesToAttackToPreMove)
        {
            if (ThereIsEnemyCharacterOnTileMCTS(tile, enemyTeam))
                tilesAlreadyConsidered.Add(tile);
        }

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

        foreach (OverlayTile tile in allPosibleTilesToMoveTo)
        {
            allPosibleTilesToAttackTo = rangeFinder.GetTilesInAttackRange(tile, attackRange);
            foreach (OverlayTile tileInAttackRange in allPosibleTilesToAttackTo)
            {
                if (ThereIsEnemyCharacterOnTileMCTS(tileInAttackRange, enemyTeam) && !tilesAlreadyConsidered.Contains(tileInAttackRange))
                {
                    tilesAlreadyConsidered.Add(tileInAttackRange);
                    unitsInAttackRange++;
                }
            }
        }

        return unitsInAttackRange;
    }

    public virtual int GetProximityScoreToMyTeam(List<Unit> myTeam)
    {
        int score = 0;
        int distanceToUnit;

        foreach (Unit unit in myTeam)
        {
            if (standingOnTile != unit.standingOnTile)
            {
                distanceToUnit = GetManhattanDistance(standingOnTile, unit.standingOnTile);
                switch (distanceToUnit)
                {
                    case 1:
                        score = score + 1;
                        break;
                    case 2:
                        score = score + 1;
                        break;
                    case 3:
                        break;
                    default:
                        score = score - 1;
                        break;
                }
            }
        }

        return score;
    }

    public bool CanBeKilled(List<Unit> myTeam, List<Unit> enemyTeam)
    {
        bool canBeKilled = false;
        int index = 0;

        while (!canBeKilled && index < enemyTeam.Count)
        {
            if (CanThisUnitKillMe(myTeam, enemyTeam, enemyTeam[index]))
                canBeKilled = true;
            index++;
        }

        return canBeKilled;
    }

    public bool CanThisUnitKillMe(List<Unit> myTeam, List<Unit> enemyTeam, Unit unit)
    {
        bool canKillMe = false;

        List<OverlayTile> allPosibleTilesToMoveTo = rangeFinder.GetTilesInMovementRangeMCTS(unit.standingOnTile, unit.movementRange, myTeam, enemyTeam);
        List<OverlayTile> positionsBlockedByOwnUnits = new List<OverlayTile>();
        List<OverlayTile> tilesAlreadyConsidered = new List<OverlayTile>();
        List<OverlayTile> allPosibleTilesToAttackTo;

        foreach (OverlayTile tile in allPosibleTilesToMoveTo)
        {
            if (tile != standingOnTile && ThereIsCharacterOnTileMCTS(tile, myTeam, enemyTeam))
            {
                positionsBlockedByOwnUnits.Add(tile);
            }
        }

        foreach (OverlayTile tile in positionsBlockedByOwnUnits)
        {
            if (unit.standingOnTile != tile)
                allPosibleTilesToMoveTo.Remove(tile);
        }

        int indexOut = 0;
        int indexIn = 0;

        while (!canKillMe && indexOut < allPosibleTilesToMoveTo.Count)
        {
            allPosibleTilesToAttackTo = rangeFinder.GetTilesInAttackRange(allPosibleTilesToMoveTo[indexOut], attackRange);

            while (!canKillMe && indexIn < allPosibleTilesToAttackTo.Count)
            {
                if (allPosibleTilesToAttackTo[indexIn] == standingOnTile && unit.damage >= currentHealth)
                    canKillMe = true;
                indexIn++;
            }
            indexOut++;
            indexIn = 0;
        }


        return canKillMe;
    }

    protected int GetManhattanDistance(OverlayTile start, OverlayTile end)
    {
        return Mathf.Abs(start.gridLocation.x - end.gridLocation.x) + Mathf.Abs(start.gridLocation.y - end.gridLocation.y);
    }

    public virtual int AmountOfTilesInRangeOfAttackWhereEnemiesCanMove(List<Unit> myTeam, List<Unit> enemyTeam)
    {
        int amountOfTiles = 0;

        List<OverlayTile> tilesInRangeOfAttack = rangeFinder.GetTilesInAttackRange(standingOnTile, attackRange);

        foreach (OverlayTile tile in tilesInRangeOfAttack)
        {
            if (ThereIsEnemyThatCanMoveToThisTile(tile, myTeam, enemyTeam))
            {
                amountOfTiles++;
            }
        }

        return amountOfTiles;
    }

    public bool ThereIsEnemyThatCanMoveToThisTile(OverlayTile tile, List<Unit> myTeam, List<Unit> enemyTeam)
    {
        bool canAnEnemyMoveToTile = false;
        bool tileMatchFound = false;
        int index = 0;
        int indexIn = 0;
        List<OverlayTile> tilesInRangeOfMovement;

        while (!canAnEnemyMoveToTile && index < enemyTeam.Count)
        {
            tilesInRangeOfMovement = rangeFinder.GetTilesInMovementRangeMCTS(enemyTeam[index].standingOnTile, enemyTeam[index].movementRange, enemyTeam, myTeam);
            tilesInRangeOfMovement.Remove(enemyTeam[index].standingOnTile);
            indexIn = 0;
            while (!tileMatchFound && indexIn < tilesInRangeOfMovement.Count)
            {
                if (tile == tilesInRangeOfMovement[indexIn])
                {
                    tileMatchFound = true;
                    canAnEnemyMoveToTile = true;
                }
                indexIn++;
            }

            index++;
        }

        return canAnEnemyMoveToTile;
    }

    public virtual double GetScoreModifier()
    {
        return 1;
    }
}