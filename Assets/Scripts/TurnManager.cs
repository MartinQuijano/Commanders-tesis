using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TurnManager : MonoBehaviour
{
    public Tilemap blueBorderTilemap;
    public Tilemap redBorderTilemap;

    // public int teamPlaying;
    public MCTSAI ia;
    public Button endTurnButton;
    private int currentPlayerIndex;
    public Player[] players;

    public bool gameEnded;

    public int turnsPlayed;

    void Awake()
    {

        currentPlayerIndex = 0;
        turnsPlayed = 0;

    }

    public void EndTurn()
    {
        if (!gameEnded)
        {
            turnsPlayed++;
            blueBorderTilemap.gameObject.SetActive(!blueBorderTilemap.gameObject.activeSelf);
            redBorderTilemap.gameObject.SetActive(!redBorderTilemap.gameObject.activeSelf);

            GridHUDDisplayer.Instance.ClearMovementTilesInRangeDisplayed();
            GridHUDDisplayer.Instance.ClearAttackTilesInRangeDisplayed();
            GridHUDDisplayer.Instance.ClearPath();

            StartCoroutine(WaitAndEndTurn());

        }
    }

    IEnumerator WaitAndEndTurn()
    {
        yield return new WaitForSeconds(0.01f);
        players[currentPlayerIndex].OnTurnEnd();
        currentPlayerIndex = GetIndexOfNextPlayer();
        if (!players[currentPlayerIndex].CanUseButtons())
            endTurnButton.gameObject.SetActive(false);
        else
            endTurnButton.gameObject.SetActive(true);
        players[currentPlayerIndex].OnTurnStart();
    }

    public int GetIndexOfNextPlayer()
    {
        int index = currentPlayerIndex + 1;
        if (index == players.Length)
        {
            index = 0;
        }
        return index;
    }
}