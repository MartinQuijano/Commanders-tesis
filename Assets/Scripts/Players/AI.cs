using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI : Player
{
    public override bool CanUseButtons()
    {
        return false;
    }

    public override void OnTurnEnd()
    {
        myTeam.MakeAllCharacterActive();
        myTeam.ChangeIsFromCurrentPlayingTeamFromCharacters();
        canTakeAction = false;
    }
}
