using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum ClassType { Warrior, Magician, Scout, Monk, Page, Cleric, Rogue, Nomad, Medium, Tamer, Druid, Performer }

public class StatBlock {
    int strength;
    int mind;
    int agility;
    int spirit;
    int charisma;
}

public class CharacterClass {
    string name;
    ClassType classType;

    int baseHP;
    int baseEP;
    int baseMP;

    StatBlock baseStats;
}

public class CharacterForm : MonoBehaviour
{

    //Vitals
    public string DisplayName = "Unknown";
    public float MaxHP = 100f;
    public float CurrentHP = 100f;
    public float MaxEnergy = 100f;
    public float CurrentEnergy = 100f;
    public int Level = 1;
    public int MoveSpeed = 1;

    // Alignment Types
    public enum Alignment { Hostile, Peaceful, Neutral, Special, Ally, Defending }
    public Alignment alignment = Alignment.Neutral;

    // Damage Text Objects
    public GameObject DamageTextPrefab;
    public float floatUpDistance = 1f;
    public float floatDuration = 1f;
    public float cameraOffsetDistance = 2f;

    // Targetting
    [HideInInspector] public CharacterForm Target;
    public float ScanRange = 10f;

    public Material flashRedMaterial;

    private bool isFlashingRed = false;
    private Coroutine knockbackRoutine;

    private bool isPlayer = false;

    public void SetIsPlayer(bool value)
    {
        isPlayer = value;
    }

    IEnumerator DeathSequence(float amount)
    {
        Transform model = transform.Find("Model");
        Transform topPlayer = transform.Find("TopPlate");
        if (model != null) {
            Destroy(model.gameObject);
        }
        if (topPlayer != null) {
            Destroy(topPlayer.gameObject);
        }
        ShowDamageText(amount);

        yield return new WaitForSeconds(floatDuration);

        yield return new WaitForSeconds(0.2f);

        Die();
    }

    public void TakeDamage(float amount, CharacterForm attacker)
    {
        CurrentHP -= amount;
        if (CurrentHP < 0f) CurrentHP = 0f;

        var plates = GetComponent<PuppetPlates>();
        if (plates != null)
            plates.UpdateHPPlate(CurrentHP, MaxHP);

        FlashRed();

        if (CurrentHP <= 0f)
        {
            if (isPlayer)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } else {
                StartCoroutine(DeathSequence(amount));
            }
        } else {
            ShowDamageText(amount);
        }
        
        if (!isPlayer && attacker.isPlayer && alignment == Alignment.Neutral) {
            alignment = Alignment.Hostile;
            if (plates != null) {
                plates.UpdateAlignmentText(false);
            }
            //check for script called NPCCombat, if i exists, call startCombatLoop()
            var npcCombat = GetComponent<NPCCombat>();
            if (npcCombat != null) {
                npcCombat.StartCombatLoop();
            }

        }
    }

    bool IsInTopPlate(Transform t)
    {
        while (t != null)
        {
            if (t.name == "TopPlate")
                return true;
            t = t.parent;
        }
        return false;
    }

    public void FlashRed(float duration = 0.05f)
    {
        if (flashRedMaterial == null) return;
        if (isFlashingRed) return;
        StartCoroutine(FlashRedRoutine(duration));
    }

    private IEnumerator FlashRedRoutine(float duration)
    {
        isFlashingRed = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        var originals = new Dictionary<Renderer, Material[]>();

        foreach (var rend in renderers)
        {
            if (IsInTopPlate(rend.transform))
                continue;

            originals[rend] = rend.materials;

            Material[] red = new Material[rend.materials.Length];
            for (int i = 0; i < red.Length; i++)
                red[i] = flashRedMaterial;

            rend.materials = red;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        foreach (var r in originals)
            if (r.Key != null)
                r.Key.materials = r.Value;

        isFlashingRed = false;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackRoutine(direction, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        var cc = GetComponent<CharacterController>();
        if (cc == null) yield break;

        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 velocity = direction * force;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cc.Move(velocity * (1f - t) * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void ShowDamageText(float amount)
    {
        if (DamageTextPrefab == null || Camera.main == null)
            return;

        Vector3 spawnPos = Camera.main.transform.position +
                           Camera.main.transform.forward * cameraOffsetDistance;

        spawnPos += Vector3.up * 1f;

        GameObject obj = Instantiate(DamageTextPrefab, spawnPos, Quaternion.identity);

        TextMesh tm = obj.GetComponent<TextMesh>();
        if (tm != null)
        {
            tm.text = amount.ToString();
            tm.color = isPlayer ? Color.blue : Color.red;
            tm.fontSize = 32;
        }

        Vector3 toCam = (Camera.main.transform.position - spawnPos).normalized;
        obj.transform.rotation = Quaternion.LookRotation(toCam) * Quaternion.Euler(0, 180f, 0);

        StartCoroutine(DamageFloatRoutine(obj));
    }

    IEnumerator DamageFloatRoutine(GameObject obj)
    {
        Vector3 start = obj.transform.position;
        Vector3 end = start + new Vector3(0, floatUpDistance, 0);

        float t = 0f;

        while (t < floatDuration)
        {
            float lerp = t / floatDuration;
            obj.transform.position = Vector3.Lerp(start, end, lerp);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
