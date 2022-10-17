using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : CharacterInfo
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

    public override void MoveAlongPathForIATest(List<OverlayTile> path)
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
    public override void MoveAlongPathForIATest2(List<OverlayTile> path, IAMCTSController iAMCTSController)
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
    public override void AttackEnemy(CharacterInfo enemy)
    {
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

    public override Action SelectRandomActionMCTS(CharacterInfo originalCharacter, List<CharacterInfo> myTeam, List<CharacterInfo> enemyTeam)
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
        attacked = true;

        return actionSelected;
    }
}
