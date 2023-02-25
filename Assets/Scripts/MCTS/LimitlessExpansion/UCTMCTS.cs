using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCTMCTS : MCTS
{
    public double computationalBudgetInSeconds;
    public double startingTimeOfComputation;
    public int constantValueForUCB;
    public List<Action> listOfActionsToPlay;

    void Awake()
    {
        listOfActionsToPlay = null;
    }

    public override void FindAndExecuteMoves()
    {
        StartCoroutine(UCTSearchCR(CloneInitialGameState(IAMCTSController.playerNumber)));
    }

    IEnumerator UCTSearchCR(GameState initialState)
    {
        startingTimeOfComputation = Time.realtimeSinceStartup;

        TreeNode<GameState> root = new TreeNode<GameState>(initialState);
        TreeNode<GameState> currentNode;
        double reward;

        while (InComputationalBudget())
        {
            currentNode = TreePolicy(root);
            reward = DefaultPolicy(currentNode.Data);
            ClearSimulationObjects();
            Backup(currentNode, reward);

            yield return null;
        }

        //Debug.Log("Recompensa del mejor hijo de la raiz: " + BestChild(root).Reward);
        //Debug.Log("---------- Reward ---------- ");
        //Debug.Log("La mejor combinacion de acciones tiene una recompensa de: " + ChildWithBestReward(root).Reward);
        Debug.Log("Amount of children of root: " + root.Children.Length);
        Debug.Log("Amount of visits of root: " + root.Visits);
        foreach (TreeNode<GameState> tngs in root.Children)
        {
            Debug.Log("Amount of children of X child: " + tngs.Children.Length + " <---------- ");
            //  Debug.Log("UCB of node: " + UCB(tngs));
            //  Debug.Log("Amount of visits of X child: " + tngs.Visits);
            /*  if (tngs.Children.Length > 0)
              {
                  foreach (TreeNode<GameState> tngs2 in root.Children)
                  {
                      Debug.Log("Amount of children of XXXX child: " + tngs2.Children.Length);
                      Debug.Log("UCB of node XXXX: " + UCB(tngs2));
                  }
              }*/
        }
        // ShowTree(root);


        listOfActionsToPlay = ChildWithBestReward(root).Data.TakenActionsToThisState;
        //     Debug.Log("-------------------------> " + ChildWithBestReward(root).Children.Length);
        ClearClonedObjects();
        IAMCTSController.ExecuteActions(listOfActionsToPlay);
    }

    public override TreeNode<GameState> Expand(TreeNode<GameState> nodeToExpand)
    {
        GameState newState = new GameState();
        List<Unit> unitsThatCanTakeAction = new List<Unit>();
        List<Unit> originalUnitsThatCanTakeAction = new List<Unit>();
        Unit unit;

        if (nodeToExpand.Data.currentPlayer == 1)
            newState.currentPlayer = 1;
        else newState.currentPlayer = 2;

        if (newState.currentPlayer == IAMCTSController.playerNumber)
        {
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
        }
        else
        {
            foreach (Unit character in nodeToExpand.Data.MyTeam)
            {
                newState.MyTeam.Add(Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, clonedUnitsContainer.transform));
            }

            foreach (Unit character in nodeToExpand.Data.EnemyTeam)
            {
                unit = Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, clonedUnitsContainer.transform);
                newState.EnemyTeam.Add(unit);
                unitsThatCanTakeAction.Add(unit);
                originalUnitsThatCanTakeAction.Add(character);
            }
        }

        int randomUnitIndex;
        Action randomSelectedAction;

        while (unitsThatCanTakeAction.Count != 0)
        {
            randomUnitIndex = UnityEngine.Random.Range(0, unitsThatCanTakeAction.Count);
            unit = unitsThatCanTakeAction[randomUnitIndex];
            Unit character = originalUnitsThatCanTakeAction[randomUnitIndex];

            if (newState.currentPlayer == IAMCTSController.playerNumber)
                randomSelectedAction = unit.SelectRandomActionMCTS(character, newState.MyTeam, newState.EnemyTeam, 80, new Dictionary<Unit, int>());
            else
                randomSelectedAction = unit.SelectRandomActionMCTS(character, newState.EnemyTeam, newState.MyTeam, 80, new Dictionary<Unit, int>());

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

    public TreeNode<GameState> TreePolicy(TreeNode<GameState> nodeToExplore)
    {
        while (!IsTerminal(nodeToExplore))
        {
            if (!IsFullyExpanded(nodeToExplore))
            {
                nodeToExplore.ReduceAmountOfExpansionPosible();
                return Expand(nodeToExplore);
            }
            else
            {
                nodeToExplore = BestChild(nodeToExplore);
            }
        }

        return nodeToExplore;
    }

    public TreeNode<GameState> BestChild(TreeNode<GameState> parentNode)
    {
        TreeNode<GameState> bestChild = null;
        double nodeUCB = 0;
        double highestUCB = -10000;

        foreach (TreeNode<GameState> treeNode in parentNode.Children)
        {
            nodeUCB = UCB(treeNode);
            if (nodeUCB > highestUCB)
            {
                bestChild = treeNode;
                highestUCB = nodeUCB;
            }
        }

        return bestChild;
    }

    public bool InComputationalBudget()
    {
        return ((Time.realtimeSinceStartup - startingTimeOfComputation) < computationalBudgetInSeconds);
    }

    public bool IsTerminal(TreeNode<GameState> node)
    {
        GameState state = node.Data;

        return (state.MyTeam.Count == 0) || (state.EnemyTeam.Count == 0);
    }

    public bool IsFullyExpanded(TreeNode<GameState> node)
    {
        return (node.RemainingAmountOfExpansionPosible == 0);
    }
    public double UCB(TreeNode<GameState> node)
    {
        double UCB = 0;
        UCB = node.Reward + constantValueForUCB * (Math.Sqrt((Math.Log(node.Parent.Visits) / node.Visits)));
        return UCB;
    }
}