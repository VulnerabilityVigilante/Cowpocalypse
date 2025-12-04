using UnityEngine;
using System.Collections.Generic;

public class Quest3_CowWaveSpawner : MonoBehaviour
{
    public static Quest3_CowWaveSpawner Instance;

    [Header("Quest Gun")]
    public GameObject gun;   

    [Header("UI Elements")]
    public GameObject crosshair;


    [Header("Spawn Settings")]
    public GameObject cowPrefab;
    public int startingCowsPerWave = 5;
    public float minSpawnDistance = 40f;
    public float spawnRadius = 25f;
    public float baseWaveDelay = 5f;    // initial cooldown between waves
    public float minWaveDelay = 2f;     // fastest cooldown possible
    public float waveDelayDecrement = 0.5f; // how much cooldown decreases each wave

    private bool questActive = false;
    private Transform player;

    private int currentWaveCows;
    private float currentWaveDelay;

    private List<GameObject> activeCows = new List<GameObject>();  // Track cows

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Instance = this;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentWaveCows = startingCowsPerWave;
        currentWaveDelay = baseWaveDelay;

        // AUTO-RESUME CHECK
        if (QuestManager.Instance != null &&
            QuestManager.Instance.IsQuestActive("ShootCowsQuest"))
        {
            Debug.Log("[Quest3] Scene loaded while quest already active. Auto-starting wave spawner.");

            questActive = true;

            // RESTORE CONSUMABLES FOR QUEST SESSION
            // If backup doesn't exist yet, rebuild it
            if (Inventory.Instance.HasNoQuestBackup())
            {
                Inventory.Instance.RebuildHealingBackupFromCurrentValues();
            }

            Inventory.Instance.LoadHealingItemsForQuest();


            // Enable gun and crosshair
            if (gun != null) gun.SetActive(true);
            if (crosshair != null) crosshair.SetActive(true);

            // Immediately start wave spawning (dialogue already finished earlier)
            Invoke(nameof(SpawnWave), currentWaveDelay);
        }

    }


    void OnEnable()
    {
        QuestEvents.OnQuestStarted += OnQuestStarted;
        QuestEvents.OnQuestCompleted += OnQuestCompleted;
    }

    void OnDisable()
    {
        QuestEvents.OnQuestStarted -= OnQuestStarted;
        QuestEvents.OnQuestCompleted -= OnQuestCompleted;
    }

    void OnQuestStarted(string id)
    {
        if (id == "ShootCowsQuest")
        {
            questActive = true;
            Debug.Log("[Quest3] Quest start detected — waiting for dialogue to close.");
        }
    }

    public void OnDialogueClosed()
    {
        if (!questActive) return;

        // ENABLE GUN
        if (gun != null)
            gun.SetActive(true);

        // ENABLE CROSSHAIR
        if (crosshair != null)
            crosshair.SetActive(true);

        Debug.Log("[Quest3] Dialogue closed — starting cow waves!");
        Invoke(nameof(SpawnWave), currentWaveDelay);
    }



    void SpawnWave()
    {
        if (!questActive) return;

        Debug.Log($"[Quest3] Spawning wave with {currentWaveCows} cows!");

        for (int i = 0; i < currentWaveCows; i++)
        {
            SpawnCow();
        }

        // Increase wave size
        currentWaveCows++;

        // Decrease wave delay
        currentWaveDelay = Mathf.Max(currentWaveDelay - waveDelayDecrement, minWaveDelay);

        // Schedule next wave
        Invoke(nameof(SpawnWave), currentWaveDelay);
    }

    void SpawnCow()
{
    // random direction
    float angle = Random.Range(0f, Mathf.PI * 2f);

    // random distance between min and max
    float distance = Random.Range(minSpawnDistance, spawnRadius);

    Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
    Vector3 spawnPos = player.position + offset;

    // ground alignment
    if (Physics.Raycast(spawnPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
        spawnPos = hit.point;

    spawnPos.y += 13f; // cow vertical offset

    GameObject cow = Instantiate(cowPrefab, spawnPos, Quaternion.identity);

    activeCows.Add(cow);

    Debug.Log($"[Quest3] Spawned cow at distance {distance} at {spawnPos}");
}


    // Called automatically when the quest is completed
    void OnQuestCompleted(string id)
    {
        if (id != "ShootCowsQuest") return;

        Debug.Log("[Quest3] Quest completed — despawning all cows!");

        questActive = false;

        // Kill all remaining cows instantly
        foreach (GameObject cow in activeCows)
        {
            if (cow != null)
            {
                // Trigger explosion manually
                Quest3_CowHealth health = cow.GetComponentInChildren<Quest3_CowHealth>();
                if (health != null)
                    health.TriggerDeathExplosionOnly();

                Destroy(cow);
            }
        }


        activeCows.Clear();

        // Cancel future waves
        CancelInvoke(nameof(SpawnWave));
    }

    public void StopAllCowSpawning()
    {
        questActive = false;

        // Stop future waves
        CancelInvoke(nameof(SpawnWave));

        // Destroy all spawned cows
        foreach (GameObject cow in activeCows)
        {
            if (cow != null)
                Destroy(cow);
        }

        activeCows.Clear();

        Debug.Log("[Quest3] Cow spawning stopped due to player death.");
    }

}
