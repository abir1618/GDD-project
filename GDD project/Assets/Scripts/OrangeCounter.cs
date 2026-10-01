using UnityEngine;
using UnityEngine.UI;

public class OrangeCounter : MonoBehaviour
{
    private Text currentOrangeText;

    void Start()
    {
        currentOrangeText = GetComponent<Text>();

        if (Player.instance != null)
        {
            currentOrangeText.text =
                Player.instance.currentOranges.ToString();
        }
    }

    void Update()
    {
        if (Player.instance != null)
        {
            currentOrangeText.text =
                Player.instance.currentOranges.ToString();
        }
    }
}