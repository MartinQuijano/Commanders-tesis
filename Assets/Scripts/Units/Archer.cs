using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public override void MoveAlongPath(List<OverlayTile> path)
    {
        isMoving = true;
        moved = true;
        attacked = true;
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
            ChangeSpriteToUnactive();
        }

        currentMovingPath = path;
    }

    public override void MoveAlongPathForAI(List<OverlayTile> path)
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
                ChangeSpriteToUnactive();
            }

        }
        moved = true;
        attacked = true;
        currentMovingPathForIA = path;
    }
    public override void MoveAlongPathForAIMCTS(List<OverlayTile> path, MCTSAI iAMCTSController)
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
                ChangeSpriteToUnactive();
                iAMCTSController.ExecuteNextAction();
            }

        }
        else if (path.Count == 0) { iAMCTSController.ExecuteNextAction(); moved = true; }
        moved = true;
        attacked = true;
        currentMovingPathForIA = path;
    }
    public override void AttackEnemy(Unit enemy)
    {
        Instantiate(attackAnimationPrefab, enemy.transform.position, Quaternion.identity);
        enemy.TakeDamage(damage);
        ChangeSpriteToUnactive();
        GridHUDDisplayer.Instance.ClearAttackTilesInRangeDisplayed();
        attacked = true;
        moved = true;
    }

    public override void OnMouseHold()
    {
        GridHUDDisplayer.Instance.DisplayMovementAndAttackRangeTiles(standingOnTile, 0, attackRange);
    }

    public override Action SelectRandomActionMCTS(Unit originalCharacter, List<Unit> myTeam, List<Unit> enemyTeam, int movePercentage, Dictionary<Unit, int> dicOfDmgTakenByUnits)
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

        return actionSelected;
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
        attacked = true;

        return actionSelected;
    }

    private Action GenerateMovementAction2(Unit originalCharacter, FullGameState fullGameState)
    {
        Action actionSelected = null;

        List<OverlayTile> allPosibleTilesToMoveTo = rangeFinder.GetTilesInMovementRangeMCTS2(standingOnTile, movementRange, fullGameState);
        List<OverlayTile> positionsBlockedByOwnUnits = new List<OverlayTile>();

        int randomIndexForPosition = Random.Range(0, allPosibleTilesToMoveTo.Count);
        OverlayTile tileToMoveTo = allPosibleTilesToMoveTo[randomIndexForPosition];

        actionSelected = new Movement(this, originalCharacter, standingOnTile.grid2DLocation, tileToMoveTo.grid2DLocation);
        moved = true;
        attacked = true;

        return actionSelected;
    }

    public override List<Action> SelectMoveAndAttackOrRandomActionMCTS(Unit originalCharacter, List<Unit> myTeam, List<Unit> enemyTeam, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        List<Action> actionsSelected = new List<Action>();
        Action action = GenerateAttackAction(originalCharacter, enemyTeam, dicOfDmgTakenByUnits);
        if (action != null)
            actionsSelected.Add(action);

        return actionsSelected;
    }

    public override string GetTypeOfUnit()
    {
        return "Archer";
    }

    public override void SimulateMovementNAG()
    {
        moved = true;
        attacked = true;
    }

    public override int GetAmountOfUnitsExclusivelyInMoveAndAttackRange(List<Unit> myTeam, List<Unit> enemyTeam)
    {
        return 0;
    }

    public override int GetProximityScoreToMyTeam(List<Unit> myTeam)
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
                        score = score + 0;
                        break;
                    case 2:
                        score = score + 0;
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    default:
                        score = score - 1;
                        break;
                }
            }
        }

        return score;
    }

    public override double GetScoreModifier()
    {
        return 1.5f;
    }
}
