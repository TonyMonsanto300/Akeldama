using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterForm))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class NPCCombat : MonoBehaviour
{
    [Header("Combat Stats")]
    public float AttackDamage = 10f;
    public float AttackForce = 6f;
    public float AttackRate = 1f;
    public float AttackDistance = 2f;

    [Header("Attack Timing")]
    public float attackDuration = 1f;
    public float AttackCooldown = 1f;
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

    private bool recentlyAppliedAttack = false;
    public float ApplyAttackCooldown = 0.2f;

    private enum CombatState { Idle, CombatIdle, Attacking }
    private CombatState state = CombatState.Idle;

    private HashSet<CharacterForm> hitTargetsThisAttack = new HashSet<CharacterForm>();

    void Start()
    {
        form = GetComponent<CharacterForm>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        if (form.alignment == CharacterForm.Alignment.Hostile)
            StartCombatLoop();
    }

    public void StartCombatLoop()
    {
        StartCoroutine(AIBehaviorLoop());
    }

    private IEnumerator AIBehaviorLoop()
    {
        while (true)
        {
            HandleCombatState();

            if (state == CombatState.CombatIdle && form.Target != null)
                FaceTargetSmoothly();

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void HandleCombatState()
    {
        if (form.Target == null)
        {
            engaged = false;
            state = CombatState.Idle;
            return;
        }

        engaged = true;

        float dist = Vector3.Distance(transform.position, form.Target.transform.position);

        if (attacking || attackOnCooldown)
            return;

        if (dist <= AttackDistance)
        {
            StartCoroutine(Attack());
        }
        else
        {
            state = CombatState.CombatIdle;
        }
    }

    private IEnumerator Attack() {
        attacking = true;
        state = CombatState.Attacking;

        // NEW: reset per-swing hit tracking
        hitTargetsThisAttack.Clear();

        float elapsed = 0f;
        while (elapsed < attackDuration) {
            elapsed += Time.deltaTime;
            if (form.Target != null)
                FaceTargetSmoothly();
            yield return null;
        }

        attacking = false;
        StartCoroutine(AttackCooldownRoutine());

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

    private bool IsAttackActiveWindow() {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("Basic Attack Animation")) return false;

        float t = info.normalizedTime % 1f;
        return t >= 0.10f && t <= 0.80f;
    }

    public void ApplyAttack(CharacterForm target) {
        // Only allow hits during an active attack
        if (!attacking) return;
        if (target == null || target == form) return;

        //if (!IsAttackActiveWindow()) return;

        // Already hit this target during this swing? Ignore.
        if (hitTargetsThisAttack.Contains(target))
            return;

        // Mark as hit for this attack
        hitTargetsThisAttack.Add(target);

        // Apply damage
        target.TakeDamage(AttackDamage, form);

        if (target.CurrentHP <= 0) {
            target.CurrentHP = 0;
        }

        // Apply knockback
        Vector3 dir = (target.transform.position - transform.position).normalized;
        target.ApplyKnockback(dir, AttackForce);
    }

    private IEnumerator ApplyAttackCooldownRoutine()
    {
        recentlyAppliedAttack = true;
        yield return new WaitForSeconds(ApplyAttackCooldown);
        recentlyAppliedAttack = false;
    }

    public void PlayerAttack()
    {
        if (attacking || attackOnCooldown)
            return;

        StartCoroutine(Attack());
    }
}
