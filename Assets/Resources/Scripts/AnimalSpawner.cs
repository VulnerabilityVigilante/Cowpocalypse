using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public static AnimalSpawner Instance;

    [Header("Setup")]
    public List<GameObject> animalPrefabs; // Add cow and future animal prefabs here
    public Transform player;
    public Transform fenceAreaCenter;
    public float spawnRadius = 200f;
    public int initialCount = 1;

    [Header("Behavior")]
    public float checkInterval = 1f; // how often to check if player looks away
    public int maxAnimals = 50;

    List<GameObject> activeAnimals = new List<GameObject>();
    bool spawningStarted = false;
    float nextCheckTime = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!spawningStarted) return;

        // Make all animals face the player
        foreach (GameObject animal in activeAnimals)
        {
            if (animal == null) continue;

            Vector3 lookDir = player.position - animal.transform.position;
            lookDir.y = 0;
            animal.transform.rotation = Quaternion.LookRotation(lookDir);
        }

        // Occasionally check if the player is looking away
        if (spawningStarted && Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            if (IsPlayerLookingAway())
                MultiplyAnimals();
        }

    }

    public void StartSpawning()
    {
        if (spawningStarted) return;
        spawningStarted = true;
        Debug.Log("🐄 Animal spawning started!");
        SpawnInitialAnimals();
    }

    public void OnFenceFixed()
    {
        // Called when each fence post is repaired
        if (FenceManager.Instance != null)
        {
            int repaired = FenceManager.Instance.logsRequired - Inventory.Instance.fenceParts;
            if (repaired <= 0)
            {
                Debug.Log("✅ Fence fully repaired — animal chaos complete!");
                DestroyAllAnimals();
            }
        }
    }

    void SpawnInitialAnimals()
    {
        for (int i = 0; i < initialCount; i++)
        {
            SpawnAnimal();
        }
    }

    void SpawnAnimal()
    {
        if (animalPrefabs.Count == 0 || player == null) return;

        GameObject prefab = animalPrefabs[Random.Range(0, animalPrefabs.Count)];

        // --- Find a position BEHIND the player ---
        Vector3 behindDir = -player.forward; // opposite of where player looks
        Vector3 randomOffset = (Random.insideUnitSphere * spawnRadius);
        randomOffset.y = 0;

        // base position behind the player, a few meters away
        Vector3 basePos = player.position + behindDir.normalized * (spawnRadius * 1.5f) + randomOffset;

        // --- Ground alignment ---
        if (Terrain.activeTerrain != null)
            basePos.y = Terrain.activeTerrain.SampleHeight(basePos);
        else if (Physics.Raycast(basePos + Vector3.up * 10f, Vector3.down, out var hit, 50f))
            basePos = hit.point;

        // --- Create wrapper ---
        GameObject wrapper = new GameObject(prefab.name + "_Wrapper");
        wrapper.transform.position = basePos;
        wrapper.transform.rotation = Quaternion.identity;

        // --- Instantiate animal prefab ---
        GameObject newAnimal = Instantiate(prefab, wrapper.transform);
        newAnimal.transform.localPosition = Vector3.zero;
        newAnimal.transform.localRotation = Quaternion.identity;

        // --- Apply prefab-specific corrections ---
        string lowerName = prefab.name.ToLower();

        if (lowerName.Contains("cow"))
        {
            // Fix cows that import with Z-up axis
            newAnimal.transform.Rotate(Vector3.right * -90f, Space.Self);
        }

        if (lowerName.Contains("horse"))
        {
            // Lift horses slightly higher above terrain
            wrapper.transform.position += Vector3.up * 20f;
        }

        // --- Final small offset (safety) ---
        wrapper.transform.position += Vector3.up * 0.05f;

        activeAnimals.Add(wrapper);
    }








    void MultiplyAnimals()
    {
        if (activeAnimals.Count >= maxAnimals) return;

        int countToAdd = Mathf.Min(3, maxAnimals - activeAnimals.Count);
        for (int i = 0; i < countToAdd; i++)
            SpawnAnimal();

        Debug.Log("🐮 Animals multiplied! Total: " + activeAnimals.Count);
    }

    bool IsPlayerLookingAway()
    {
        if (Camera.main == null || fenceAreaCenter == null)
            return false;

        Camera cam = Camera.main;

        // Convert fence position to viewport coordinates
        // (0,0) = bottom-left, (1,1) = top-right, z > 0 = in front of camera
        Vector3 viewPos = cam.WorldToViewportPoint(fenceAreaCenter.position);

        // If object is behind camera, viewPos.z will be negative.
        bool behindCamera = viewPos.z < 0f;

        // If object is outside the screen horizontally or vertically, it's out of view.
        bool outsideView = viewPos.x < 0f || viewPos.x > 1f || viewPos.y < 0f || viewPos.y > 1f;

        // Combine both
        return behindCamera || outsideView;
    }


    public void StopSpawning()
    {
        if (!spawningStarted) return;

        Debug.Log("Stopping animal spawner and cleaning up animals.");
        spawningStarted = false;

        DestroyAllAnimals();
    }

    public void DestroyAllAnimals()
    {
        Debug.Log("Destroying all spawned animals...");

        // Destroy each active animal safely
        foreach (var animal in activeAnimals)
        {
            if (animal != null)
                Destroy(animal);
        }

        activeAnimals.Clear();
    }


}
