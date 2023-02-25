using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLMCTSNGUB : DLMCTSNG
{
    public double percentageOfAgresiveInSimulations;

    public override double DefaultPolicy(GameState currentState)
    {
        GameState simulatedNode = CopyGameState(currentState);
        int simulationsDoneSoFar = 0;
        int branchSimulationsDoneSoFar = 0;
        double stateReward;
        double accumulativeReward = 0;
        double dmgDoneBU = simulatedNode.dmgBasedReward;

        Dictionary<Unit, int> acumulatedDmgOnUnit = new Dictionary<Unit, int>();

        while (branchSimulationsDoneSoFar < maxAmountOfBranchSimulated)
        {
            while (simulationsDoneSoFar < maxAmountOfTurnSimulated && !IsTerminal(simulatedNode, acumulatedDmgOnUnit))
            {
                if (branchSimulationsDoneSoFar < (maxAmountOfBranchSimulated * percentageOfAgresiveInSimulations))
                {
                    SimulateAgresiveTurn(simulatedNode, IAMCTSController.playerNumber, acumulatedDmgOnUnit);
                }
                else
                {
                    SimulateTurn(simulatedNode, IAMCTSController.playerNumber, acumulatedDmgOnUnit);
                }
                simulationsDoneSoFar++;
            }
            simulationsDoneSoFar = 0;
            stateReward = RewardForState(simulatedNode, acumulatedDmgOnUnit);

            accumulativeReward += stateReward;

            acumulatedDmgOnUnit.Clear();
            simulatedNode.dmgBasedReward = dmgDoneBU;
            int index = 0;
            while (index < currentState.MyTeam.Count)
            {
                simulatedNode.MyTeam[index].standingOnTile = currentState.MyTeam[index].standingOnTile;
                simulatedNode.MyTeam[index].moved = currentState.MyTeam[index].moved;
                simulatedNode.MyTeam[index].attacked = currentState.MyTeam[index].attacked;
                index++;
            }

            index = 0;
            while (index < currentState.EnemyTeam.Count)
            {
                simulatedNode.EnemyTeam[index].standingOnTile = currentState.EnemyTeam[index].standingOnTile;
                simulatedNode.EnemyTeam[index].moved = currentState.EnemyTeam[index].moved;
                simulatedNode.EnemyTeam[index].attacked = currentState.EnemyTeam[index].attacked;
                index++;
            }
            branchSimulationsDoneSoFar++;
        }

        currentState.localReward = (currentState.CalculatePositionReward() * 0.5);

        return accumulativeReward / maxAmountOfBranchSimulated;
    }

    protected void SimulateAgresiveTurn(GameState currentState, int playerNumber, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        List<Unit> unitsThatCanTakeAction = new List<Unit>();
        Unit unit;

        if (currentState.currentPlayer == playerNumber)
        {
            foreach (Unit character in currentState.MyTeam)
            {
                if (dicOfDmgTakenByUnits.ContainsKey(character))
                {
                    if (character.currentHealth > dicOfDmgTakenByUnits[character])
                        unitsThatCanTakeAction.Add(character);
                }
                else unitsThatCanTakeAction.Add(character);
            }
        }
        else
        {
            foreach (Unit character in currentState.EnemyTeam)
            {
                if (dicOfDmgTakenByUnits.ContainsKey(character))
                {
                    if (character.currentHealth > dicOfDmgTakenByUnits[character])
                        unitsThatCanTakeAction.Add(character);
                }
                else unitsThatCanTakeAction.Add(character);
            }
        }

        foreach (Unit character in unitsThatCanTakeAction)
        {
            character.moved = false;
            character.attacked = false;
        }

        int randomUnitIndex;
        List<Action> randomSelectedAction = new List<Action>();
        List<Action> toRemove = new List<Action>();

        while (unitsThatCanTakeAction.Count != 0)
        {
            randomUnitIndex = UnityEngine.Random.Range(0, unitsThatCanTakeAction.Count);
            unit = unitsThatCanTakeAction[randomUnitIndex];

            if (!unit.moved && !unit.attacked)
            {
                if (currentState.currentPlayer == playerNumber)
                    randomSelectedAction.AddRange(unit.SelectMoveAndAttackOrRandomActionMCTS(null, currentState.MyTeam, currentState.EnemyTeam, dicOfDmgTakenByUnits));
                else
                    randomSelectedAction.AddRange(unit.SelectMoveAndAttackOrRandomActionMCTS(null, currentState.EnemyTeam, currentState.MyTeam, dicOfDmgTakenByUnits));
            }

            if (randomSelectedAction.Count == 0)
                if (currentState.currentPlayer == playerNumber)
                    randomSelectedAction.Add(unit.SelectRandomActionMCTS(null, currentState.MyTeam, currentState.EnemyTeam, 10, dicOfDmgTakenByUnits));
                else
                    randomSelectedAction.Add(unit.SelectRandomActionMCTS(null, currentState.EnemyTeam, currentState.MyTeam, 10, dicOfDmgTakenByUnits));

            foreach (Action action in randomSelectedAction)
            {
                if (action != null)
                {
                    action.Simulate(currentState, IAMCTSController.playerNumber, dicOfDmgTakenByUnits);
                }
            }

            randomSelectedAction.Clear();

            if (!unit.CanTakeAction())
                unitsThatCanTakeAction.Remove(unit);
        }

        if (currentState.currentPlayer == 1)
        {
            currentState.currentPlayer = 2;
        }
        else if (currentState.currentPlayer == 2)
        {
            currentState.currentPlayer = 1;
        }
    }
    protected override TreeNode<GameState> ChildWithBestReward(TreeNode<GameState> parentNode)
    {
        TreeNode<GameState> bestChild = null;
        double reward = 0;
        double highestReward = -10000;
        foreach (TreeNode<GameState> treeNode in parentNode.Children)
        {
            reward = treeNode.Reward + treeNode.Data.localReward;
            if (reward > highestReward)
            {
                bestChild = treeNode;
                highestReward = reward;
            }
        }

        return bestChild;
    }
}