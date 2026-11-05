using UnityEngine;

public class Stats : MonoBehaviour
{
    //Basic stats for the Player Units and Enemy Units to draw from
    [Header("HP")]
    public int hp = 100;

    [Header("Skills")]
    public int strength = 10;
    public int dexterity = 10;
    public int agility = 6;
    //This will be tied directly into movement, the agility number is used to show how many nodes (tiles) a unit can move, default is 6.
    public int defence = 15;
    
}
