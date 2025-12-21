using UnityEngine;

public static class AvatarPaletteUtil {
    public static void ApplyHeadPalette(
        Renderer r,
        MaterialPropertyBlock mpb,
        RacePalette p,
        Texture2D baseTex = null
    ) {
        if (r == null) return;

        var m = r.sharedMaterial;
        if (m == null) return;

        r.GetPropertyBlock(mpb);

        if (m.HasProperty("_HairColor1")) mpb.SetColor("_HairColor1", p.hair1);
        if (m.HasProperty("_HairColor2")) mpb.SetColor("_HairColor2", p.hair2);
        if (m.HasProperty("_EyeColor")) mpb.SetColor("_EyeColor", p.eyes);
        if (m.HasProperty("_SkinColor")) mpb.SetColor("_SkinColor", p.skin);
        if (m.HasProperty("_SkinColor2")) mpb.SetColor("_SkinColor2", p.skin2);
        if (m.HasProperty("_PaintColor")) mpb.SetColor("_PaintColor", p.paint);

        if (baseTex != null && m.HasProperty("_MainTex"))
            mpb.SetTexture("_MainTex", baseTex);

        r.SetPropertyBlock(mpb);
    }

    // Convenience: apply palette to all renderers under a transform.
    public static void ApplyHeadPaletteToHierarchy(
        Transform root,
        MaterialPropertyBlock mpb,
        RacePalette p,
        Texture2D baseTex = null
    ) {
        if (root == null) return;

        var renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach(var r in renderers)
        ApplyHeadPalette(r, mpb, p, baseTex);
    }

    // Checks if a material looks like one of your "palette-driven" head materials.
    public static bool IsPaletteMaterial(Material m) {
        if (m == null) return false;

        return
        m.HasProperty("_SkinColor") ||
            m.HasProperty("_SkinColor2") ||
            m.HasProperty("_EyeColor") ||
            m.HasProperty("_HairColor1") ||
            m.HasProperty("_HairColor2") ||
            m.HasProperty("_PaintColor");
    }
}
