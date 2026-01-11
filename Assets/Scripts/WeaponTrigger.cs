// WeaponTrigger.cs (on the Weapon object)
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    private Collider weaponCollider;
    private NPCCombat ownerCombat;
    private CharacterForm ownerForm;

    void Start()
    {
        weaponCollider = GetComponentInChildren<Collider>();
        if (weaponCollider == null)
        {
            Debug.LogError($"{name}: No collider found under weapon.");
            return;
        }
        weaponCollider.isTrigger = true;

        if (weaponCollider is MeshCollider mc && !mc.convex)
            mc.convex = true; // mesh triggers must be convex

        ownerCombat = GetComponentInParent<NPCCombat>();
        ownerForm = GetComponentInParent<CharacterForm>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (ownerCombat == null) return;

        var target = other.GetComponent<CharacterForm>();
        if (target == null || target == ownerForm) return;

        // Pass the ACTUAL victim to the combat script.
        ownerCombat.ApplyAttack(target);
    }
}
