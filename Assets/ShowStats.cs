using UnityEngine;

public class ShowStats : MonoBehaviour
{
    public ResourceManager manager;

    public void ShowStatsButton()
    {
        Debug.Log("Combat Power: " + manager.combatPower);
        Debug.Log("Souls: " + manager.souls);
        Debug.Log("Gold: " + manager.gold);
        Debug.Log("Lives: " + manager.lives);
    }
}