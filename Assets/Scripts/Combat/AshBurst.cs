using UnityEngine;

public static class AshBurst
{
    static Material mat;
    static Texture2D dot;

    public static void Play(Vector3 center, Vector3 size, Color tint)
    {
        GameObject go = new GameObject("AshBurst");
        go.SetActive(false);
        go.transform.position = center;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.playOnAwake = false;
        main.duration = 1.2f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.0f, 2.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.18f);
        main.startColor = new ParticleSystem.MinMaxGradient(tint, new Color(tint.r * 0.55f, tint.g * 0.55f, tint.b * 0.55f, 1f));
        main.gravityModifier = -0.03f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 500;
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)170) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(Mathf.Max(0.3f, size.x), Mathf.Max(0.3f, size.y), Mathf.Max(0.3f, size.z));

        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.World;
        vel.x = new ParticleSystem.MinMaxCurve(-0.4f, 0.4f);
        vel.y = new ParticleSystem.MinMaxCurve(0.8f, 2.0f);
        vel.z = new ParticleSystem.MinMaxCurve(-0.4f, 0.4f);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.6f;
        noise.frequency = 0.5f;
        noise.scrollSpeed = 0.4f;

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.12f),
                new GradientAlphaKey(0.55f, 0.55f),
                new GradientAlphaKey(0f, 1f)
            });
        col.color = g;

        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.1f)));

        var rot = ps.rotationOverLifetime;
        rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(-90f, 90f);

        ParticleSystemRenderer r = go.GetComponent<ParticleSystemRenderer>();
        r.material = AshMaterial();
        r.renderMode = ParticleSystemRenderMode.Billboard;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;

        go.SetActive(true);
        ps.Play();
    }

    static Material AshMaterial()
    {
        if (mat != null) return mat;

        Shader sh = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (sh == null) sh = Shader.Find("Legacy Shaders/Particles/Additive");
        if (sh == null) sh = Shader.Find("Sprites/Default");

        mat = new Material(sh);
        Texture2D d = DotTexture();
        if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", d);
        if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", d);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.white);
        mat.color = Color.white;

        if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);
        if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 2f);
        if (mat.HasProperty("_SrcBlend")) mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        if (mat.HasProperty("_DstBlend")) mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
        if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = 3000;

        return mat;
    }

    static Texture2D DotTexture()
    {
        if (dot != null) return dot;

        int s = 32;
        dot = new Texture2D(s, s, TextureFormat.RGBA32, false);
        float half = s * 0.5f;
        for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float dx = (x + 0.5f - half) / half;
                float dy = (y + 0.5f - half) / half;
                float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                a *= a;
                dot.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        dot.Apply();
        return dot;
    }
}
