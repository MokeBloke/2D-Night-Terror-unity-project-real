using UnityEngine;
using UnityEngine.UI;

public class bossHealthBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxHealth(int Health)
    {
        slider.maxValue = Health;
        slider.value = Health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

}
