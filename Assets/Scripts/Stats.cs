using UnityEngine;

public class Stats : MonoBehaviour
{
    //Basic stats for the Player Units and Enemy Units to draw from
    [Header("HP")]
    public int hp = 100;

    /*
     * Stat Breakdown:
     * HP: Max  Health
     * Strength: Used in physical damage calculation
     * Dexterity: Used to Calculate hit chance
     * Agility: Used to calculate dodge chance (Basically hit chance is a comparison of attackers dexterity and defender's agility)
     * Defence: Used to calculate physical damage mitigation
     * Movement: How many spaces a character can be move (This will be primarily dependant on class as opposed to scaling with level)
     */

    [Header("Skills")]
    public int strength = 10;
    public int dexterity = 10;
    public int agility = 15;
    public int defence = 15;
    public int movement = 5;


    
}
