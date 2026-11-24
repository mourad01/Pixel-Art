using UnityEngine;

public class RadialParticlesScript : MonoBehaviour
{
    public int numParticles = 8;
    public float maxRadius = 2f;
    public float processLifetime = 1f;
    public float particleSize = 1f;
    public int loops = 3;

    float currDegrees;
    float curveRate;
    float currRadius;
    int currLoop;
    bool deactivateOnComplete = true;
    ParticleSystem.Particle[] particles;

    void Start()
    {
        particles = new ParticleSystem.Particle[numParticles];
        curveRate = 90f / processLifetime;
        currLoop = loops;
        Reset(deactivateOnComplete);
        gameObject.SetActive(false);
    }

    void Update()
    {
        currDegrees += curveRate * Time.smoothDeltaTime;
        currRadius = Mathf.Sin(0.0174532924f * currDegrees) * maxRadius;
        currRadius = Mathf.Clamp(currRadius * 1.5f, 0f, maxRadius);
        UpdateParticles(currRadius);
        if (currDegrees > 90f)
        {
            currLoop--;
            if (currLoop > 0)
            {
                Reset(deactivateOnComplete);
            }
            else if (deactivateOnComplete)
            {
                gameObject.SetActive(false);
            }
            else
            {
                enabled = false;
            }
        }
    }

    public void Reset(bool deactivateGO)
    {
        deactivateOnComplete = deactivateGO;
        currDegrees = 0f;
        currRadius = Mathf.Sin(0.0174532924f * currDegrees) * maxRadius;
        for (int i = 0; i < numParticles; i++)
        {
            var val = particles[i];
            val.position = new Vector3(0f, 0f, transform.position.z);
            particles[i].color = new Color(1f, 1f, 1f, 1f);
            particles[i].size = particleSize;
            particles[i].remainingLifetime = processLifetime;
            particles[i].startLifetime = processLifetime;
        }
        base.GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
        gameObject.SetActive(true);
    }

    void UpdateParticles(float radius)
    {
        float a = 1f;
        float offset = 0f;
        float step = 6.28318548f / (float)numParticles;
        for (int i = 0; i < numParticles; i++)
        {
            float x = Mathf.Cos(step * (float)i) * radius;
            offset = Mathf.Sin(step * (float)i) * radius;
            var val = particles[i];
            val.position = new Vector3(x, offset, transform.position.z);
            if (particles[i].remainingLifetime < processLifetime * 0.5f)
            {
                a = particles[i].remainingLifetime / processLifetime;
            }
            particles[i].color = new Color(1f, 1f, 1f, a);
            particles[i].size = particleSize;
            particles[i].remainingLifetime -= Time.smoothDeltaTime;
        }
        base.GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
    }
}


