using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Unit
{
    public override string GetTypeOfUnit()
    {
        return "Fighter";
    }

    public override double GetScoreModifier()
    {
        return 1.3f;
    }
}
