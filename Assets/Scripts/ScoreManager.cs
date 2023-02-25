using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int winsTeamOne;
    public int winsTeamTwo;

    public int amountOfGamesToPlay;

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            created = true;
            DontDestroyOnLoad(gameObject);
            amountOfGamesToPlay = 200;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
