using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(NPCCombat))]
[RequireComponent(typeof(Animator))]
public class NPCController : MonoBehaviour
{
    private NPCMovement movement;
    private NPCCombat combat;
    private Animator animator;

    private CharacterForm form;

    private bool engaged = false;
    public bool Engaged => engaged;

    private string currentAnim = "";

    void Start()
    {
        movement = GetComponent<NPCMovement>();
        combat = GetComponent<NPCCombat>();
        animator = GetComponent<Animator>();
        form = GetComponent<CharacterForm>();
    }

    void Update()
    {
        ScanForTargets();
        UpdateState();
    }

    private void ScanForTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, form.ScanRange);
        float nearest = float.MaxValue;
        CharacterForm closest = null;

        foreach (var hit in hits)
        {
            CharacterForm cf = hit.GetComponent<CharacterForm>();
            if (cf == null || cf == form) continue;
            if (cf.alignment == form.alignment) continue;

            float dist = Vector3.Distance(transform.position, cf.transform.position);
            if (dist < nearest)
            {
                nearest = dist;
                closest = cf;
            }
        }

        if (closest != null)
        {
            form.Target = closest;
            engaged = true;
            movement.target = closest.transform;
        }
        else
        {
            form.Target = null;
            engaged = false;
            movement.target = null;
        }
    }

    private void UpdateState()
    {
        bool moving = movement.IsMoving;
        bool attacking = combat.IsAttacking;
        bool engaged = combat.Engaged;

        string nextAnim;

        if (attacking)
            nextAnim = "Basic Attack Animation";
        else if (moving && engaged)
            nextAnim = "Combat Walk Animation";
        else if (moving)
            nextAnim = "Basic Walk Animation";
        else if (engaged)
            nextAnim = "Combat Idle Animation";
        else
            nextAnim = "Basic Idle Animation";

        if (currentAnim != nextAnim)
        {
            currentAnim = nextAnim;
            animator.CrossFadeInFixedTime(currentAnim, 0.1f);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (form == null) form = GetComponent<CharacterForm>();
        if (form == null) return;
        Gizmos.color = new Color(0.5f, 0f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, form.ScanRange);
    }
#endif
}
