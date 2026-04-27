using UnityEngine;
using TMPro;

public class CombatUI : MonoBehaviour
{
    public ResourceManager manager;

    [Header("Left Panel")]
    public TMP_Text trueSoulsText;
    public TMP_Text currentCombatText;

    [Header("Center Panel")]
    public TMP_Text enemyNameText;
    public TMP_Text enemyPowerText;
    public TMP_Text resultText;

    [Header("Dice")]
    public TMP_Text diceText;

    void OnEnable()
    {
        RefreshUI();
    }

    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        trueSoulsText.text =
            "True Souls: " + Mathf.FloorToInt(manager.trueSouls);

        // Read current displayed roll from dice text
        int shownRoll = 10;

        if (diceText != null && diceText.text != "-")
        {
            int.TryParse(diceText.text, out shownRoll);
        }

        float currentValue =
            manager.combatPower *
            manager.GetRollMultiplier(shownRoll);

        currentCombatText.text =
            "Current Combat: " +
            Mathf.RoundToInt(currentValue);

        enemyNameText.text =
            "Demon #" + manager.GetEnemyLevel();

        enemyPowerText.text =
            "Enemy Combat: " +
            Mathf.FloorToInt(manager.GetEnemyPower());

        if (diceText != null)
        {
            if (shownRoll == 10)
                diceText.text = "-";
            else
                diceText.text = shownRoll.ToString();
        }

        resultText.text = "Ready";
    }

    public void ShowRoll(int roll)
    {
        diceText.text = roll.ToString();

        float currentValue =
            manager.combatPower *
            manager.GetRollMultiplier(roll);

        currentCombatText.text =
            "Current Combat: " +
            Mathf.RoundToInt(currentValue);
    }

    public void ShowCurrentCombat(float value)
    {
        currentCombatText.text =
            "Current Combat: " +
            Mathf.RoundToInt(value);
    }

    public void ShowResult(string text)
    {
        resultText.text = text;

        // Keep current combat accurate after fight
        int shownRoll = 10;

        if (diceText != null && diceText.text != "-")
        {
            int.TryParse(diceText.text, out shownRoll);
        }

        float currentValue =
            manager.combatPower *
            manager.GetRollMultiplier(shownRoll);

        currentCombatText.text =
            "Current Combat: " +
            Mathf.RoundToInt(currentValue);
    }
}