using UnityEngine;
using System.Collections;

public class CharacterForm : MonoBehaviour
{
    public string DisplayName = "Unknown";
    public float MaxHP = 100f;
    public float CurrentHP = 100f;
    public int Level = 1;
    public enum Alignment { Hostile, Peaceful, Neutral, Special, Ally, Defending }
    public Alignment alignment = Alignment.Neutral;

    [HideInInspector] public CharacterForm Target;
    public float ScanRange = 10f;

    private Coroutine knockbackRoutine;

    public void ApplyKnockback(Vector3 direction, float force)
    {
        // Cancel previous knockback if still running
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackRoutine(direction, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc == null) yield break;

        float duration = 0.25f; // total knockback time
        float elapsed = 0f;
        Vector3 velocity = direction * force;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Linearly decay force (you can swap for exponential if you prefer)
            Vector3 frameMove = velocity * (1f - t) * Time.deltaTime;
            cc.Move(frameMove);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void TakeDamage(float amount)
    {
        CurrentHP -= amount;
        if (CurrentHP < 0f) {
            CurrentHP = 0;
        }
        //See if this character has a component called "PuppetPlates" and grab it
        PuppetPlates plates = GetComponent<PuppetPlates>();
        if (plates != null)
        {
            plates.UpdateHPPlate(CurrentHP, MaxHP);
        }
    }
}
