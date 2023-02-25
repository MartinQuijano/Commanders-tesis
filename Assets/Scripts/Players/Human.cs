using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Player
{
    [SerializeField]
    private GameObject mouse;
    private MouseController mouseController;

    void Start()
    {
        mouseController = mouse.GetComponent<MouseController>();
    }

    void Update()
    {
        if (canTakeAction)
        {
            mouseController.Operate();
        }
    }

    public override void OnTurnStart()
    {
        myTeam.MakeAllCharacterActive();
        myTeam.ChangeIsFromCurrentPlayingTeamFromCharacters();
        myTeam.RefreshCharacters();
        canTakeAction = true;
    }

    public override void OnTurnEnd()
    {
        myTeam.MakeAllCharacterActive();
        myTeam.ChangeIsFromCurrentPlayingTeamFromCharacters();
        canTakeAction = false;
    }

    public override bool CanUseButtons()
    {
        return true;
    }

    public override void ExecuteListOfActions(List<Action> actions)
    {

    }
}
