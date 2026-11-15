using UnityEngine;

public class Stats : MonoBehaviour
{
    //Basic stats for the Player Units and Enemy Units to draw from
    [Header("HP")]
    public int hp = 100;

    /*
     * Stat Breakdown:
     * HP (hp): Max  Health
     * Strength (strength): Used in physical damage calculation
     * Dexterity (dexterity): Used to Calculate hit chance
     * Agility (agility): Used to calculate dodge chance (Basically hit chance is a comparison of attackers dexterity and defender's agility)
     * Defence (defence): Used to calculate physical damage mitigation
     * Movement (movement): How many spaces a character can be move (This will be primarily dependant on class as opposed to scaling with level)
     * Minimum Range (minRange): Keeps track of a units minimum range
     * Maximum Range (maxRange): Keeps track a of unit's maximum range.
     */

    [Header("Skills")]
    public int strength = 10;
    public int dexterity = 10;
    public int agility = 15; // For enemies only this will also be used to calculate the order enemy movements move in.
    public int defence = 15;
    public int movement = 5;

    // Default to one because the bulk of the units in the game will be low range melee units
    public int minRange = 1;
    public int maxRange = 1;



    
}
