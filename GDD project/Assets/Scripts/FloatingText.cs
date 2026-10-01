using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TextMesh textMesh;

    void Start()
    {
        Destroy(gameObject, 1.01f);

        int randomNumber = Random.Range(1, 101);
        textMesh.text = randomNumber.ToString();
    }
}