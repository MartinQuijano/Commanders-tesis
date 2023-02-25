using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LogExecutor : MonoBehaviour
{
    public Tilemap blueBorderTilemap;
    public Tilemap redBorderTilemap;
    public TeamManager teamOne;
    public TeamManager teamTwo;
    public GameObject gameOverScreen;
    public Text gameOverText;

    public Player playerOne;
    public Player playerTwo;

    public LogManager logManager;

    public List<List<Action>> listOfListOfActions;
    public int playerNumberLog;
    public int listOfListIndex;

    void Start()
    {
        //team 1
        teamOne.characters = logManager.GetTeamOne();
        playerOne.SetTeam(teamOne);
        playerOne.SetCanTakeAction(true);

        foreach (Unit unit in playerOne.GetTeam().characters)
        {
            unit.SetMyTeam(playerOne.GetTeam());
        }
        playerOne.OnTurnEnd();

        //team2
        teamTwo.characters = logManager.GetTeamTwo();
        playerTwo.SetTeam(teamTwo);

        foreach (Unit unit in playerTwo.GetTeam().characters)
        {
            unit.SetMyTeam(playerTwo.GetTeam());
        }

        playerNumberLog = 1;
        listOfListIndex = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            blueBorderTilemap.gameObject.SetActive(!blueBorderTilemap.gameObject.activeSelf);
            redBorderTilemap.gameObject.SetActive(!redBorderTilemap.gameObject.activeSelf);
            listOfListOfActions = logManager.GetAllActions(teamOne.characters, teamTwo.characters);
            ExecuteNextListOfActions();
        }

        if (teamOne.characters.Count == 0)
        {
            gameOverScreen.SetActive(true);
            gameOverText.text = "Red team won!";

        }
        else if (teamTwo.characters.Count == 0)
        {
            gameOverScreen.SetActive(true);
            gameOverText.text = "Blue team won!";
        }

    }

    public void ExecuteNextListOfActions()
    {
        if (listOfListIndex < listOfListOfActions.Count)
        {
            if (playerNumberLog == 1)
            {
                playerOne.OnTurnStart();
                playerOne.ExecuteListOfActions(listOfListOfActions[listOfListIndex]);
                playerNumberLog = 2;
            }
            else
            {
                playerTwo.OnTurnStart();
                playerTwo.ExecuteListOfActions(listOfListOfActions[listOfListIndex]);
                playerNumberLog = 1;
            }
            listOfListIndex++;
        }
        blueBorderTilemap.gameObject.SetActive(!blueBorderTilemap.gameObject.activeSelf);
        redBorderTilemap.gameObject.SetActive(!redBorderTilemap.gameObject.activeSelf);
    }
}
