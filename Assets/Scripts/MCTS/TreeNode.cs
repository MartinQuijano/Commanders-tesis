using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode<T>
{
    private T nodeData;
    private ArrayList childNodes;

    private TreeNode<T> parent;

    private double reward;
    private int visits;

    public TreeNode(T nodeData)
    {
        this.nodeData = nodeData;
        childNodes = new ArrayList();
        parent = null;
        reward = 0;
        visits = 0;
    }

    public T Data
    {
        get { return this.nodeData; }

        set { nodeData = value; }
    }

    public double Reward { get { return reward; } }

    public TreeNode<T> Parent { get { return parent; } }

    public int Visits { get { return visits; } }

    public TreeNode<T>[] Children
    {
        get { return (TreeNode<T>[])this.childNodes.ToArray(typeof(TreeNode<T>)); }
    }

    public TreeNode<T> this[int index]
    {
        get { return (TreeNode<T>)this.childNodes[index]; }
    }

    public TreeNode<T> AddChild(T nodeData)
    {
        TreeNode<T> newNode = new TreeNode<T>(nodeData);
        this.childNodes.Add(newNode);
        newNode.parent = this;
        return newNode;
    }

    public override string ToString()
    {
        return this.nodeData.ToString();
    }

    public void IncreaseReward(double rewardIncreaseAmount)
    {
        reward += rewardIncreaseAmount;
    }

    public void IncreaseVisits()
    {
        visits++;
    }
}
