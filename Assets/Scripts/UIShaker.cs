using UnityEngine;

public class UIShaker : MonoBehaviour
{
    bool running = true;
    Vector3 originPosition;
    Quaternion originRotation;

    public float shake_decay = 0.002f;
    public float shake_intensity = .4f;

    float temp_shake_intensity = 0;
    void Update()
    {
        if (running)
        {
            if (temp_shake_intensity > 0)
            {
                //transform.localPosition = originPosition + Random.insideUnitSphere * temp_shake_intensity;
                transform.rotation = new Quaternion(
                    originRotation.x + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                    originRotation.y + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                    originRotation.z + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                    originRotation.w + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f);
                temp_shake_intensity -= shake_decay;
            }
            else
            {
                Shake();
            }
        }
    }
    void Shake()
    {
        //originPosition = transform.localPosition;
        originRotation = transform.rotation;
        temp_shake_intensity = shake_intensity;

    }
    void Start()
    {
        Shake();
    }

    public void Pause()
    {
        running = false;
        gameObject.transform.rotation = Quaternion.identity;
        temp_shake_intensity = 0;
    }

    public void Resume()
    {
        running = true;
        Shake();
    }
}
