using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    public CinemachineVirtualCamera cam;

    private float shakeTime;
    private CinemachineBasicMultiChannelPerlin perlin;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        perlin = cam.GetCinemachineComponent<
            CinemachineBasicMultiChannelPerlin>();
    }


    void Update()
    {
        if (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;

            if (shakeTime <= 0f)
            {
                perlin.m_AmplitudeGain = 0f;
            }
        }
    }


    public void Shake(float intensity, float duration)
    {
        shakeTime = duration;

        perlin.m_AmplitudeGain = intensity;
    }
}