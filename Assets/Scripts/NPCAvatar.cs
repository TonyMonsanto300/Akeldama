using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RacePalette {
    public Color hair1;
    public Color hair2;
    public Color eyes;
    public Color skin;   // skin1
    public Color skin2;
    public Color paint;  // defaults to skin

    public RacePalette(string hair1Hex, string hair2Hex, string eyesHex, string skinHex, string skin2Hex, string paintHex = null) {
        hair1 = Parse(hair1Hex);
        hair2 = Parse(hair2Hex);
        eyes = Parse(eyesHex);
        skin = Parse(skinHex);
        skin2 = Parse(skin2Hex);
        paint = string.IsNullOrEmpty(paintHex) ? skin : Parse(paintHex);
    }

    static Color Parse(string hex) {
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        return c;
    }
}

public class NPCAvatar : MonoBehaviour {
    public enum Race {
        Human,
        ElfM,   // regular elf
        ElfD,   // dark elf
        ElfA,   // ancient elf
        Celt,
        Dragonman,
        Dwarf   // uses elf ears prefab
    }

    public enum HairStyle {
        Short,
        Medium,
        Long
    }

    public enum ClothingColor {
        Blue,
        Green,
        Draconic
    }

    public Race race = Race.Human;
    public HairStyle hairStyle = HairStyle.Short;
    public ClothingColor clothingColor = ClothingColor.Blue;

    Transform model;
    Transform upperBody;
    Transform head;
    Transform torso;
    Transform lowerBody;

    MaterialPropertyBlock _mpb;
    Texture2D _headBaseTex;

    void Start() {
        _mpb = new MaterialPropertyBlock();

        model = transform.Find("Model");
        if (model != null) {
            upperBody = model.Find("Upper Body");
            lowerBody = model.Find("Lower Body");
            if (upperBody != null) {
                torso = upperBody.Find("Torso");
                head = upperBody.Find("Head");
            }
        }

        ApplyBodyMaterial();
        ApplyHairMaterial();
        ApplyAccessories();
    }

    void ApplyBodyMaterial() {
        string matName =
            (clothingColor == ClothingColor.Draconic)
                ? "Material_Clothing_Draconic_Green"
                : $"Material_Clothing_Basic_{clothingColor}";

        Material bodyMat = Resources.Load<Material>($"clothing/{matName}");
        if (bodyMat == null) return;

        if (torso != null)
            ApplyToHierarchyExcludeWeapon(torso, bodyMat);
        if (lowerBody != null)
            ApplyToHierarchyExcludeWeapon(lowerBody, bodyMat);

        Color fillColor = AvatarPaletteRepo.GetPalette(race).skin;

        if (torso != null)
            SetFillColorExcludeWeapon(torso, fillColor);
        if (lowerBody != null)
            SetFillColorExcludeWeapon(lowerBody, fillColor);
    }

    void ApplyHairMaterial() {
        if (head == null) return;

        var p = AvatarPaletteRepo.GetPalette(race);

        string texName = null;
        if (race == Race.Dwarf) {
            texName = "mask_head_dwarf_longhair";
        } else {
            switch (hairStyle) {
                case HairStyle.Short: texName = "mask_head_base_shorthair"; break;
                case HairStyle.Medium: texName = "mask_head_base_medhair"; break;
                case HairStyle.Long: texName = "mask_head_base_longhair"; break;
            }
        }

        _headBaseTex = string.IsNullOrEmpty(texName)
            ? null
            : Resources.Load<Texture2D>($"hair/{texName}");

        Transform hair = head.Find("hair");
        Transform faceHead = head.Find("head");

        if (hair != null)
            AvatarPaletteUtil.ApplyHeadPaletteToHierarchy(hair, _mpb, p, _headBaseTex);

        if (faceHead != null)
            AvatarPaletteUtil.ApplyHeadPaletteToHierarchy(faceHead, _mpb, p, _headBaseTex);
    }

    void ApplyHeadPaletteToAllHeadParts() {
        if (head == null) return;

        var p = AvatarPaletteRepo.GetPalette(race);

        // This is your "apply to everything under Head" pass used after spawning accessories.
        var renderers = head.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) {
            var m = r.sharedMaterial;
            if (!AvatarPaletteUtil.IsPaletteMaterial(m))
                continue;

            AvatarPaletteUtil.ApplyHeadPalette(r, _mpb, p, null);
        }
    }

    void ApplyAccessories() {
        if (!head) return;

        Transform earsElf = FindRecursive(head, "ears_elfm");
        Transform earsDwarf = FindRecursive(head, "ears_dwarf");
        Transform earsCelt = FindRecursive(head, "ears_celt");
        Transform hornsCelt = FindRecursive(head, "horns_celt");
        Transform hornsDragonman = FindRecursive(head, "horns_dragonman");
        Transform beardDwarf = FindRecursive(head, "beard_dwarf");

        if (earsElf != null) Destroy(earsElf.gameObject);
        if (earsDwarf != null) Destroy(earsDwarf.gameObject);
        if (earsCelt != null) Destroy(earsCelt.gameObject);
        if (hornsCelt != null) Destroy(hornsCelt.gameObject);
        if (hornsDragonman != null) Destroy(hornsDragonman.gameObject);
        if (beardDwarf != null) Destroy(beardDwarf.gameObject);

        bool usesElfEars = (race == Race.ElfM || race == Race.ElfD || race == Race.ElfA);
        bool usesDwarf = (race == Race.Dwarf);

        if (usesElfEars) {
            GameObject prefab = Resources.Load<GameObject>("ears/ears_elfm");
            if (prefab != null)
                Instantiate(prefab, head);
        } else if (usesDwarf) {
            GameObject prefab = Resources.Load<GameObject>("ears/ears_dwarf");
            if (prefab != null)
                Instantiate(prefab, head);

            prefab = Resources.Load<GameObject>("beard/beard_dwarf");
            if (prefab != null)
                Instantiate(prefab, head);
        } else if (race == Race.Celt) {
            GameObject prefab = Resources.Load<GameObject>("ears/ears_celt");
            if (prefab != null)
                Instantiate(prefab, head);

            prefab = Resources.Load<GameObject>("horns/horns_celt");
            if (prefab != null)
                Instantiate(prefab, head);
        } else if (race == Race.Dragonman) {
            GameObject prefab = Resources.Load<GameObject>("horns/horns_dragonman");
            if (prefab != null)
                Instantiate(prefab, head);
        }

        // Now that we've spawned/removed head parts, reapply palette to the whole head.
        ApplyHeadPaletteToAllHeadParts();

        if (upperBody != null) {
            Transform accessories = upperBody.Find("Accessories");
            if (accessories == null)
                accessories = upperBody;

            Transform redCloak = FindRecursive(accessories, "red_cloak");

            if (clothingColor == ClothingColor.Draconic) {
                if (redCloak == null) {
                    GameObject cloakPrefab = Resources.Load<GameObject>("clothing/accessories/red_cloak");
                    if (cloakPrefab != null) {
                        GameObject cloakInstance = Instantiate(cloakPrefab, accessories);
                        cloakInstance.name = "red_cloak";
                    }
                }
            } else {
                if (redCloak != null)
                    Destroy(redCloak.gameObject);
            }
        }
    }

    Transform FindRecursive(Transform root, string name) {
        if (root.name == name) return root;
        foreach (Transform child in root) {
            Transform found = FindRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }

    void ApplyToHierarchyExcludeWeapon(Transform t, Material mat) {
        string clean = CleanName(t.name);
        if (clean == "Weapon" || clean == "Accessories")
            return;

        Renderer r = t.GetComponent<Renderer>();
        if (r != null)
            r.sharedMaterial = mat;

        foreach (Transform child in t)
            ApplyToHierarchyExcludeWeapon(child, mat);
    }

    void SetFillColorExcludeWeapon(Transform t, Color c) {
        string clean = CleanName(t.name);
        if (clean == "Weapon" || clean == "Accessories")
            return;

        Renderer r = t.GetComponent<Renderer>();
        if (r != null && r.sharedMaterial != null && r.sharedMaterial.HasProperty("_FillColor")) {
            r.GetPropertyBlock(_mpb);
            _mpb.SetColor("_FillColor", c);
            r.SetPropertyBlock(_mpb);
        }

        foreach (Transform child in t)
            SetFillColorExcludeWeapon(child, c);
    }

    string CleanName(string n) {
        int inst = n.IndexOf(" (Instance)");
        return inst >= 0 ? n.Substring(0, inst) : n;
    }
}
