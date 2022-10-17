using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{

    public Attack(CharacterInfo unit, CharacterInfo originalUnit, Vector2Int standingGrid2DLocation, Vector2Int targetGrid2DLocation)
    {
        this.unit = unit;
        this.originalUnit = originalUnit;
        this.standingGrid2DLocation = standingGrid2DLocation;
        this.targetGrid2DLocation = targetGrid2DLocation;
        executed = false;
    }

    public override void Execute(IAMCTSController iAMCTSController)
    {
        //   Debug.Log("Ejecutando accion de ataque de la unidad " + originalUnit);
        //   Debug.Log("Desde la posicion " + standingGrid2DLocation);
        //   Debug.Log("A la posicion " + targetGrid2DLocation);
        //TODO: buscar unidad a atacar en la lista de unidades del enemigo, usando standingOnTile para la posicion.
        //  originalUnit.AttackEnemy(new CharacterInfo());
        originalUnit.AttackEnemy(MapManager.Instance.map[targetGrid2DLocation].characterOnTile);
        // Debug.Log("ATAQUE!");
        iAMCTSController.ExecuteNextAction();
    }

    public override void Simulate(GameState startingState)
    {
        //     Debug.Log(" --------- Accion de ataque de " + unit + " desde la posicion " + standingGrid2DLocation + " a la posicion " + targetGrid2DLocation);
        CharacterInfo enemyToAttack = null;
        //TODO: hacer mas eficiente con un while para que corte cuando lo encuentre
        List<CharacterInfo> listOfEnemies = null;
        //    Debug.Log("startingState.currentPlayer " + startingState.currentPlayer);
        if (startingState.currentPlayer == 1)
        {
            listOfEnemies = startingState.MyTeam;
        }
        else if (startingState.currentPlayer == 2)
        {
            listOfEnemies = startingState.EnemyTeam;
        }
        foreach (CharacterInfo enemy in listOfEnemies)
        {
            /*       Debug.Log("Buscando enemigo - " + enemy);
                   Debug.Log("Enemigo en la posicion: " + enemy.standingOnTile.grid2DLocation);
                   Debug.Log("Posicion de ataque: " + targetGrid2DLocation);*/
            if (enemy.standingOnTile.grid2DLocation == targetGrid2DLocation)
                enemyToAttack = enemy;
        }

        enemyToAttack.TakeDamage(unit.damage);
        if (enemyToAttack.currentHealth <= 0)
        {
            startingState.EnemyTeam.Remove(enemyToAttack);
            startingState.extraReward += 5;
        }

    }
}
