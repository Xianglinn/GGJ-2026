using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ParticleSystem))]
public class MainMenuParticlesController : MonoBehaviour
{
    [Header("基础设置")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private int sortingOrder = 10; // 解决 UI 遮挡的关键
    [SerializeField] private string sortingLayerName = "UI";

    [Header("粒子参数")]
    public float lifetime = 10f;
    public float speed = 0.2f;
    public Vector2 sizeRange = new Vector2(0.1f, 0.3f);
    public Color colorStart = new Color(0f, 0.75f, 1f); // #00BFFF
    public Color colorEnd = new Color(0.88f, 0.69f, 1f);   // #E0B0FF

    private ParticleSystem _ps;

    void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        SetupParticleSystem();
    }

    void Start()
    {
        if (playOnStart && !_ps.isPlaying)
        {
            _ps.Play();
        }
    }

    /// <summary>
    /// 初始化粒子系统配置
    /// </summary>
    public void SetupParticleSystem()
    {
        // 1. 主模块配置
        var main = _ps.main;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = new ParticleSystem.MinMaxCurve(sizeRange.x, sizeRange.y);
        main.startColor = new ParticleSystem.MinMaxGradient(colorStart, colorEnd);
        main.maxParticles = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        // 2. 发射模块
        var emission = _ps.emission;
        emission.rateOverTime = 10f;

        // 3. 形状模块 (设为 Box 覆盖全屏)
        var shape = _ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(16f, 9f, 1f);

        // 4. 渲染器与层级修复 (解决“看不见”的问题)
        var psRenderer = _ps.GetComponent<ParticleSystemRenderer>();
        if (psRenderer != null)
        {
            // 设置排序层，确保在 UI 背景之上
            psRenderer.sortingLayerName = sortingLayerName;
            psRenderer.sortingOrder = sortingOrder;

            // 材质修复：仅在缺失材质时创建
            if (psRenderer.sharedMaterial == null || psRenderer.sharedMaterial.name.Contains("Default"))
            {
                psRenderer.material = CreateFallbackMaterial();
            }
        }
    }

    private Material CreateFallbackMaterial()
    {
        // 优先查找 URP Shader，其次是标准 Shader
        string[] shaderNames = {
            "Universal Render Pipeline/Particles/Unlit",
            "Particles/Standard Unlit",
            "Legacy Shaders/Particles/Alpha Blended"
        };

        foreach (var name in shaderNames)
        {
            Shader s = Shader.Find(name);
            if (s != null) return new Material(s);
        }
        return null;
    }
}