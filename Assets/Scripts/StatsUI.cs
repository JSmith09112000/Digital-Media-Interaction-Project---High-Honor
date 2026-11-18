using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{

    [Header("UI Stats References")]

    public GameObject statsPanel;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI dexterityText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI defenceText;
    public TextMeshProUGUI movementText;

    private Stats currentUnit;
    private PlayerHealth playerHealth;
    private EnemyHealth enemyHealth;



    //Ensures the stats ui is hidden by default
    void Start()
    {
        HideStats();
    }

    public void ShowStats(Stats unit)
    {
        currentUnit = unit;

        playerHealth = unit.GetComponent<PlayerHealth>();
        enemyHealth = unit.GetComponent<EnemyHealth>();

        statsPanel.SetActive(true);
        Update();
    }

    public void HideStats()
    {
        //Hides and nullifies stats when they are not being hovered over, clearing the UI box
        currentUnit = null;
        playerHealth = null;
        enemyHealth = null;
        statsPanel.SetActive(false);
    }

    
    void Update()
    {
        //Checks whether a player or an enemies health needs to be displayed based on what the Unit has available.
        if(playerHealth != null)
        {
            healthText.text = "Health: " + playerHealth.currentHealth;
        }
        else if (enemyHealth != null)
        {
            healthText.text = "Health: " + enemyHealth.currentHealth;
        }


        strengthText.text = "Strength: " + currentUnit.strength;
        dexterityText.text = "Dexterity: " + currentUnit.dexterity;
        agilityText.text = "Agility: " + currentUnit.agility;
        defenceText.text = "Defence: " + currentUnit.defence;
        movementText.text = "Movement: " + currentUnit.movement;

        
    }
}
