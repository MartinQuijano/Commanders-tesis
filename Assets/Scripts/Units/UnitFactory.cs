using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    public GameObject characterFighterTeamOnePrefab;
    public GameObject characterTankTeamOnePrefab;
    public GameObject characterArcherTeamOnePrefab;

    public GameObject characterFighterTeamTwoPrefab;
    public GameObject characterTankTeamTwoPrefab;
    public GameObject characterArcherTeamTwoPrefab;

    public int currentUnitNumericValue;

    void Awake()
    {
        currentUnitNumericValue = 1;
    }

    public Unit GetUnitTeamOne(string unitType, Vector2Int position)
    {
        Unit unit = null;
        switch (unitType)
        {
            case "Fighter":
                unit = Instantiate(characterFighterTeamOnePrefab).GetComponent<Unit>();
                break;
            case "Tank":
                unit = Instantiate(characterTankTeamOnePrefab).GetComponent<Unit>();
                break;
            case "Archer":
                unit = Instantiate(characterArcherTeamOnePrefab).GetComponent<Unit>();
                break;
        }

        unit.PositionCharacterOnTile(MapManager.Instance.map[position]);
        unit.isFromCurrentPlayingTeam = true;
        MapManager.Instance.map[position].characterOnTile = unit;
        unit.numericLogValue = currentUnitNumericValue;
        currentUnitNumericValue++;

        return unit;
    }

    public Unit GetUnitTeamTwo(string unitType, Vector2Int position)
    {
        Unit unit = null;
        switch (unitType)
        {
            case "Fighter":
                unit = Instantiate(characterFighterTeamTwoPrefab).GetComponent<Unit>();
                break;
            case "Tank":
                unit = Instantiate(characterTankTeamTwoPrefab).GetComponent<Unit>();
                break;
            case "Archer":
                unit = Instantiate(characterArcherTeamTwoPrefab).GetComponent<Unit>();
                break;
        }

        unit.PositionCharacterOnTile(MapManager.Instance.map[position]);
        unit.isFromCurrentPlayingTeam = false;
        unit.GetComponent<SpriteRenderer>().flipX = true;
        MapManager.Instance.map[position].characterOnTile = unit;
        unit.numericLogValue = currentUnitNumericValue;
        currentUnitNumericValue++;

        return unit;
    }
}
