using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterForm))]
public class PuppetPlates : MonoBehaviour
{
    public GameObject namePlateObject;
    public GameObject alignPlateObject;
    public GameObject hpPlateObject;
    public GameObject levelPlateObject;

    private CharacterForm form;

    private TextMeshPro namePlateText;
    private TextMeshPro alignPlateText;
    private TextMeshPro hpPlateText;
    private TextMeshPro levelPlateText;

    private bool nameInitialized = false;
    private bool alignmentInitialized = false;
    private bool hpInitialized = false;
    private bool levelInitialized = false;

    public bool Initialized => nameInitialized && alignmentInitialized && hpInitialized && levelInitialized;

    void Awake()
    {
        form = GetComponent<CharacterForm>();
        if (form == null)
        {
            Debug.LogError("CharacterForm not found on the same GameObject as PuppetPlates!");
        }

        if (namePlateObject) namePlateText = namePlateObject.GetComponent<TextMeshPro>();
        if (alignPlateObject) alignPlateText = alignPlateObject.GetComponent<TextMeshPro>();
        if (hpPlateObject) hpPlateText = hpPlateObject.GetComponent<TextMeshPro>();
        if (levelPlateObject) levelPlateText = levelPlateObject.GetComponent<TextMeshPro>();
    }

    void Start()
    {
        if (form != null)
        {
            UpdateNamePlate(form.DisplayName);
            UpdateAlignmentText(false);
            UpdateHPPlate(form.CurrentHP, form.MaxHP);
            UpdateLevelPlate(form.Level);
        }
    }

    public void UpdateNamePlate(string displayName)
    {
        if (namePlateText != null)
        {
            namePlateText.text = displayName;
            nameInitialized = true;
        }
    }

    public void UpdateAlignmentText(bool isUnknown)
    {
        if (alignPlateText == null) return;

        string alignmentText = "";

        if (isUnknown)
        {
            alignmentText = "[<color=#D3D3D3>Unknown</color>]";
        }
        else
        {
            switch (form.alignment)
            {
                case CharacterForm.Alignment.Peaceful:
                    alignmentText = "[<color=green>Peaceful</color>]";
                    break;
                case CharacterForm.Alignment.Neutral:
                    alignmentText = "[<color=yellow>Neutral</color>]";
                    break;
                case CharacterForm.Alignment.Hostile:
                    alignmentText = "[<color=red>Hostile</color>]";
                    break;
                case CharacterForm.Alignment.Ally:
                    alignmentText = "[<color=blue>Ally</color>]";
                    break;
                case CharacterForm.Alignment.Defending:
                    alignmentText = "[<color=blue>Defending</color>]";
                    break;
                case CharacterForm.Alignment.Special:
                    alignmentText = "[<color=purple>Special</color>]";
                    break;
            }
        }

        alignPlateText.text = alignmentText;
        alignmentInitialized = true;
    }

    public void UpdateHPPlate(float currentHP, float maxHP)
    {
        if (hpPlateText != null)
        {
            hpPlateText.text = $"HP: {currentHP}/{maxHP}";
            hpInitialized = true;
        }
    }

    public void UpdateLevelPlate(int level)
    {
        if (levelPlateText != null)
        {
            levelPlateText.text = $"Level: {level}";
            levelInitialized = true;
        }
    }
}
