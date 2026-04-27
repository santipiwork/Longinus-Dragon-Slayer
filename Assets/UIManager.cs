using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject combatPanel;
    public CombatUI combatUI;

    public void OpenCombatPanel()
    {
        combatPanel.SetActive(true);
        combatUI.RefreshUI();
    }

    public void CloseCombatPanel()
    {
        combatPanel.SetActive(false);
    }
}
