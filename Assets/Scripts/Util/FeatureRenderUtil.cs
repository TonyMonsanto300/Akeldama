#nullable enable

using UnityEngine;

public class FeatureRenderUtil : MonoBehaviour {
    public static void RenderEars(CharacterAvatar avatar) {
        if (avatar.Body.Head == null) return;

        // TODO: add Ears string to CharacterRace and load based on that instead of hardcoding here
        bool usesElfEars = avatar.Race.Name.Contains("Elf");
        GameObject? prefab = null;

        // TODO: Refactor to put ears as a feature of CharacterRace instead of hardcoding here
        if (usesElfEars) {
            prefab = Resources.Load<GameObject>("ears/ears_elfm");
        } else if (avatar.Race.Enum == RaceEnum.Dwarf) {
            prefab = Resources.Load<GameObject>("ears/ears_dwarf");
        } else if (avatar.Race.Enum == RaceEnum.Celt) {
            prefab = Resources.Load<GameObject>("ears/ears_celt");
        } else if (avatar.Race.Enum == RaceEnum.Dragonman) {
            prefab = Resources.Load<GameObject>("ears/ears_dragonman");
        }
            // Class override
            if (avatar.Class.Enum == ClassEnum.WITCH) {
            prefab = Resources.Load<GameObject>("ears/ears_witch");
        }

        if (prefab != null) {
            var inst = Instantiate(prefab, avatar.Body.Head);
            inst.name = EquipmentNameUtil.CleanName(prefab.name);
        }
    }

    public static void RenderHorns(CharacterAvatar avatar) {
        if (avatar.Body.Head == null) return;

        GameObject? prefab = null;

        if (avatar.Race.Enum == RaceEnum.Celt) {
            prefab = Resources.Load<GameObject>("horns/horns_celt");
        } else if (avatar.Race.Enum == RaceEnum.Dragonman) {
            prefab = Resources.Load<GameObject>("horns/horn_dragonman");
        }

        if (prefab != null) {
            var inst = Instantiate(prefab, avatar.Body.Head);
            inst.name = EquipmentNameUtil.CleanName(prefab.name);
        }
    }

    public static void RenderBeard(CharacterAvatar avatar) {
        if (avatar.Body.Head == null) return;

        if (avatar.Race.Enum == RaceEnum.Dwarf) {
            GameObject? prefab = Resources.Load<GameObject>("beard/beard_dwarf");
            if (prefab != null) {
                var inst = Instantiate(prefab, avatar.Body.Head);
                inst.name = EquipmentNameUtil.CleanName(prefab.name);
            }
        }
    }


    public static void RenderMutations(CharacterAvatar avatar) {
        for (int i = 0; i < avatar.Class.Mutations.Length; i++) {
            string mutation = avatar.Class.Mutations[i];
            Debug.Log($"Rendering: {avatar.Class.Mutations[i]}");
            if (avatar.Body.Head != null) {
                GameObject prefab = Resources.Load<GameObject>($"mutation/{mutation}");
                Debug.Log(prefab);
                if (prefab != null) {
                    var inst = Instantiate(prefab, avatar.Body.Head);
                    inst.name = EquipmentNameUtil.CleanName(prefab.name);
                }
            }
        }
    }
}
