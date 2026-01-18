using UnityEngine;

public class MaterialPropertyUtil {
    public static void SetFillColor(Transform t, Color c1, Color c2, MaterialPropertyBlock mpb) {
        Renderer r = t.GetComponent<Renderer>();
        if (r != null && r.sharedMaterial != null) {
            r.GetPropertyBlock(mpb);

            if (r.sharedMaterial.HasProperty("_FillColor"))
                mpb.SetColor("_FillColor", c1);

            if (r.sharedMaterial.HasProperty("_FillColor2"))
                mpb.SetColor("_FillColor2", c2);

            r.SetPropertyBlock(mpb);
        }

        foreach (Transform child in t) {
            SetFillColor(child, c1, c2, mpb);
        }
    }
}