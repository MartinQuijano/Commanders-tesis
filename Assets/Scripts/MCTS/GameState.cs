using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    private List<Unit> myTeam;
    private List<Unit> enemyTeam;
    private List<Action> takenActionsToThisState;
    public int currentPlayer;
    public double localReward;
    public RangeFinder rangeFinder;

    public double dmgBasedReward;

    public double ScoreForEnemiesInRangeOfAttackValue;
    public double ScoreForUnitsThatCanBeKilledValue;
    public double ScoreForEnemiesThatCanEnterInRangeOfAttackValue;
    public double ScoreForEnemiesThatCanAttackValue;


    public GameState()
    {
        myTeam = new List<Unit>();
        enemyTeam = new List<Unit>();
        takenActionsToThisState = new List<Action>();
        currentPlayer = 2;
        localReward = 0;
        dmgBasedReward = 0;
        rangeFinder = new RangeFinder();
    }

    public List<Action> TakenActionsToThisState { get { return takenActionsToThisState; } set { takenActionsToThisState = value; } }
    public List<Unit> MyTeam { get { return myTeam; } }
    public List<Unit> EnemyTeam { get { return enemyTeam; } }

    public double CalculatePositionReward()
    {
        //return ScoreForEnemiesInRangeOfAttack() + ScoreForEnemiesThatCanAttack() + ScoreForTeamCloseness() + ScoreForUnitsThatCanBeKilled();
        return ScoreForEnemiesInRangeOfAttack() + ScoreForEnemiesThatCanAttack() + ScoreForUnitsThatCanBeKilled() + ScoreForEnemiesThatCanEnterInRangeOfAttack();
        //return 0;
    }

    public double ScoreForEnemiesInRangeOfAttack()
    {
        double score = 0;

        foreach (Unit unit in myTeam)
        {
            // score = score + (unit.GetAmountOfUnitsInAttackRange(enemyTeam) * 2);
            // score = score + (unit.GetAmountOfUnitsExclusivelyInMoveAndAttackRange(myTeam, enemyTeam));
            score = score + (unit.GetAmountOfUnitsInAttackRange(enemyTeam));
            score = score + (unit.GetAmountOfUnitsExclusivelyInMoveAndAttackRange(myTeam, enemyTeam) * 0.5);
        }
        //Debug.Log("ScoreForEnemiesInRangeOfAttack: " + score);
        ScoreForEnemiesInRangeOfAttackValue = score;

        return score;
    }

    public double ScoreForUnitsThatCanBeKilled()
    {
        int score = 0;

        foreach (Unit unit in myTeam)
        {
            if (unit.CanBeKilled(myTeam, enemyTeam))
                //score = score - 2;
                score = score - 1;
        }
        //Debug.Log("ScoreForUnitsThatCanBeKilled: " + score);
        ScoreForUnitsThatCanBeKilledValue = score;

        return score;
    }

    public double ScoreForEnemiesThatCanEnterInRangeOfAttack()
    {
        double score = 0;

        foreach (Unit unit in myTeam)
        {
            score += (unit.AmountOfTilesInRangeOfAttackWhereEnemiesCanMove(myTeam, enemyTeam) * 0.10);
        }

        //Debug.Log("ScoreForEnemiesThatCanEnterInRangeOfAttack: " + score);
        ScoreForEnemiesThatCanEnterInRangeOfAttackValue = score;

        return score;
    }

    public double ScoreForEnemiesThatCanAttack()
    {
        double score = 0;

        foreach (Unit unit in enemyTeam)
        {
            // score = score - (unit.GetAmountOfUnitsInAttackRange(myTeam) * 2);
            // score = score - (unit.GetAmountOfUnitsExclusivelyInMoveAndAttackRange(enemyTeam, myTeam));
            score = score - (unit.GetAmountOfUnitsInAttackRange(myTeam));
            score = score - (unit.GetAmountOfUnitsExclusivelyInMoveAndAttackRange(enemyTeam, myTeam) * 0.5);
        }
        //Debug.Log("ScoreForEnemiesThatCanAttack: " + score);
        ScoreForEnemiesThatCanAttackValue = score;

        return score;
    }

    public int ScoreForTeamCloseness()
    {
        int score = 0;

        foreach (Unit unit in myTeam)
        {
            score = score + unit.GetProximityScoreToMyTeam(myTeam);
        }
        //     Debug.Log("ScoreForTeamCloseness: " + score);

        return score;
    }

    public int ScoreForEnemiesThatCanKill()
    {
        int score = 0;

        foreach (Unit unit in enemyTeam)
        {
            score = score - (unit.GetAmountOfUnitsThatCanKill(enemyTeam, myTeam) * 3);
            //      score = score - (unit.GetAmountOfUnitsExclusivelyInMoveAndAttackRange(myTeam, enemyTeam));
        }
        //      Debug.Log("ScoreForEnemiesThatCanKillMe: " + score);

        return score;
    }
}
