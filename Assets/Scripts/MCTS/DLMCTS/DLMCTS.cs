using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLMCTS : MCTS
{
    public int maxAmountOfChilds;
    public int amountOfChildsComputed;
    public List<Action> listOfActionsToPlay;
    void Awake()
    {
        listOfActionsToPlay = null;
    }

    public override void FindAndExecuteMoves()
    {
        StartCoroutine(MCTSearchCR(CloneInitialGameState(IAMCTSController.playerNumber)));
    }

    protected virtual IEnumerator MCTSearchCR(GameState initialState)
    {
        amountOfChildsComputed = 0;
        TreeNode<GameState> root = new TreeNode<GameState>(initialState);
        TreeNode<GameState> currentNode;
        double reward;

        while (InComputationalBudget())
        {
            currentNode = Expand(root);
            reward = DefaultPolicy(currentNode.Data);
            ClearSimulationObjects();
            ClearClonedObjects();
            Backup(currentNode, reward);
            amountOfChildsComputed++;

            yield return null;
        }

        listOfActionsToPlay = ChildWithBestReward(root).Data.TakenActionsToThisState;
        ClearClonedObjects();
        IAMCTSController.ExecuteActions(listOfActionsToPlay);
    }

    public bool InComputationalBudget()
    {
        return (amountOfChildsComputed < maxAmountOfChilds);
    }
}