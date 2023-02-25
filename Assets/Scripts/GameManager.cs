using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TeamManager teamOne;
    public TeamManager teamTwo;

    public TurnManager turnManager;
    public GameObject mouseController;

    public GameObject gameOverScreen;
    public Text gameOverText;

    public Player playerOne;
    public Player playerTwo;

    public LogManager logManager;
    public UnitFactory unitFactory;

    void Start()
    {
        //team 1
        teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Fighter", new Vector2Int(-1, -2)));
        teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Fighter", new Vector2Int(-2, -1)));
        teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Tank", new Vector2Int(-1, 0)));
        teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Archer", new Vector2Int(-3, 0)));

        playerOne.SetTeam(teamOne);

        //team2
        //teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Fighter", new Vector2Int(-1, -1)));
        teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Fighter", new Vector2Int(0, -2)));
        teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Tank", new Vector2Int(-3, -1)));
        teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Archer", new Vector2Int(1, -1)));

        /*     teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Fighter", new Vector2Int(-5, 0)));
            teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Tank", new Vector2Int(-4, -1)));
            teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Archer", new Vector2Int(-5, -2)));

            playerOne.SetTeam(teamOne);

            //team2
            teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Fighter", new Vector2Int(2, 0)));
            teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Tank", new Vector2Int(1, -1)));
            teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Archer", new Vector2Int(2, -2))); */


        playerTwo.SetTeam(teamTwo);

        logManager.SaveTeamsToFile(teamOne.characters, teamTwo.characters);
        turnManager.gameEnded = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerOne.OnTurnEnd();
            playerOne.OnTurnStart();
        }

        if (teamOne.characters.Count == 0)
        {
            turnManager.gameEnded = true;
            playerOne.SetCanTakeAction(false);
            playerTwo.SetCanTakeAction(false);
            mouseController.SetActive(false);
            gameOverScreen.SetActive(true);
            if (turnManager.ia == null)
            {
                gameOverText.text = "Red team won!";
            }
            else
            {
                gameOverText.text = "You Lost!";
            }
        }
        else if (teamTwo.characters.Count == 0)
        {
            turnManager.gameEnded = true;
            playerOne.SetCanTakeAction(false);
            playerTwo.SetCanTakeAction(false);
            mouseController.SetActive(false);
            gameOverScreen.SetActive(true);
            if (turnManager.ia == null)
            {
                gameOverText.text = "Blue team won!";
            }
            else
            {
                gameOverText.text = "You won!";
            }
        }

    }
}