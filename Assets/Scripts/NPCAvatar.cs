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
        Long,
        Test
    }

    public enum ClothingColor {
        Guardian,
        Fencer,
        Priest,
        Raider
    }

    public Race race = Race.Human;
    public HairStyle hairStyle = HairStyle.Short;
    public ClothingColor clothingColor = ClothingColor.Guardian;

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
        // Pick the right texture name based on ClothingColor
        string texName;
        switch (clothingColor) {
            case ClothingColor.Guardian:
                texName = "texture_clothing_guardian";
                break;
            case ClothingColor.Fencer:
                texName = "texture_clothing_fencer";
                break;
            case ClothingColor.Priest:
                texName = "texture_clothing_priest";
                break;
            case ClothingColor.Raider:
                texName = "texture_clothing_raider";
                break;
            default:
                texName = "texture_clothing_guardian"; // fallback
                break;
        }

        Texture2D bodyTex = Resources.Load<Texture2D>($"clothing/{texName}");
        if (bodyTex == null) {
            Debug.LogWarning($"NPCAvatar: Clothing texture not found at clothing/{texName}");
        } else {
            if (torso != null)
                SetMainTextureExcludeWeapon(torso, bodyTex);
            if (lowerBody != null)
                SetMainTextureExcludeWeapon(lowerBody, bodyTex);
        }

        // Still apply skin fill color into the same material via MPB
        Color fillColor = AvatarPaletteRepo.GetPalette(race).skin;
        Color fillColor2 = AvatarPaletteRepo.GetPalette(race).skin2;

        if (torso != null)
            SetFillColorExcludeWeapon(torso, fillColor, fillColor2);
        if (lowerBody != null)
            SetFillColorExcludeWeapon(lowerBody, fillColor, fillColor2);
    }

    // Uses MaterialPropertyBlock to override _MainTex without swapping materials
    void SetMainTextureExcludeWeapon(Transform t, Texture tex) {
        string clean = CleanName(t.name);
        if (clean == "Weapon" || clean == "Accessories")
            return;

        Renderer r = t.GetComponent<Renderer>();
        if (r != null) {
            r.GetPropertyBlock(_mpb);
            _mpb.SetTexture("_MainTex", tex);
            r.SetPropertyBlock(_mpb);
        }

        foreach (Transform child in t)
            SetMainTextureExcludeWeapon(child, tex);
    }


    string getHairTexture(Race race, HairStyle hairStyle) {
        //if( race == Race.Dwarf ) {
        //    return "mask_head_dwarf_longhair";
        //} else {
        //    switch(hairStyle) {
        //        case HairStyle.Short: return "mask_head_base_shorthair";
        //        case HairStyle.Medium: return "mask_head_base_medhair";
        //        case HairStyle.Long: return "mask_head_base_longhair";
        //        case HairStyle.Test: return "texture_head_longhair_mask";
        //        default: return null;
        //    }
        //}
        return "mask_head_base_longhair";
    }

    void ApplyHairMaterial() {
        if (head == null) return;

        var p = AvatarPaletteRepo.GetPalette(race);

        string texName = getHairTexture(race, hairStyle);

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
        Transform helmetRaider = FindRecursive(head, "helmet_raider");

        // Clear old accessories
        if (earsElf != null) Destroy(earsElf.gameObject);
        if (earsDwarf != null) Destroy(earsDwarf.gameObject);
        if (earsCelt != null) Destroy(earsCelt.gameObject);
        if (hornsCelt != null) Destroy(hornsCelt.gameObject);
        if (hornsDragonman != null) Destroy(hornsDragonman.gameObject);
        if (beardDwarf != null) Destroy(beardDwarf.gameObject);
        if (helmetRaider != null) Destroy(helmetRaider.gameObject);

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

        // Raider helmet tied to ClothingColor
        if (clothingColor == ClothingColor.Raider) {
            GameObject prefab = Resources.Load<GameObject>("helmet/helmet_raider");
            if (prefab != null) {
                GameObject inst = Instantiate(prefab, head);
                inst.name = "helmet_raider"; // avoid " (Clone)" name issues with FindRecursive
            }
        }

        // Reapply palette so new parts get colored
        ApplyHeadPaletteToAllHeadParts();
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

    void SetFillColorExcludeWeapon(Transform t, Color c1, Color c2) {
        string clean = CleanName(t.name);
        if (clean == "Weapon" || clean == "Accessories")
            return;

        Renderer r = t.GetComponent<Renderer>();
        if (r != null && r.sharedMaterial != null) {
            r.GetPropertyBlock(_mpb);

            if (r.sharedMaterial.HasProperty("_FillColor"))
                _mpb.SetColor("_FillColor", c1);

            if (r.sharedMaterial.HasProperty("_FillColor2"))
                _mpb.SetColor("_FillColor2", c2);

            r.SetPropertyBlock(_mpb);
        }

        foreach (Transform child in t)
            SetFillColorExcludeWeapon(child, c1, c2);
    }

    string CleanName(string n) {
        int inst = n.IndexOf(" (Instance)");
        return inst >= 0 ? n.Substring(0, inst) : n;
    }
}
