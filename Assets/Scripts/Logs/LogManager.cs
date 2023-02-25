using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LogManager : MonoBehaviour
{
    private string pathOfDir = "Assets/Resources/";
    public UnitFactory unitFactory;

    private int currentPlayerLog;
    private int currentFileNumber;
    private string finalPath;
    public TextAsset fileToExecute;

    void Awake()
    {
        currentPlayerLog = 1;
    }

    public void SaveTeamsToFile(List<Unit> teamOne, List<Unit> teamTwo)
    {
        var info = new DirectoryInfo(pathOfDir);
        int i = info.GetFiles().Length;
        //     Debug.Log("i: " + i);
        currentFileNumber = i + 1;

        finalPath = pathOfDir + "log_" + currentFileNumber.ToString() + ".txt";
        //     Debug.Log("finalPath: " + finalPath);

        StreamWriter writer = new StreamWriter(finalPath, true);

        int index = 0;
        while (index < teamOne.Count)
        {
            writer.Write(teamOne[index].GetTypeOfUnit());
            writer.Write("#");
            writer.Write(teamOne[index].standingOnTile.grid2DLocation.x + "," + teamOne[index].standingOnTile.grid2DLocation.y);
            index++;
            if (index < teamOne.Count)
                writer.Write("/");
        }

        writer.WriteLine();

        index = 0;
        while (index < teamTwo.Count)
        {
            writer.Write(teamTwo[index].GetTypeOfUnit());
            writer.Write("#");
            writer.Write(teamTwo[index].standingOnTile.grid2DLocation.x + "," + teamTwo[index].standingOnTile.grid2DLocation.y);
            index++;
            if (index < teamTwo.Count)
                writer.Write("/");
        }

        writer.WriteLine();

        writer.Close();
    }

    public List<Unit> GetTeamOne()
    {
        List<Unit> teamOne = new List<Unit>();
        StreamReader reader = new StreamReader(pathOfDir + fileToExecute.name + ".txt");

        string teamLine = reader.ReadLine();
        string[] unitsInfo = teamLine.Split('/');
        string[] unitData;
        string[] xyValues;
        foreach (string unit in unitsInfo)
        {
            unitData = unit.Split('#');
            xyValues = unitData[1].Split(',');
            teamOne.Add(unitFactory.GetUnitTeamOne(unitData[0], new Vector2Int(int.Parse(xyValues[0]), int.Parse(xyValues[1]))));
        }

        reader.Close();
        return teamOne;
    }

    public List<Unit> GetTeamTwo()
    {
        List<Unit> teamTwo = new List<Unit>();
        StreamReader reader = new StreamReader(pathOfDir + fileToExecute.name + ".txt");

        string teamLine = reader.ReadLine();
        teamLine = reader.ReadLine();
        string[] unitsInfo = teamLine.Split('/');
        string[] unitData;
        string[] xyValues;
        foreach (string unit in unitsInfo)
        {
            unitData = unit.Split('#');
            xyValues = unitData[1].Split(',');
            teamTwo.Add(unitFactory.GetUnitTeamTwo(unitData[0], new Vector2Int(int.Parse(xyValues[0]), int.Parse(xyValues[1]))));
        }

        reader.Close();
        return teamTwo;
    }

    public void LogListOfActions(List<Action> listOfActionsToLog)
    {
        string player;
        if (currentPlayerLog == 1)
        {
            player = "Player 1";
            currentPlayerLog = 2;
        }
        else
        {
            player = "Player 2";
            currentPlayerLog = 1;
        }
        StreamWriter writer = new StreamWriter(finalPath, true);

        writer.WriteLine(player);

        foreach (Action action in listOfActionsToLog)
        {
            Unit unit = action.originalUnit;
            writer.WriteLine(unit.numericLogValue + "/" + action.GetLogName() + "/" + action.standingGrid2DLocation.x + "," + action.standingGrid2DLocation.y + "/" + action.targetGrid2DLocation.x + "," + action.targetGrid2DLocation.y);
        }

        writer.Close();
    }

    public List<List<Action>> GetAllActions(List<Unit> teamOne, List<Unit> teamTwo)
    {
        List<List<Action>> listOfListofActions = new List<List<Action>>();
        List<Action> actions = new List<Action>();
        Action action = null;

        StreamReader reader = new StreamReader(pathOfDir + fileToExecute.name + ".txt");
        string teamLine = reader.ReadLine();
        teamLine = reader.ReadLine();
        teamLine = reader.ReadLine();
        teamLine = reader.ReadLine();

        while (teamLine != null)
        {
            while (teamLine != "Player 2" && teamLine != null)
            {
                string[] actionData = teamLine.Split('/');
                int index = 0;
                bool found = false;
                Unit unit = null;
                while (index < teamOne.Count && !found)
                {
                    if (teamOne[index].numericLogValue == int.Parse(actionData[0]))
                    {
                        found = true;
                        unit = teamOne[index];
                    }
                    index++;
                }
                string[] xyValuesOrigin = actionData[2].Split(',');
                string[] xyValuesDestiny = actionData[3].Split(',');
                switch (actionData[1])
                {
                    case "Movement":
                        action = new Movement(null, unit, new Vector2Int(int.Parse(xyValuesOrigin[0]), int.Parse(xyValuesOrigin[1])), new Vector2Int(int.Parse(xyValuesDestiny[0]), int.Parse(xyValuesDestiny[1])));
                        break;
                    case "Attack":
                        action = new Attack(null, unit, new Vector2Int(int.Parse(xyValuesOrigin[0]), int.Parse(xyValuesOrigin[1])), new Vector2Int(int.Parse(xyValuesDestiny[0]), int.Parse(xyValuesDestiny[1])));
                        break;
                }

                actions.Add(action);
                teamLine = reader.ReadLine();
            }
            teamLine = reader.ReadLine();

            listOfListofActions.Add(actions);
            actions = new List<Action>();

            while (teamLine != "Player 1" && teamLine != null)
            {
                string[] actionData = teamLine.Split('/');
                int index = 0;
                bool found = false;
                Unit unit = null;
                while (index < teamTwo.Count && !found)
                {
                    if (teamTwo[index].numericLogValue == int.Parse(actionData[0]))
                    {
                        found = true;
                        unit = teamTwo[index];
                    }
                    index++;
                }
                string[] xyValuesOrigin = actionData[2].Split(',');
                string[] xyValuesDestiny = actionData[3].Split(',');
                switch (actionData[1])
                {
                    case "Movement":
                        action = new Movement(null, unit, new Vector2Int(int.Parse(xyValuesOrigin[0]), int.Parse(xyValuesOrigin[1])), new Vector2Int(int.Parse(xyValuesDestiny[0]), int.Parse(xyValuesDestiny[1])));
                        break;
                    case "Attack":
                        action = new Attack(null, unit, new Vector2Int(int.Parse(xyValuesOrigin[0]), int.Parse(xyValuesOrigin[1])), new Vector2Int(int.Parse(xyValuesDestiny[0]), int.Parse(xyValuesDestiny[1])));
                        break;
                }

                actions.Add(action);
                teamLine = reader.ReadLine();
            }

            teamLine = reader.ReadLine();
            listOfListofActions.Add(actions);
            actions = new List<Action>();
        }

        return listOfListofActions;
    }
}
