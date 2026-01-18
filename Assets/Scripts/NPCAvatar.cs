#nullable enable

using UnityEngine;
using System.Collections.Generic;

public class AvatarBody {
    public Transform? Model;
    public Transform? UpperBody;
    public Transform? Head;
    public Transform? Torso;
    public Transform? LowerBody;

    public AvatarBody(Transform model) {
        Model = model;
        if (Model != null) {
            UpperBody = Model.Find("Upper Body");
            LowerBody = Model.Find("Lower Body");
            if (UpperBody != null) {
                Torso = UpperBody.Find("Torso");
                Head = UpperBody.Find("Head");
            }
        }
    }
}

public class CharacterAvatar {
    public CharacterRace Race;
    public CharacterClass Class;
    public AvatarBody Body;

    public CharacterAvatar(CharacterRace characterRace, CharacterClass characterClass, AvatarBody avatarBody) {
        Race = characterRace;
        Class = characterClass;
        Body = avatarBody;
    }
}

public class NPCAvatar : MonoBehaviour {
    public enum HairStyle { Short, Medium, Long, Test }
    public HairStyle hairStyle = HairStyle.Short;

    CharacterAvatar _avatar = null!;

    MaterialPropertyBlock _mpb = null!;
    Texture2D? _headBaseTexture;

    CharacterForm _form = null!;

    void Awake() {
        _form = GetComponent<CharacterForm>();
        if (_form == null) {
            Debug.LogError("NPCAvatar: CharacterForm missing on same GameObject!", this);
            enabled = false;
            return;
        }
    }

    void Start() {
        // Ensure identity is resolved (in case something reordered execution / changed at runtime)
        if (_form.Race == null || _form.Class == null)
            _form.ResolveIdentity();

        if (_form.Race == null || _form.Class == null) {
            Debug.LogError("NPCAvatar: CharacterForm did not resolve Race/Class.", this);
            return;
        }

        _mpb = new MaterialPropertyBlock();

        Transform model = transform.Find("Model");
        if (model == null) {
            Debug.LogError("NPCAvatar: Model transform not found!", this);
            return;
        }

        _avatar = new CharacterAvatar(_form.Race, _form.Class, new AvatarBody(model));

        ApplyBodyMaterial();
        ApplyHairMaterial();
        ApplyOrnaments();
    }

    void ApplyBodyMaterial() {
        // Use resolved class directly
        var characterClass = _form.Class!;
        string clothingTexture = characterClass.BaseEquipment.ArmorSet.View.Clothing;

        Texture2D textureFound = Resources.Load<Texture2D>($"clothing/texture_{clothingTexture}");
        if (textureFound == null) {
            Debug.LogWarning($"NPCAvatar: Clothing texture not found at clothing/texture_{clothingTexture}", this);
        } else {
            if (_avatar.Body.Torso != null) SetMainTextureExcludeWeapon(_avatar.Body.Torso, textureFound);
            if (_avatar.Body.LowerBody != null) SetMainTextureExcludeWeapon(_avatar.Body.LowerBody, textureFound);
        }

        // Skin fill colors from resolved race
        var race = _form.Race!;
        Color fillColor = race.Palette.skin;
        Color fillColor2 = race.Palette.skin2;

        if (_avatar.Body.Torso != null)
            MaterialPropertyUtil.SetFillColor(_avatar.Body.Torso, fillColor, fillColor2, _mpb);
        if (_avatar.Body.LowerBody != null)
            MaterialPropertyUtil.SetFillColor(_avatar.Body.LowerBody, fillColor, fillColor2, _mpb);
    }

    void SetMainTextureExcludeWeapon(Transform t, Texture tex) {
        string clean = EquipmentNameUtil.CleanName(t.name);
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

    string getHairTexture(HairStyle style) {
        // Example: use resolved race enum if you need branching
        // (Assuming CharacterRace has Enum like before; otherwise adjust.)
        // if (_form.Race!.Enum == RaceEnum.Dwarf) return "mask_head_base_longhair";
        return "mask_head_base_longhair";
    }

    void ApplyHairMaterial() {
        if (_avatar.Body.Head == null) return;

        RacePalette palette = _form.Race!.Palette;
        string texName = getHairTexture(hairStyle);

        _headBaseTexture = string.IsNullOrEmpty(texName)
            ? null
            : Resources.Load<Texture2D>($"hair/{texName}");

        Transform hair = _avatar.Body.Head.Find("hair");
        Transform faceHead = _avatar.Body.Head.Find("head");

        if (hair != null)
            AvatarPaletteUtil.ApplyHeadPaletteToHierarchy(hair, _mpb, palette, _headBaseTexture);

        if (faceHead != null)
            AvatarPaletteUtil.ApplyHeadPaletteToHierarchy(faceHead, _mpb, palette, _headBaseTexture);
    }

    void ApplyHeadPalette() {
        if (_avatar.Body.Head == null) return;

        var palette = _form.Race!.Palette;

        var renderers = _avatar.Body.Head.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) {
            var m = r.sharedMaterial;
            if (!AvatarPaletteUtil.IsPaletteMaterial(m))
                continue;

            AvatarPaletteUtil.ApplyHeadPalette(r, _mpb, palette, null);
        }
    }

    void ApplyOrnaments() {
        FeatureRenderUtil.RenderEars(_avatar);
        FeatureRenderUtil.RenderHorns(_avatar);
        FeatureRenderUtil.RenderBeard(_avatar);

        EquipmentRenderUtil.RenderHelmet(_avatar);
        EquipmentRenderUtil.RenderPauldrons(_avatar);
        EquipmentRenderUtil.RenderCloak(_avatar);
        EquipmentRenderUtil.RenderWeapon(_avatar);

        FeatureRenderUtil.RenderMutations(_avatar);

        ApplyHeadPalette();
    }
}