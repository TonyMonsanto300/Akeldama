using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterForm))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class NPCCombat : MonoBehaviour
{
    [Header("Combat Stats")]
    public float AttackDamage = 10f;
    public float AttackForce = 6f;
    public float AttackRate = 1f;       // Used for AI pacing or other systems
    public float AttackDistance = 2f;

    [Header("Attack Timing")]
    public float attackDuration = 1f;   // Duration of the attack animation
    public float AttackCooldown = 1f;   // NEW: cooldown lockout after each attack
    public float turnSpeed = 10f;

    private CharacterForm form;
    private Animator animator;
    private CharacterController controller;

    private bool attacking;
    private bool engaged;
    private bool attackOnCooldown;

    public bool Engaged => engaged;
    public bool IsAttacking => attacking;
    public bool IsOnCooldown => attackOnCooldown;

    private enum CombatState { Idle, CombatIdle, Attacking }
    private CombatState state = CombatState.Idle;

    void Start()
    {
        form = GetComponent<CharacterForm>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        if (form.alignment == CharacterForm.Alignment.Hostile)
            StartCoroutine(AIBehaviorLoop());
    }

    private IEnumerator AIBehaviorLoop()
    {
        while (true)
        {
            ScanForTargets();
            HandleCombatState();

            if (state == CombatState.CombatIdle && form.Target != null)
                FaceTargetSmoothly();

            yield return new WaitForSeconds(0.1f);
        }
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

        form.Target = closest;
        engaged = (closest != null);
    }


    private void HandleCombatState()
    {
        if (form.Target == null)
            return;

        float dist = Vector3.Distance(transform.position, form.Target.transform.position);

        // Don’t interrupt ongoing attack or cooldown
        if (attacking || attackOnCooldown) return;

        if (dist <= AttackDistance)
        {
            StartCoroutine(Attack());
        }
        else if (dist > form.ScanRange)
        {
            form.Target = null;
            engaged = false;
            state = CombatState.Idle;
        }
        else
        {
            // Between AttackDistance and ScanRange
            state = CombatState.CombatIdle;
        }
    }


    private IEnumerator Attack()
    {
        attacking = true;
        state = CombatState.Attacking;

        // Don't trigger animation here — controller handles visuals
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;
            if (form.Target != null)
                FaceTargetSmoothly();
            yield return null;
        }

        attacking = false;
        StartCoroutine(AttackCooldownRoutine());

        // Return to logical combat state
        state = form.Target != null ? CombatState.CombatIdle : CombatState.Idle;
    }


    private IEnumerator AttackCooldownRoutine()
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(AttackCooldown);
        attackOnCooldown = false;
    }

    private void FaceTargetSmoothly()
    {
        if (form.Target == null) return;

        Vector3 dir = (form.Target.transform.position - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir.normalized) * Quaternion.Euler(0, 180f, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }

    private bool IsAttackActiveWindow()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Basic Attack Animation")) return false;

        Debug.Log($"Checking attack window: {info.IsName("Basic Attack Animation")} at time {info.normalizedTime}");

        float t = info.normalizedTime % 1f;
        return t >= 0.35f && t <= 0.67f;
    }

    public void ApplyAttack(CharacterForm target)
    {
        if (target == null || target == form) return;
        if (!IsAttackActiveWindow()) return;

        Debug.Log($"{form.DisplayName} hit {target.DisplayName} for {AttackDamage} damage.");

        target.TakeDamage(AttackDamage);
        if (target.CurrentHP <= 0)
        {
            target.CurrentHP = 0;
            Debug.Log($"{target.DisplayName} has died.");
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        target.ApplyKnockback(dir, AttackForce);
    }

    public void PlayerAttack()
    {
        if (attacking || attackOnCooldown)
        {
            Debug.Log($"{name} cannot attack yet (attacking={attacking}, cooldown={attackOnCooldown}).");
            return;
        }

        Debug.Log($"{name} triggered PlayerAttack()");

        StartCoroutine(Attack());
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var cf = GetComponent<CharacterForm>();
        if (cf == null) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, cf.ScanRange);
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }
#endif
}
