using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float arrowSpeed = 5f;
    public float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}