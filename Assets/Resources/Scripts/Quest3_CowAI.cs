using UnityEngine;

public class Quest3_CowAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotateSpeed = 4f;
    public float detectionRange = 50f;

    [Header("Model Orientation")]
    public Transform cowModel;  
    private Quaternion modelOffset = Quaternion.Euler(-137f, -17f, 15f);

    private Transform player;
    private CharacterController controller;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponent<CharacterController>();

        if (controller == null)
            Debug.LogError("No CharacterController on ROOT. Add it to Enemy Cow (ROOT).");
    }

    void Update()
    {
        if (player == null || controller == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;

        if (toPlayer.magnitude > detectionRange)
            return;

        // Rotate ROOT toward player
        Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        // Apply visual model orientation
        cowModel.localRotation = modelOffset;

        // Move forward
        Vector3 move = transform.forward * moveSpeed * Time.deltaTime;
        controller.Move(move);
    }
}
