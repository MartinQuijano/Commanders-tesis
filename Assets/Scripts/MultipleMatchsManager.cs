using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultipleMatchsManager : MonoBehaviour
{
    public TeamManager teamOne;
    public TeamManager teamTwo;

    public Player playerOne;
    public Player playerTwo;

    public LogManager logManager;
    public UnitFactory unitFactory;

    public bool gameLaunched;

    public ScoreManager scoreManager;

    public TurnManager turnManager;
    private int lastHealthOfTeamOne;
    private int lastHealthOfTeamTwo;

    private int lastTurnThatHealthChanged;

    void Start()
    {

        //team 1 standard
        teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Fighter", new Vector2Int(-5, 0)));
        teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Tank", new Vector2Int(-4, -1)));
        teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Archer", new Vector2Int(-5, -2)));
        //    teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Archer", new Vector2Int(-5, -1)));

        playerOne.SetTeam(teamOne);

        //team 2 standard
        teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Fighter", new Vector2Int(2, 0)));
        teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Tank", new Vector2Int(1, -1)));
        teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Archer", new Vector2Int(2, -2)));
        //    teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Archer", new Vector2Int(2, -1)));

        /*
                //team 1 diagonal
                teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Fighter", new Vector2Int(-5, 0)));
                teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Tank", new Vector2Int(-4, 1)));
                teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Archer", new Vector2Int(-5, 2)));

                playerOne.SetTeam(teamOne);

                //team 2 diagonal
                teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Fighter", new Vector2Int(2, -2)));
                teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Tank", new Vector2Int(1, -3)));
                teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Archer", new Vector2Int(2, -4)));
        */

        /*  //team 1 test
          teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Fighter", new Vector2Int(-2, 0)));
          teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Tank", new Vector2Int(-2, -1)));
          teamOne.AddCharacter(unitFactory.GetUnitTeamOne("Archer", new Vector2Int(-5, 2)));

          playerOne.SetTeam(teamOne);

          //team 2 test
          teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Fighter", new Vector2Int(2, -2)));
          teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Tank", new Vector2Int(-1, -1)));
          teamTwo.AddCharacter(unitFactory.GetUnitTeamTwo("Archer", new Vector2Int(2, -4)));
  */
        playerTwo.SetTeam(teamTwo);


        gameLaunched = false;
        scoreManager = GameObject.FindGameObjectWithTag("score").GetComponent<ScoreManager>();

        lastTurnThatHealthChanged = 0;
        lastHealthOfTeamOne = teamOne.GetTeamHealth();
        lastHealthOfTeamTwo = teamTwo.GetTeamHealth();
    }

    void Update()
    {
        if (lastHealthOfTeamOne != teamOne.GetTeamHealth() || lastHealthOfTeamTwo != teamTwo.GetTeamHealth())
        {
            lastTurnThatHealthChanged = turnManager.turnsPlayed;
        }

        if (turnManager.turnsPlayed - lastTurnThatHealthChanged >= 10)
        {
            scoreManager.amountOfGamesToPlay--;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!gameLaunched)
        {
            logManager.SaveTeamsToFile(teamOne.characters, teamTwo.characters);
            gameLaunched = true;

            playerOne.OnTurnEnd();
            playerOne.OnTurnStart();
        }

        if (teamOne.characters.Count == 0)
        {
            scoreManager.winsTeamTwo++;
            if (scoreManager.amountOfGamesToPlay > 0)
            {
                scoreManager.amountOfGamesToPlay--;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else if (teamTwo.characters.Count == 0)
        {
            scoreManager.winsTeamOne++;
            if (scoreManager.amountOfGamesToPlay > 0)
            {
                scoreManager.amountOfGamesToPlay--;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        lastHealthOfTeamOne = teamOne.GetTeamHealth();
        lastHealthOfTeamTwo = teamTwo.GetTeamHealth();
    }
}
