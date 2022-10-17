using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCTMCTS : MonoBehaviour
{
    public GameManager gameManager;
    public double computationalBudgetInSeconds;
    public double startingTimeOfComputation;
    public int maxAmountOfTurnSimulated;
    public int maxAmountOfBranchSimulated;
    public int constantValueForUCB;

    public GameObject simulatedUnitsContainer;
    public GameObject clonedUnitsContainer;
    public List<Action> listOfActionsToPlay;
    public IAMCTSController IAMCTSController;

    public float totalAmountOfTimeUsedForExpansions = 0;
    public float totalAmountOfTimeUsedForSimulations = 0;
    public float totalAmountOfTimeUsedForClearingSimulated = 0;
    public float totalAmountOfTimeUsedForBestChild = 0;
    public float totalAmountOfTimeUsedForBackup = 0;
    public float totalAmountOfTimeUsedForTreePolicy = 0;
    public float totalAmountOfTimeUsedForDefaultPolicy = 0;
    public double tiempoConsumido = 0;

    void Awake()
    {
        listOfActionsToPlay = null;
    }

    public void FindAndExecuteMoves()
    {
        StartCoroutine(UCTSearchCR(CloneInitialGameState()));
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
        //   Debug.Log("Cantidad de tiempo consumido en las expansiones: " + totalAmountOfTimeUsedForExpansions);
        //   Debug.Log("Cantidad de tiempo consumido en las simulaciones: " + totalAmountOfTimeUsedForSimulations);
        //   Debug.Log("Cantidad de tiempo consumido en la limpieza de simulaciones: " + (totalAmountOfTimeUsedForClearingSimulated));
        //   Debug.Log("Cantidad de tiempo consumido: " + (totalAmountOfTimeUsedForExpansions + totalAmountOfTimeUsedForSimulations + totalAmountOfTimeUsedForClearingSimulated + totalAmountOfTimeUsedForBackup));


        // ShowTree(root);


        listOfActionsToPlay = ChildWithBestReward(root).Data.TakenActionsToThisState;
        Debug.Log("-------------------------> " + ChildWithBestReward(root).Children.Length);
        ClearClonedObjects();
        IAMCTSController.ExecuteActions(listOfActionsToPlay);
    }

    private void ClearSimulationObjects()
    {
        Debug.Log("------------------------ ClearingSimulations --------------------------");
        float startingTimeOfComputationForCleaning = Time.realtimeSinceStartup;

        int childs = simulatedUnitsContainer.transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            GameObject.Destroy(simulatedUnitsContainer.transform.GetChild(i).gameObject);
        }

        totalAmountOfTimeUsedForClearingSimulated += Time.realtimeSinceStartup - startingTimeOfComputationForCleaning;
        Debug.Log("Tiempo consumido en la limpieza de las simulaciones: " + (Time.realtimeSinceStartup - startingTimeOfComputationForCleaning));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");
    }

    private void ClearClonedObjects()
    {
        int childs = clonedUnitsContainer.transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            GameObject.Destroy(clonedUnitsContainer.transform.GetChild(i).gameObject);
        }
    }

    public TreeNode<GameState> TreePolicy(TreeNode<GameState> nodeToExplore)
    {
        Debug.Log("------------------------ TreePolicy --------------------------");
        float startingTimeOfComputationForTreePolicy = Time.realtimeSinceStartup;

        while (!IsTerminal(nodeToExplore))
        {
            if (!IsFullyExpanded(nodeToExplore))
            {
                nodeToExplore.Data.ReduceAmountOfExpansionPosible();
                return Expand(nodeToExplore);
            }
            else
            {
                nodeToExplore = BestChild(nodeToExplore);
            }
        }

        totalAmountOfTimeUsedForTreePolicy += Time.realtimeSinceStartup - startingTimeOfComputationForTreePolicy;
        Debug.Log("Tiempo consumido en TreePolicy---------------------------------------------xxxxxxxxxxxxXXXXXXXXXXXXX: " + (Time.realtimeSinceStartup - startingTimeOfComputationForTreePolicy));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");

        return nodeToExplore;
    }

    public TreeNode<GameState> Expand(TreeNode<GameState> nodeToExpand)
    {
        Debug.Log("------------------------ Expansion --------------------------");
        float startingTimeOfComputationForExpansion = Time.realtimeSinceStartup;

        GameState newState = new GameState();
        List<CharacterInfo> unitsThatCanTakeAction = new List<CharacterInfo>();
        List<CharacterInfo> originalUnitsThatCanTakeAction = new List<CharacterInfo>();
        CharacterInfo unit;

        foreach (CharacterInfo character in nodeToExpand.Data.EnemyTeam)
        {
            newState.EnemyTeam.Add(Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, clonedUnitsContainer.transform));
        }

        foreach (CharacterInfo character in nodeToExpand.Data.MyTeam)
        {
            unit = Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, clonedUnitsContainer.transform);
            newState.MyTeam.Add(unit);
            unitsThatCanTakeAction.Add(unit);
            originalUnitsThatCanTakeAction.Add(character);
        }

        int randomUnitIndex;
        Action randomSelectedAction;

        Debug.Log("------------------------ Expansion ::> Unit actions --------------------------");
        float startingTimeOfComputationForExpansionUnitActions = Time.realtimeSinceStartup;

        while (unitsThatCanTakeAction.Count != 0)
        {
            randomUnitIndex = UnityEngine.Random.Range(0, unitsThatCanTakeAction.Count);
            unit = unitsThatCanTakeAction[randomUnitIndex];
            CharacterInfo character = originalUnitsThatCanTakeAction[randomUnitIndex];

            randomSelectedAction = unit.SelectRandomActionMCTS(character, newState.MyTeam, newState.EnemyTeam);

            if (randomSelectedAction != null)
            {
                newState.TakenActionsToThisState.Add(randomSelectedAction);
                randomSelectedAction.Simulate(newState);
            }

            if (!unit.CanTakeAction())
            {
                unitsThatCanTakeAction.Remove(unit);
                originalUnitsThatCanTakeAction.Remove(character);
                unit.moved = false;
                unit.attacked = false;
                //    character.moved = false;
                //    character.attacked = false;
            }
        }

        Debug.Log("Tiempo consumido para generar las acciones y simularlas: " + (Time.realtimeSinceStartup - startingTimeOfComputationForExpansionUnitActions));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");

        if (nodeToExpand.Data.currentPlayer == 1)
        {
            newState.currentPlayer = 2;
        }
        else if (nodeToExpand.Data.currentPlayer == 2)
        {
            newState.currentPlayer = 1;
        }

        totalAmountOfTimeUsedForExpansions += Time.realtimeSinceStartup - startingTimeOfComputationForExpansion;
        Debug.Log("Tiempo consumido en la expansion: " + (Time.realtimeSinceStartup - startingTimeOfComputationForExpansion));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");

        return nodeToExpand.AddChild(newState);
    }

    public double DefaultPolicy(GameState currentState)
    {
        Debug.Log("------------------------ DefaultPolicy --------------------------");
        float startingTimeOfComputationForDefaultPolicy = Time.realtimeSinceStartup;
        GameState simulatedNode = CopyGameState(currentState);
        int simulationsDoneSoFar = 0;
        int branchSimulationsDoneSoFar = 0;
        double maxRewardForState = -10000;
        double stateReward;
        double accumulativeReward = 0;

        while (branchSimulationsDoneSoFar < maxAmountOfBranchSimulated)
        {
            while (simulationsDoneSoFar < maxAmountOfTurnSimulated)
            {
                SimulateTurn(simulatedNode);
                simulationsDoneSoFar++;
            }
            simulationsDoneSoFar = 0;
            stateReward = RewardForState(simulatedNode);
            // Mejor reward de las simulaciones se guarda
            //if (stateReward > maxRewardForState)
            //{
            //    maxRewardForState = stateReward;
            //}
            accumulativeReward += stateReward;
            simulatedNode = CopyGameState(currentState);
            branchSimulationsDoneSoFar++;
        }
        totalAmountOfTimeUsedForDefaultPolicy += Time.realtimeSinceStartup - startingTimeOfComputationForDefaultPolicy;
        Debug.Log("Tiempo consumido en DefaultPolicy: " + (Time.realtimeSinceStartup - startingTimeOfComputationForDefaultPolicy));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");


        //return maxRewardForState;
        return accumulativeReward;
    }

    public TreeNode<GameState> BestChild(TreeNode<GameState> parentNode)
    {
        Debug.Log("------------------------ BestChild --------------------------");
        float startingTimeOfComputationForBestChild = Time.realtimeSinceStartup;

        TreeNode<GameState> bestChild = null;
        double nodeUCB = 0;
        double highestUCB = -1000;

        foreach (TreeNode<GameState> treeNode in parentNode.Children)
        {
            nodeUCB = UCB(treeNode);
            if (nodeUCB > highestUCB)
            {
                bestChild = treeNode;
                highestUCB = nodeUCB;
            }
        }

        totalAmountOfTimeUsedForBestChild += Time.realtimeSinceStartup - startingTimeOfComputationForBestChild;
        Debug.Log("Tiempo consumido en encontrar el mejor hijo: " + (Time.realtimeSinceStartup - startingTimeOfComputationForBestChild));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");
        return bestChild;
    }

    public void Backup(TreeNode<GameState> currentNode, double deltaValue)
    {
        Debug.Log("------------------------ Backup --------------------------");
        float startingTimeOfComputationForBackup = Time.realtimeSinceStartup;

        while (currentNode != null)
        {
            currentNode.IncreaseVisits();
            currentNode.IncreaseReward(deltaValue);
            currentNode = currentNode.Parent;
        }

        totalAmountOfTimeUsedForBackup += Time.realtimeSinceStartup - startingTimeOfComputationForBackup;
        Debug.Log("Tiempo consumido en hacer backup: " + (Time.realtimeSinceStartup - startingTimeOfComputationForBackup));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");
    }

    private GameState CloneInitialGameState()
    {
        GameState initialState = new GameState();

        foreach (CharacterInfo character in gameManager.teamOne.characters)
        {
            initialState.EnemyTeam.Add(character);
        }

        foreach (CharacterInfo character in gameManager.teamTwo.characters)
        {
            initialState.MyTeam.Add(character);
        }

        return initialState;
    }

    public bool InComputationalBudget()
    {
        tiempoConsumido = (Time.realtimeSinceStartup - startingTimeOfComputation);
        Debug.Log("Tiempo consumido: " + (Time.realtimeSinceStartup - startingTimeOfComputation));
        return ((Time.realtimeSinceStartup - startingTimeOfComputation) < computationalBudgetInSeconds);
    }

    public bool IsTerminal(TreeNode<GameState> node)
    {
        GameState state = node.Data;

        return (state.MyTeam.Count == 0) || (state.EnemyTeam.Count == 0);
    }

    public bool IsFullyExpanded(TreeNode<GameState> node)
    {
        return (node.Data.RemainingAmountOfExpansionPosible == 0);
    }

    private GameState CopyGameState(GameState gameStateToCopy)
    {
        GameState gameState = new GameState();
        gameState.currentPlayer = gameStateToCopy.currentPlayer;

        foreach (CharacterInfo character in gameStateToCopy.MyTeam)
        {
            gameState.MyTeam.Add(Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, simulatedUnitsContainer.transform));
        }

        foreach (CharacterInfo character in gameStateToCopy.EnemyTeam)
        {
            gameState.EnemyTeam.Add(Instantiate(character, new Vector3(0, 15, 0), Quaternion.identity, simulatedUnitsContainer.transform));
        }

        return gameState;
    }

    private void SimulateTurn(GameState currentState)
    {
        Debug.Log("------------------------ Simulacion --------------------------");
        float startingTimeOfComputationForSimulation = Time.realtimeSinceStartup;

        currentState.TakenActionsToThisState = new List<Action>();
        List<CharacterInfo> unitsThatCanTakeAction = new List<CharacterInfo>();
        CharacterInfo unit;

        if (currentState.currentPlayer == 1)
        {
            foreach (CharacterInfo character in currentState.EnemyTeam)
            {
                unitsThatCanTakeAction.Add(character);
            }
        }
        else if (currentState.currentPlayer == 2)
        {
            foreach (CharacterInfo character in currentState.MyTeam)
            {
                unitsThatCanTakeAction.Add(character);
            }
        }

        foreach (CharacterInfo character in unitsThatCanTakeAction)
        {
            character.moved = false;
            character.attacked = false;
        }

        int randomUnitIndex;
        Action randomSelectedAction = null;

        Debug.Log("------------------------ Pre::> Simulacion ::> UnitsActions -------------------------- ");
        Debug.Log(Time.realtimeSinceStartup - startingTimeOfComputationForSimulation);
        Debug.Log("------------------------ Simulacion ::> UnitsActions --------------------------");
        float startingTimeOfComputationForSimulationUnitsActions = Time.realtimeSinceStartup;

        while (unitsThatCanTakeAction.Count != 0)
        {
            randomUnitIndex = UnityEngine.Random.Range(0, unitsThatCanTakeAction.Count);
            unit = unitsThatCanTakeAction[randomUnitIndex];
            if (currentState.currentPlayer == 1)
                randomSelectedAction = unit.SelectRandomActionMCTS(null, currentState.EnemyTeam, currentState.MyTeam);
            else if (currentState.currentPlayer == 2)
                randomSelectedAction = unit.SelectRandomActionMCTS(null, currentState.MyTeam, currentState.EnemyTeam);

            if (randomSelectedAction != null)
            {
                currentState.TakenActionsToThisState.Add(randomSelectedAction);
                randomSelectedAction.Simulate(currentState);
            }
            if (!unit.CanTakeAction())
                unitsThatCanTakeAction.Remove(unit);
        }
        Debug.Log("Tiempo consumido en simular units actions: " + (Time.realtimeSinceStartup - startingTimeOfComputationForSimulationUnitsActions));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");
        Debug.Log("------------------------ Pos ::> Simulacion ::> UnitsActions -------------------------- ");
        Debug.Log(Time.realtimeSinceStartup - startingTimeOfComputationForSimulation);

        if (currentState.currentPlayer == 1)
        {
            currentState.currentPlayer = 2;
        }
        else if (currentState.currentPlayer == 2)
        {
            currentState.currentPlayer = 1;
        }

        Debug.Log("Tiempo consumido en simular turno: " + (Time.realtimeSinceStartup - startingTimeOfComputationForSimulation));
        Debug.Log("------------------------ XXXXXXXXXXX --------------------------");
    }

    private double RewardForState(GameState stateToEvaluate)
    {
        double healthOfMyTeam = 0;
        double healthfEnemyTeam = 0;
        double modifier = 1;

        foreach (CharacterInfo character in stateToEvaluate.MyTeam)
        {
            healthOfMyTeam += character.currentHealth;
        }

        foreach (CharacterInfo character in stateToEvaluate.EnemyTeam)
        {
            healthfEnemyTeam += character.currentHealth;
        }

        if (healthOfMyTeam == 0)
        {
            modifier = 0.5f;
        }
        else if (healthfEnemyTeam == 0)
        {
            modifier = 2f;
        }

        return ((healthOfMyTeam - healthfEnemyTeam) + stateToEvaluate.GetExtraReward()) * modifier;
    }

    public double UCB(TreeNode<GameState> node)
    {
        double UCB = 0;
        //TODO: chequear si es log 10 o log nat
        UCB = node.Reward + constantValueForUCB * (Math.Sqrt((Math.Log(node.Parent.Visits) / node.Visits)));
        return UCB;
    }

    public TreeNode<GameState> ChildWithBestReward(TreeNode<GameState> parentNode)
    {
        TreeNode<GameState> bestChild = null;
        double reward = 0;
        double highestReward = -1000;

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

    private void ShowTree(TreeNode<GameState> root)
    {
        foreach (TreeNode<GameState> child in root.Children)
        {
            Debug.Log("---------- Node ----------");
            GameState childState = child.Data;
            foreach (Action action in childState.TakenActionsToThisState)
            {
                Debug.Log("------ Action ------");
                Debug.Log("Accion de la unidad: " + action.unit + " !-! " + action + " ::: " + action.standingGrid2DLocation + " ---> " + action.targetGrid2DLocation);
            }
        }
    }
}