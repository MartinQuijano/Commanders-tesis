using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{

    public Attack(Unit unit, Unit originalUnit, Vector2Int standingGrid2DLocation, Vector2Int targetGrid2DLocation)
    {
        this.unit = unit;
        this.originalUnit = originalUnit;
        this.standingGrid2DLocation = standingGrid2DLocation;
        this.targetGrid2DLocation = targetGrid2DLocation;
        executed = false;
    }

    public override void Execute(MCTSAI iAMCTSController)
    {
        /*  Debug.Log("Ejecutando accion de ataque de la unidad " + originalUnit);
          Debug.Log("Desde la posicion " + standingGrid2DLocation);
          Debug.Log("A la posicion " + targetGrid2DLocation);
          Debug.Log("En esa posicion se encuentra la unidad: " + MapManager.Instance.map[targetGrid2DLocation].characterOnTile);*/
        //  originalUnit.AttackEnemy(new CharacterInfo());
        originalUnit.AttackEnemy(MapManager.Instance.map[targetGrid2DLocation].characterOnTile);
        // Debug.Log("ATAQUE!");
        iAMCTSController.ExecuteNextAction();
    }

    public override void SimulateExpansion(GameState startingState, int playerNumber)
    {
        // Debug.Log("SimulateExp");
        //  Debug.Log("StartingState.currentPlayer: " + startingState.currentPlayer);
        //  Debug.Log("Player number: " + playerNumber);
        //   Debug.Log(" >>>>>>>>> Accion de ataque de " + unit + " desde la posicion " + standingGrid2DLocation + " a la posicion " + targetGrid2DLocation);

        Unit enemyToAttack = null;
        List<Unit> listOfEnemies = null;
        int index = 0;
        bool enemyFound = false;

        if (playerNumber == startingState.currentPlayer)
        {
            listOfEnemies = startingState.EnemyTeam;
        }
        else
        {
            listOfEnemies = startingState.MyTeam;
        }

        while (!enemyFound && index < listOfEnemies.Count)
        {
            if (listOfEnemies[index].standingOnTile.grid2DLocation == targetGrid2DLocation)
            {
                enemyToAttack = listOfEnemies[index];
                enemyFound = true;
            }
            index++;
        }

        if (enemyToAttack.currentHealth - unit.damage <= 0)
        {
            //            Debug.Log("Extra reward en SimulateExp!");
            startingState.EnemyTeam.Remove(enemyToAttack);
            //      startingState.extraReward += (enemyToAttack.GetKillScore() * extraRewardModifier);
            //         Debug.Log("SE AGREGA RECOMPENSA AL MATAR! " + (enemyToAttack.GetKillScore() * extraRewardModifier));
            //     startingState.localReward += (20 * extraRewardModifier);
        }
        if (enemyToAttack.currentHealth < unit.damage)
        {
            // Debug.Log("Exp, Se suma a la recompensa de daño : " + (enemyToAttack.currentHealth * enemyToAttack.GetScoreModifier()) * 1f);
            startingState.dmgBasedReward += (enemyToAttack.currentHealth * enemyToAttack.GetScoreModifier());
        }
        else
        {
            // Debug.Log("Exp, Se suma a la recompensa de daño : " + (unit.damage * enemyToAttack.GetScoreModifier()) * 1f);
            startingState.dmgBasedReward += (unit.damage * enemyToAttack.GetScoreModifier());
        }
        enemyToAttack.TakeDamage(unit.damage);
        //     Debug.Log(" ------------------>  Attack: " + enemyToAttack + " le queda: " + enemyToAttack.currentHealth);
    }

    public override void Simulate(GameState startingState, int playerNumber, Dictionary<Unit, int> dicOfDmgTakenByUnits)
    {
        //Debug.Log("Simulate");
        // Debug.Log("StartingState.currentPlayer: " + startingState.currentPlayer);
        // Debug.Log("Player number: " + playerNumber);
        // Debug.Log(" >>>>>>>>> Accion de ataque de " + unit + " desde la posicion " + standingGrid2DLocation + " a la posicion " + targetGrid2DLocation);
        Unit enemyToAttack = null;
        int extraRewardModifier = 1;
        int damageDone = 0;
        int index = 0;
        bool enemyFound = false;
        List<Unit> listOfEnemies = null;

        if (playerNumber == startingState.currentPlayer)
        {
            listOfEnemies = startingState.EnemyTeam;
            extraRewardModifier = 1;
        }
        else
        {
            listOfEnemies = startingState.MyTeam;
            extraRewardModifier = -1;
        }

        while (!enemyFound && index < listOfEnemies.Count)
        {
            if (listOfEnemies[index].standingOnTile.grid2DLocation == targetGrid2DLocation)
            {
                enemyToAttack = listOfEnemies[index];
                enemyFound = true;
            }
            index++;
        }
        /*  Debug.Log("Player number: " + playerNumber);
          Debug.Log("StartingState currentPlayer: " + startingState.currentPlayer);*/

        if (enemyToAttack != null)
        {
            if (dicOfDmgTakenByUnits.ContainsKey(enemyToAttack))
            {
                int dañoPrevio = dicOfDmgTakenByUnits[enemyToAttack];
                if (unit.damage > (enemyToAttack.currentHealth - dañoPrevio))
                {
                    damageDone = enemyToAttack.currentHealth - dañoPrevio;
                    dicOfDmgTakenByUnits[enemyToAttack] = enemyToAttack.currentHealth;
                }
                else
                {
                    damageDone = unit.damage;
                    dicOfDmgTakenByUnits[enemyToAttack] = dañoPrevio + unit.damage;
                }
            }
            else
            {
                if (enemyToAttack.currentHealth < unit.damage)
                {
                    damageDone = enemyToAttack.currentHealth;
                    dicOfDmgTakenByUnits.Add(enemyToAttack, enemyToAttack.currentHealth);
                }
                else
                {
                    damageDone = unit.damage;
                    dicOfDmgTakenByUnits.Add(enemyToAttack, unit.damage);
                }
            }
            //     Debug.Log("enemyToAttack: " + enemyToAttack + " currentHealth: " + enemyToAttack.currentHealth);
            //     Debug.Log("dicOfDmgTakenByUnits[enemyToAttack]: " + dicOfDmgTakenByUnits[enemyToAttack]);
            //  if (enemyToAttack.currentHealth - unit.damage <= 0 || enemyToAttack.currentHealth <= dicOfDmgTakenByUnits[enemyToAttack])
            //  {
            //        Debug.Log("En la simulacion de ataque, se mato la unidad: " + enemyToAttack + " en la posicion: " + targetGrid2DLocation + " recompensa = " + (enemyToAttack.GetKillScore() * extraRewardModifier));

            //TODO: lo saco pq genera demasiado valor en el nodo expandido... y ese valor se obtiene de las simulaciones... que puede resultar en buenos valores por matar una unidad
            // dsp y no inmediatamente

            //  Debug.Log("UNIDAD MATADA!" + enemyToAttack + " en la posicion: " + enemyToAttack.standingOnTile.grid2DLocation + " Con modifier: " + extraRewardModifier);
            //      startingState.extraReward += (enemyToAttack.GetKillScore() * extraRewardModifier);
            // }
            //            Debug.Log("Se suma a la recompensa de daño: " + ((damageDone * enemyToAttack.GetScoreModifier()) * extraRewardModifier));
            startingState.dmgBasedReward += ((damageDone * enemyToAttack.GetScoreModifier()) * extraRewardModifier);
        }
    }

    public override void SimulateForANAG(AggressiveNAG anag)
    {
        //        Debug.Log("Sim3: " + unit);
        unit.SimulateAttackNAG();
        anag.enemyUnitsToRepositionOnTiles.Add(MapManager.Instance.map[targetGrid2DLocation].characterOnTile);
        if (anag.acumulatedDmgOnUnit.ContainsKey(MapManager.Instance.map[targetGrid2DLocation].characterOnTile))
        {
            int dañoPrevio = anag.acumulatedDmgOnUnit[MapManager.Instance.map[targetGrid2DLocation].characterOnTile];
            anag.acumulatedDmgOnUnit[MapManager.Instance.map[targetGrid2DLocation].characterOnTile] = dañoPrevio + unit.damage;
        }
        else
        {
            anag.acumulatedDmgOnUnit.Add(MapManager.Instance.map[targetGrid2DLocation].characterOnTile, unit.damage);
        }
        if (MapManager.Instance.map[targetGrid2DLocation].characterOnTile.currentHealth - unit.damage <= 0 || MapManager.Instance.map[targetGrid2DLocation].characterOnTile.currentHealth <= anag.acumulatedDmgOnUnit[MapManager.Instance.map[targetGrid2DLocation].characterOnTile])
        {
            // como la vida no se le va descontando, en sucesivos ataques esto no va a pasar nunca y va a haber un error... buscar forma de marcarlo como muerto
            //    Debug.Log("En simulate3 de ataque, se va a sacar el caracter del tile pq murio! posicion: " + targetGrid2DLocation);
            MapManager.Instance.map[targetGrid2DLocation].characterOnTile = null;
        }
    }

    public override void LogSimulate(Simulator simulator)
    {
        originalUnit.AttackEnemy(MapManager.Instance.map[targetGrid2DLocation].characterOnTile);
        simulator.ExecuteNextAction();
    }

    public override string GetLogName()
    {
        return "Attack";
    }

    public override string GetString()
    {
        return "Attack action from position: " + standingGrid2DLocation + " to position: " + targetGrid2DLocation;
    }
}