using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TurnManager : MonoBehaviour
{
    public TeamManager teamOne;
    public TeamManager teamTwo;

    public Tilemap blueBorderTilemap;
    public Tilemap redBorderTilemap;

    public int teamPlaying;
    public IAMCTSController ia;
    public Button endTurnButton;

    void Awake()
    {
        teamPlaying = 1;
        GameObject IAGO = GameObject.Find("IA");
        if (IAGO != null)
            ia = IAGO.GetComponent<IAMCTSController>();
        else ia = null;
    }

    public void EndTurn()
    {
        GridHUDDisplayer.Instance.ClearMovementTilesInRangeDisplayed();
        GridHUDDisplayer.Instance.ClearAttackTilesInRangeDisplayed();
        GridHUDDisplayer.Instance.ClearPath();

        teamOne.MakeAllCharacterActive();
        teamTwo.MakeAllCharacterActive();

        if (teamPlaying == 1)
        {
            teamTwo.RefreshCharacters();
            teamPlaying = 2;
            if (ia != null)
            {
                ia.OnTurnStart();
                endTurnButton.gameObject.SetActive(false);
            }
        }
        else
        {
            teamOne.RefreshCharacters();
            teamPlaying = 1;
            if (ia != null)
            {
                endTurnButton.gameObject.SetActive(true);
            }
        }
        teamOne.ChangeIsFromCurrentPlayingTeamFromCharacters();
        teamTwo.ChangeIsFromCurrentPlayingTeamFromCharacters();

        blueBorderTilemap.gameObject.SetActive(!blueBorderTilemap.gameObject.activeSelf);
        redBorderTilemap.gameObject.SetActive(!redBorderTilemap.gameObject.activeSelf);
    }
}