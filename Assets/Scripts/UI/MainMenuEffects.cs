using UnityEngine;

public class MainMenuEffects : MonoBehaviour
{
    void Start()
    {
        // 确保粒子系统播放并配置参数（作为 MCP 工具失败的所谓“后备”）
        var particles = GetComponentInChildren<ParticleSystem>();
        if (particles == null)
        {
            particles = FindObjectOfType<ParticleSystem>();
        }

        if (particles != null)
        {
            var main = particles.main;
            main.startLifetime = 10f;
            main.startSpeed = 0.2f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
            main.maxParticles = 100;
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(0f, 0.75f, 1f), // #00BFFF
                new Color(0.88f, 0.69f, 1f) // #E0B0FF
            );

            var emission = particles.emission;
            emission.rateOverTime = 10f;

            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(16f, 9f, 1f);

            if (!particles.isPlaying)
            {
                particles.Play();
            }
        }
    }
}
