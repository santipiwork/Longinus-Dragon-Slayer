using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainUI : MonoBehaviour
{
    public Slider slider;
    public TMP_Text amountText;
    public ResourceManager manager;

    void Update()
    {
        amountText.text = "Train x" + slider.value;
    }

    public void BuyTrain()
    {
        manager.BuyCombatPowerAmount((int)slider.value);
    }
}