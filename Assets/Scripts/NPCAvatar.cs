using UnityEngine;

public class NPCAvatar : MonoBehaviour
{
    public enum Race
    {
        Human,
        ElfM,
        Celt
    }

    public enum HairStyle
    {
        Short,
        Medium,
        Long
    }

    public enum ClothingColor
    {
        Blue,
        Green
    }

    public Race race = Race.Human;
    public HairStyle hairStyle = HairStyle.Short;
    public ClothingColor clothingColor = ClothingColor.Blue;

    Transform model;
    Transform upperBody;
    Transform head;
    Transform torso;
    Transform lowerBody;

    void Start()
    {
        model = transform.Find("Model");
        if (model != null)
        {
            upperBody = model.Find("Upper Body");
            lowerBody = model.Find("Lower Body");
            if (upperBody != null)
            {
                torso = upperBody.Find("Torso");
                head = upperBody.Find("Head");
            }
        }

        ApplyBodyMaterial();
        ApplyHairMaterial();
        ApplyAccessories();
    }

    void ApplyBodyMaterial()
    {
        string matName = $"Material_Clothing_Basic_{clothingColor}";
        Material bodyMat = Resources.Load<Material>($"clothing/{matName}");
        if (bodyMat == null)
            return;

        if (torso != null)
            ApplyToHierarchyExcludeWeapon(torso, bodyMat);

        if (lowerBody != null)
            ApplyToHierarchyExcludeWeapon(lowerBody, bodyMat);

        Color fillColor = GetRaceFillColor();

        if (torso != null)
            SetFillColorExcludeWeapon(torso, fillColor);

        if (lowerBody != null)
            SetFillColorExcludeWeapon(lowerBody, fillColor);
    }

    void ApplyHairMaterial()
    {
        if (head == null)
            return;

        string targetName = $"Material_Head_{hairStyle}_{race}";
        string folder = race.ToString().ToLower();
        string loadPath = $"hair/{folder}/{targetName}";
        Material raceMat = Resources.Load<Material>(loadPath);
        if (raceMat == null)
            return;

        Transform hair = head.Find("hair");
        Transform faceHead = head.Find("head");

        if (hair != null)
            ApplyToHierarchy(hair, raceMat);

        if (faceHead != null)
            ApplyToHierarchy(faceHead, raceMat);
    }

    void ApplyAccessories()
    {
        if (head == null)
            return;

        Transform earsElf = FindRecursive(head, "ears_elfm");
        Transform earsCelt = FindRecursive(head, "ears_celt");
        Transform hornsCelt = FindRecursive(head, "horns_celt");

        if (race == Race.ElfM)
        {
            if (earsElf == null)
            {
                GameObject prefab = Resources.Load<GameObject>("ears/ears_elfm");
                if (prefab != null)
                    Instantiate(prefab, head);
            }
            if (earsCelt != null) Destroy(earsCelt.gameObject);
            if (hornsCelt != null) Destroy(hornsCelt.gameObject);
        }
        else if (race == Race.Celt)
        {
            if (earsCelt == null)
            {
                GameObject prefab = Resources.Load<GameObject>("ears/ears_celt");
                if (prefab != null)
                    Instantiate(prefab, head);
            }
            if (hornsCelt == null)
            {
                GameObject prefab = Resources.Load<GameObject>("horns/horns_celt");
                if (prefab != null)
                    Instantiate(prefab, head);
            }
            if (earsElf != null) Destroy(earsElf.gameObject);
        }
        else
        {
            if (earsElf != null) Destroy(earsElf.gameObject);
            if (earsCelt != null) Destroy(earsCelt.gameObject);
            if (hornsCelt != null) Destroy(hornsCelt.gameObject);
        }
    }

    Material GetHairAdjusted(Material source)
    {
        return source;
    }

    Color GetRaceFillColor()
    {
        switch (race)
        {
            case Race.ElfM: return HexToColor("73B2CA");
            case Race.Human:
            case Race.Celt:
            default: return HexToColor("C7B28D");
        }
    }

    Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#" + hex, out color);
        return color;
    }

    Transform FindRecursive(Transform root, string name)
    {
        if (root.name == name) return root;
        foreach (Transform child in root)
        {
            Transform found = FindRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }

    void ApplyToHierarchy(Transform t, Material mat)
    {
        Renderer r = t.GetComponent<Renderer>();
        if (r != null)
            r.material = mat;
        foreach (Transform child in t)
            ApplyToHierarchy(child, mat);
    }

    void ApplyToHierarchyExcludeWeapon(Transform t, Material mat)
    {
        if (t.name == "Weapon")
            return;
        Renderer r = t.GetComponent<Renderer>();
        if (r != null)
            r.material = mat;
        foreach (Transform child in t)
            ApplyToHierarchyExcludeWeapon(child, mat);
    }

    void SetFillColorExcludeWeapon(Transform t, Color c)
    {
        if (t.name == "Weapon")
            return;
        Renderer r = t.GetComponent<Renderer>();
        if (r != null && r.material.HasProperty("_FillColor"))
            r.material.SetColor("_FillColor", c);
        foreach (Transform child in t)
            SetFillColorExcludeWeapon(child, c);
    }

    string CleanName(string n)
    {
        int inst = n.IndexOf(" (Instance)");
        return inst >= 0 ? n.Substring(0, inst) : n;
    }
}
