using UnityEngine;

public class Quest3_CowCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackInterval = 1.5f;      // how often cows attempt to shoot
    public float attackRange = 40f;          // cows must be within this distance

    [Header("Hit Chance Settings (Standing / Walking / Sprinting)")]
    public float standingHitChance = 0.75f;  
    public float walkingHitChance = 0.45f;   
    public float sprintingHitChance = 0.12f; 

    [Header("Damage (Standing / Walking / Sprinting)")]
    public int standingDamage = 18;
    public int walkingDamage = 12;
    public int sprintingDamage = 6;

    private Transform player;
    private CharacterController playerController;
    private float attackTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (player == null) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            TryShootPlayer();
        }
    }

    void TryShootPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange)
            return;

        // Player movement speed from CharacterController
        float speed = playerController.velocity.magnitude;

        float hitChance = 0f;
        int dmg = 0;

        // -------------------------
        //  TUNED THRESHOLDS
        //  Based on actual PlayerController.cs values:
        //  walk = 100, sprint = 200
        // -------------------------

        // STANDING STILL
        if (speed < 1f)
        {
            hitChance = standingHitChance;   // 75%
            dmg = standingDamage;            // 18 dmg
        }
        // WALKING / MOVING NORMALLY
        else if (speed < 120f) // anything below sprint threshold
        {
            hitChance = walkingHitChance;    // 45%
            dmg = walkingDamage;             // 12 dmg
        }
        // FULL SPRINT
        else
        {
            hitChance = sprintingHitChance;  // 12%
            dmg = sprintingDamage;           // 6 dmg
        }

        // RNG CHECK
        if (Random.value <= hitChance)
        {
            PlayerHit(dmg);
        }
    }

    void PlayerHit(int dmg)
    {
        Debug.Log($"PLAYER SHOT! Damage: {dmg}");

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(dmg);
    }

}
