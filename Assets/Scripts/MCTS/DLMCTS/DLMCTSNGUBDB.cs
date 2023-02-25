using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLMCTSNGUBDB : DLMCTSNGUB
{
    protected override double RewardForState(GameState stateToEvaluate, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        double healthOfMyTeam = 0;
        double healthOfEnemyTeam = 0;
        double modifier = 1;

        //Debug.Log("---------> tamaño de dic: " + dicOfDmgTakenByUnits.Count);
        foreach (Unit character in stateToEvaluate.MyTeam)
        {
            if (dicOfDmgTakenByUnits.ContainsKey(character))
            {
                //     Debug.Log("En reward mi team: " + character + " con salud: " + character.currentHealth + " recibio daño igual a " + dicOfDmgTakenByUnits[character]);
                healthOfMyTeam += character.currentHealth - dicOfDmgTakenByUnits[character];
            }
            else
            {
                //     Debug.Log("En reward mi team: " + character + " con salud: " + character.currentHealth);
                healthOfMyTeam += character.currentHealth;
            }
        }

        foreach (Unit character in stateToEvaluate.EnemyTeam)
        {
            if (dicOfDmgTakenByUnits.ContainsKey(character))
            {
                //      Debug.Log("En reward enemy team: " + character + " con salud: " + character.currentHealth + " recibio daño igual a " + dicOfDmgTakenByUnits[character]);
                healthOfEnemyTeam += character.currentHealth - dicOfDmgTakenByUnits[character];
            }
            else
            {
                //      Debug.Log("En reward enemy team: " + character + " con salud: " + character.currentHealth);
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

        //      Debug.Log("Health of my team: " + healthOfMyTeam);
        //      Debug.Log("Health of enemy team: " + healthOfEnemyTeam);
        //      Debug.Log("Extra reward: " + stateToEvaluate.GetExtraReward());
        //Debug.Log("stateToEvaluate.dmgBasedReward: " + stateToEvaluate.dmgBasedReward);
        //Debug.Log("stateToEvaluate.extraReward: " + stateToEvaluate.extraReward);
        //Debug.Log("Modifier: " + modifier);
        //Debug.Log("Puntaje al evaluar estado recompensa daño: " + (((stateToEvaluate.dmgBasedReward) + stateToEvaluate.extraReward) * modifier));
        return (stateToEvaluate.dmgBasedReward) * modifier;
    }

    protected override TreeNode<GameState> ChildWithBestReward(TreeNode<GameState> parentNode)
    {
        TreeNode<GameState> bestChild = null;
        double reward = 0;
        double highestReward = -10000;
        foreach (TreeNode<GameState> treeNode in parentNode.Children)
        {
            reward = treeNode.Data.dmgBasedReward + treeNode.Data.localReward;
            if (reward > highestReward)
            {
                bestChild = treeNode;
                highestReward = reward;
            }
        }

        return bestChild;
    }

    public override void Backup(TreeNode<GameState> currentNode, double deltaValue)
    {
        while (currentNode != null)
        {
            currentNode.IncreaseVisits();
            currentNode.Data.dmgBasedReward = deltaValue;
            currentNode = currentNode.Parent;
        }
    }
}