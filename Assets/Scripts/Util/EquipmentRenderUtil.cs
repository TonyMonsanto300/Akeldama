#nullable enable

using UnityEngine;
using UnityEngine.WSA;

public class EquipmentRenderUtil : MonoBehaviour {
    public static void RenderHelmet(CharacterAvatar avatar) {
        if (avatar.Class.BaseEquipment.Helmet != null) {
            string prefabPath = avatar.Class.BaseEquipment.Helmet.Model;
            GameObject prefab = Resources.Load<GameObject>($"helmet/{prefabPath}");
            if (prefab != null) {
                var inst = Instantiate(prefab, avatar.Body.Head);
                inst.name = EquipmentNameUtil.CleanName(prefab.name);
            }
        }
        if (avatar.Class.Enum == ClassEnum.GUARDIAN && avatar.Race.Enum == RaceEnum.Celt) {
            // Celt version of Guardian helmet is not horned
            Transform inst = TransformUtil.FindRecursive(avatar.Body.Head, "helmet_guardian");
            if (inst != null) {
                Transform hornLeft = TransformUtil.FindRecursive(inst.transform, "horn-left");
                Transform hornRight = TransformUtil.FindRecursive(inst.transform, "horn-right");
                if (hornLeft != null) Destroy(hornLeft.gameObject);
                if (hornRight != null) Destroy(hornRight.gameObject);
            }
        }

        //if Helmet.HideHair is true, disable the mesh on the "Hair" child of Head
        if (avatar.Class.BaseEquipment.Helmet != null && avatar.Class.BaseEquipment.Helmet.HideHair) {
            Transform? hair = avatar.Body.Head.Find("hair");
            if (hair != null) {
                MeshRenderer? hairMesh = hair.GetComponent<MeshRenderer>();
                if (hairMesh != null) {
                    hairMesh.enabled = false;
                }
            }
            Transform? beard = avatar.Body.Head.Find("beard_dwarf");
            if (beard != null) {
                MeshRenderer? beardMesh = beard.GetComponent<MeshRenderer>();
                if (beardMesh != null) {
                    beardMesh.enabled = false;
                }
            }
        }
    }
    public static void RenderPauldrons(CharacterAvatar avatar) {
        if (avatar.Class.BaseEquipment.ArmorSet.View.Pauldrons != null) {
            string? prefabPath = avatar.Class.BaseEquipment.ArmorSet.View.Pauldrons;
            if (avatar.Body.Torso == null || prefabPath == null) {
                return;
            }

            GameObject pauldronsPrefab = Resources.Load<GameObject>($"pauldrons/{prefabPath}");
            if (pauldronsPrefab == null) {
                Debug.LogWarning($"NPCAvatar: Pauldrons prefab not found at {prefabPath}");
                return;
            }

            // Spawn as child of torso so it inherits the correct rig space
            GameObject tempRoot = Instantiate(pauldronsPrefab, avatar.Body.Torso);
            tempRoot.name = EquipmentNameUtil.CleanName(pauldronsPrefab.name) + "_root";

            Transform leftArm = avatar.Body.Torso.Find("Left Arm");
            Transform rightArm = avatar.Body.Torso.Find("Right Arm");

            Transform leftPauldron = TransformUtil.FindRecursive(tempRoot.transform, "pauldron-left");
            Transform rightPauldron = TransformUtil.FindRecursive(tempRoot.transform, "pauldron-right");

            if (leftArm != null && leftPauldron != null) {
                leftPauldron.SetParent(leftArm, true); // keep world-space pose
            }

            if (rightArm != null && rightPauldron != null) {
                rightPauldron.SetParent(rightArm, true);
            }

            // Now that both children are reparented, kill the prefab root
            Destroy(tempRoot);
        }
    }
    public static void RenderCloak(CharacterAvatar avatar) {
        if (avatar.Class.BaseEquipment.ArmorSet.View.Cloak != null && avatar.Body.Torso != null) {
            string? prefabPath = avatar.Class.BaseEquipment.ArmorSet.View.Cloak;
            GameObject shawlPrefab = Resources.Load<GameObject>($"cloak/{prefabPath}");
            if (shawlPrefab == null) {
                Debug.LogWarning($"NPCAvatar: Witch shawl prefab not found at {prefabPath}");
                return;
            }

            // Spawn under torso so it’s in the same rig space
            GameObject tempRoot = Instantiate(shawlPrefab, avatar.Body.Torso);
            tempRoot.name = EquipmentNameUtil.CleanName(shawlPrefab.name) + "_root";

            Transform leftArm = avatar.Body.Torso.Find("Left Arm");
            Transform rightArm = avatar.Body.Torso.Find("Right Arm");

            Transform shawlLeft = TransformUtil.FindRecursive(tempRoot.transform, "shawl-left");
            Transform shawlRight = TransformUtil.FindRecursive(tempRoot.transform, "shawl-right");
            Transform shawlMiddle = TransformUtil.FindRecursive(tempRoot.transform, "shawl-middle");

            if (leftArm != null && shawlLeft != null) {
                shawlLeft.SetParent(leftArm, true);   // keep world pose
            }

            if (rightArm != null && shawlRight != null) {
                shawlRight.SetParent(rightArm, true);
            }

            // middle should ride the torso bone directly, not the temp root
            if (avatar.Body.Torso != null && shawlMiddle != null) {
                shawlMiddle.SetParent(avatar.Body.Torso, true);
            }

            // destroy the container now that children have been reparented
            Destroy(tempRoot);
        }
    }

    public static void RenderWeapon(CharacterAvatar avatar) {
        if(!avatar.Body.Torso) {
            return;
        }
        if (avatar.Class.BaseEquipment.Weapon != null) {
            string prefabPath = avatar.Class.BaseEquipment.Weapon.View;
            GameObject prefab = Resources.Load<GameObject>($"weapon/{prefabPath}");
            Debug.Log($"Loading weapon prefab at weapon/{prefabPath}");
            Debug.Log($"Prefab found: {prefab != null}");
            if (prefab != null) {
                Transform rightHand = avatar.Body.Torso.Find("Right Arm").Find("Right Lower Arm").Find("Right Hand");
                var inst = Instantiate(prefab, rightHand);
                inst.name = EquipmentNameUtil.CleanName(prefab.name);
            }
        }
    }
}