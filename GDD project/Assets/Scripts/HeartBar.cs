using UnityEngine;
using UnityEngine.UI;

public class HeartBar : MonoBehaviour
{
    public Image[] hearts;

    void Update()
    {
        if (Player.instance == null)
            return;

        int health = Player.instance.maxHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < health;
        }
    }
}