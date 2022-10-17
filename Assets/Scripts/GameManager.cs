using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TeamManager teamOne;
    public TeamManager teamTwo;

    public GameObject characterFighterTeamOnePrefab;
    public GameObject characterTankTeamOnePrefab;
    public GameObject characterArcherTeamOnePrefab;

    public GameObject characterFighterTeamTwoPrefab;
    public GameObject characterTankTeamTwoPrefab;
    public GameObject characterArcherTeamTwoPrefab;

    public TurnManager turnManager;
    public GameObject mouseController;

    public GameObject gameOverScreen;
    public Text gameOverText;

    void Start()
    {
        //team 1
        CharacterInfo characterInfo = Instantiate(characterFighterTeamOnePrefab).GetComponent<CharacterInfo>();
        characterInfo.PositionCharacterOnTile(MapManager.Instance.map[new Vector2Int(-5, 0)]);
        characterInfo.isFromCurrentPlayingTeam = true;
        MapManager.Instance.map[new Vector2Int(-5, 0)].characterOnTile = characterInfo;
        teamOne.AddCharacter(characterInfo);

        characterInfo = Instantiate(characterTankTeamOnePrefab).GetComponent<CharacterInfo>();
        characterInfo.PositionCharacterOnTile(MapManager.Instance.map[new Vector2Int(-4, -1)]);
        characterInfo.isFromCurrentPlayingTeam = true;
        MapManager.Instance.map[new Vector2Int(-4, -1)].characterOnTile = characterInfo;
        teamOne.AddCharacter(characterInfo);

        characterInfo = Instantiate(characterArcherTeamOnePrefab).GetComponent<CharacterInfo>();
        characterInfo.PositionCharacterOnTile(MapManager.Instance.map[new Vector2Int(-5, -3)]);
        characterInfo.isFromCurrentPlayingTeam = true;
        MapManager.Instance.map[new Vector2Int(-5, -3)].characterOnTile = characterInfo;
        teamOne.AddCharacter(characterInfo);

        //team2
        characterInfo = Instantiate(characterFighterTeamTwoPrefab).GetComponent<CharacterInfo>();
        characterInfo.PositionCharacterOnTile(MapManager.Instance.map[new Vector2Int(1, 1)]);
        characterInfo.isFromCurrentPlayingTeam = false;
        characterInfo.GetComponent<SpriteRenderer>().flipX = true;
        MapManager.Instance.map[new Vector2Int(1, 1)].characterOnTile = characterInfo;
        teamTwo.AddCharacter(characterInfo);

        characterInfo = Instantiate(characterTankTeamTwoPrefab).GetComponent<CharacterInfo>();
        characterInfo.PositionCharacterOnTile(MapManager.Instance.map[new Vector2Int(1, 0)]);
        characterInfo.isFromCurrentPlayingTeam = false;
        characterInfo.GetComponent<SpriteRenderer>().flipX = true;
        MapManager.Instance.map[new Vector2Int(1, 0)].characterOnTile = characterInfo;
        teamTwo.AddCharacter(characterInfo);

        characterInfo = Instantiate(characterArcherTeamTwoPrefab).GetComponent<CharacterInfo>();
        characterInfo.PositionCharacterOnTile(MapManager.Instance.map[new Vector2Int(1, -3)]);
        characterInfo.isFromCurrentPlayingTeam = false;
        characterInfo.GetComponent<SpriteRenderer>().flipX = true;
        MapManager.Instance.map[new Vector2Int(1, -3)].characterOnTile = characterInfo;
        teamTwo.AddCharacter(characterInfo);
    }

    void Update()
    {
        if (teamOne.characters.Count == 0)
        {
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