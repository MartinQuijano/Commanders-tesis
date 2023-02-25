using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MCTS : MonoBehaviour
{
    public int maxAmountOfTurnSimulated;
    public int maxAmountOfBranchSimulated;
    public TeamManager myTeam;
    public TeamManager enemyTeam;
    public MCTSAI IAMCTSController;
    public GameObject simulatedUnitsContainer;
    public GameObject clonedUnitsContainer;

    public abstract void FindAndExecuteMoves();

    public virtual double DefaultPolicy(GameState currentState)
    {
        GameState simulatedNode = CopyGameState(currentState);
        int simulationsDoneSoFar = 0;
        int branchSimulationsDoneSoFar = 0;
        double stateReward;
        double accumulativeReward = 0;

        Dictionary<Unit, int> acumulatedDmgOnUnit = new Dictionary<Unit, int>();

        //        Debug.Log("-----------------------------------------------------------------------------------------------------------");
        //      Debug.Log("current player: " + currentState.currentPlayer);
        while (branchSimulationsDoneSoFar < maxAmountOfBranchSimulated)
        {
            //         Debug.Log("Branch sim");
            while (simulationsDoneSoFar < maxAmountOfTurnSimulated && !IsTerminal(simulatedNode, acumulatedDmgOnUnit))
            {
                //             Debug.Log("Turno sim");
                SimulateTurn(simulatedNode, IAMCTSController.playerNumber, acumulatedDmgOnUnit);
                simulationsDoneSoFar++;
            }
            simulationsDoneSoFar = 0;
            stateReward = RewardForState(simulatedNode, acumulatedDmgOnUnit);

            accumulativeReward += stateReward;

            acumulatedDmgOnUnit.Clear();
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

        return accumulativeReward / maxAmountOfBranchSimulated;
    }
    public virtual TreeNode<GameState> Expand(TreeNode<GameState> nodeToExpand)
    {
        GameState newState = new GameState();
        List<Unit> unitsThatCanTakeAction = new List<Unit>();
        List<Unit> originalUnitsThatCanTakeAction = new List<Unit>();
        Unit unit;

        if (nodeToExpand.Data.currentPlayer == 1)
            newState.currentPlayer = 1;
        else newState.currentPlayer = 2;

        foreach (Unit character in nodeToExpand.Data.EnemyTeam)
        {
            newState.EnemyTeam.Add(Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, clonedUnitsContainer.transform));
        }

        foreach (Unit character in nodeToExpand.Data.MyTeam)
        {
            unit = Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, clonedUnitsContainer.transform);
            newState.MyTeam.Add(unit);
            unitsThatCanTakeAction.Add(unit);
            originalUnitsThatCanTakeAction.Add(character);
        }

        int randomUnitIndex;
        Action randomSelectedAction;

        while (unitsThatCanTakeAction.Count != 0)
        {
            randomUnitIndex = UnityEngine.Random.Range(0, unitsThatCanTakeAction.Count);
            unit = unitsThatCanTakeAction[randomUnitIndex];
            Unit character = originalUnitsThatCanTakeAction[randomUnitIndex];

            randomSelectedAction = unit.SelectRandomActionMCTS(character, newState.MyTeam, newState.EnemyTeam, 80, new Dictionary<Unit, int>());

            if (randomSelectedAction != null)
            {
                newState.TakenActionsToThisState.Add(randomSelectedAction);
                randomSelectedAction.SimulateExpansion(newState, IAMCTSController.playerNumber);
            }

            if (!unit.CanTakeAction())
            {
                unitsThatCanTakeAction.Remove(unit);
                originalUnitsThatCanTakeAction.Remove(character);
                unit.moved = false;
                unit.attacked = false;
            }
        }

        if (nodeToExpand.Data.currentPlayer == 1)
            newState.currentPlayer = 2;
        else newState.currentPlayer = 1;

        return nodeToExpand.AddChild(newState);
    }
    protected void SimulateTurn(GameState currentState, int playerNumber, Dictionary<Unit, int> dicOfDmgTakenByUnits)
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
        Action randomSelectedAction = null;

        while (unitsThatCanTakeAction.Count != 0)
        {
            randomUnitIndex = UnityEngine.Random.Range(0, unitsThatCanTakeAction.Count);
            unit = unitsThatCanTakeAction[randomUnitIndex];
            if (currentState.currentPlayer == playerNumber)
                randomSelectedAction = unit.SelectRandomActionMCTS(null, currentState.MyTeam, currentState.EnemyTeam, 10, dicOfDmgTakenByUnits);
            else
                randomSelectedAction = unit.SelectRandomActionMCTS(null, currentState.EnemyTeam, currentState.MyTeam, 10, dicOfDmgTakenByUnits);

            if (randomSelectedAction != null)
            {
                //                Debug.Log("Accion en simulate SimulateTurn: " + randomSelectedAction.GetString());
                randomSelectedAction.Simulate(currentState, IAMCTSController.playerNumber, dicOfDmgTakenByUnits);
            }
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
    public virtual void Backup(TreeNode<GameState> currentNode, double deltaValue)
    {
        while (currentNode != null)
        {
            currentNode.IncreaseVisits();
            currentNode.IncreaseReward(deltaValue);
            currentNode = currentNode.Parent;
        }
    }
    public virtual bool IsTerminal(GameState node, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        double healthOfMyTeam = 0;
        double healthOfEnemyTeam = 0;

        foreach (Unit character in node.MyTeam)
        {
            if (dicOfDmgTakenByUnits.ContainsKey(character))
                healthOfMyTeam += character.currentHealth - dicOfDmgTakenByUnits[character];
            else healthOfMyTeam += character.currentHealth;
        }

        foreach (Unit character in node.EnemyTeam)
        {
            if (dicOfDmgTakenByUnits.ContainsKey(character))
                healthOfEnemyTeam += character.currentHealth - dicOfDmgTakenByUnits[character];
            else healthOfEnemyTeam += character.currentHealth;
        }

        return (healthOfMyTeam <= 0) || (healthOfEnemyTeam <= 0);
    }
    protected GameState CloneInitialGameState(int playerNumber)
    {
        GameState initialState = new GameState();
        if (playerNumber == 1)
        {
            initialState.currentPlayer = 1;
        }
        else if (playerNumber == 2)
        {
            initialState.currentPlayer = 2;
        }
        foreach (Unit character in enemyTeam.characters)
        {
            initialState.EnemyTeam.Add(character);
        }

        foreach (Unit character in myTeam.characters)
        {
            initialState.MyTeam.Add(character);
        }

        return initialState;
    }
    protected GameState CopyGameState(GameState gameStateToCopy)
    {
        GameState gameState = new GameState();
        gameState.currentPlayer = gameStateToCopy.currentPlayer;
        gameState.dmgBasedReward = gameStateToCopy.dmgBasedReward;

        foreach (Unit character in gameStateToCopy.MyTeam)
        {
            gameState.MyTeam.Add(Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, simulatedUnitsContainer.transform));
        }

        foreach (Unit character in gameStateToCopy.EnemyTeam)
        {
            gameState.EnemyTeam.Add(Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, simulatedUnitsContainer.transform));
        }

        return gameState;
    }
    protected virtual TreeNode<GameState> ChildWithBestReward(TreeNode<GameState> parentNode)
    {
        TreeNode<GameState> bestChild = null;
        double reward = 0;
        double highestReward = -10000;

        foreach (TreeNode<GameState> treeNode in parentNode.Children)
        {
            reward = treeNode.Reward;
            if (reward > highestReward)
            {
                bestChild = treeNode;
                highestReward = reward;
            }
        }

        return bestChild;
    }
    protected virtual double RewardForState(GameState stateToEvaluate, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        double healthOfMyTeam = 0;
        double healthOfEnemyTeam = 0;
        double modifier = 1;

        foreach (Unit character in stateToEvaluate.MyTeam)
        {
            if (dicOfDmgTakenByUnits.ContainsKey(character))
            {
                healthOfMyTeam += character.currentHealth - dicOfDmgTakenByUnits[character];
            }
            else
            {
                healthOfMyTeam += character.currentHealth;
            }
        }

        foreach (Unit character in stateToEvaluate.EnemyTeam)
        {
            if (dicOfDmgTakenByUnits.ContainsKey(character))
            {
                healthOfEnemyTeam += character.currentHealth - dicOfDmgTakenByUnits[character];
            }
            else
            {
                healthOfEnemyTeam += character.currentHealth;
            }
        }

        if (healthOfMyTeam == 0)
        {
            modifier = 0.5f;
        }
        else if (healthOfEnemyTeam == 0)
        {
            modifier = 2f;
        }

        return (healthOfMyTeam - healthOfEnemyTeam) * modifier;
    }
    protected void ClearClonedObjects()
    {
        int childs = clonedUnitsContainer.transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            GameObject.Destroy(clonedUnitsContainer.transform.GetChild(i).gameObject);
        }
    }
    protected void ClearSimulationObjects()
    {
        int childs = simulatedUnitsContainer.transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            GameObject.Destroy(simulatedUnitsContainer.transform.GetChild(i).gameObject);
        }
    }
}