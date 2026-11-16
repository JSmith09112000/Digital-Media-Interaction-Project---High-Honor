using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public void Battle(GameObject attackingUnit, GameObject defendingUnit)
    {
        bool isEnemy = false;
        // Some parts of the method will rely on knowing if the attacker is the player or the enemy.
        if (attackingUnit.TryGetComponent<EnemyAI>(out _))
        {
            isEnemy = true;
        }

        // Minimum the player needs to roll in order to hit the defending unit.
        int minimumToHit = 50 + defendingUnit.GetComponent<Stats>().agility;

        // Attacker rolls a "hit dice" with a minimum of 10 and a max of 100 then adds their dexterity to see if they hit
        int hitRoll = Random.Range(10, 101) + attackingUnit.GetComponent<Stats>().dexterity;

        // Will probably need to be seperated from this method to actually be used but for now it's in here to show how hit chance can be calculated
        int hitChance = ((101 - (minimumToHit - attackingUnit.GetComponent<Stats>().dexterity)) / 91 * 100);
        Debug.Log("Hit chance: " + hitChance);
        Debug.Log("Minimum To Hit: " + minimumToHit);
        Debug.Log("hitRoll: " + hitRoll);

        if(hitRoll >= minimumToHit)
        {
            if (isEnemy)
            {
                // If weapons are added then this can be changed to take them into account.
                int attackingUnitStrength = attackingUnit.GetComponent<Stats>().strength;
                int defendingUnitDefence = defendingUnit.GetComponent<Stats>().defence;

                // We will start with damage not being random. Subject to change during playtesting phase.
                int damage = (2 * attackingUnitStrength) - defendingUnitDefence;
                defendingUnit.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
            else
            {
                // If weapons are added then this can be changed to take them into account.
                int attackingUnitStrength = attackingUnit.GetComponent<Stats>().strength;
                int defendingUnitDefence = defendingUnit.GetComponent<Stats>().defence;

                // We will start with damage not being random. Subject to change during playtesting phase.
                int damage = (2 * attackingUnitStrength) - defendingUnitDefence;
                defendingUnit.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }

    }
}
