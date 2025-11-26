using UnityEngine;

public class Quest3_CowAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotateSpeed = 4f;
    public float detectionRange = 5000f;

    [Header("Model Orientation")]
    public Transform cowModel;
    private Quaternion modelOffset = Quaternion.Euler(-137f, -17f, 15f);

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0; // stay flat on ground

        if (toPlayer.magnitude > detectionRange)
            return;

        // ROTATE TOWARD PLAYER
        Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        // MODEL VISUAL OFFSET
        cowModel.localRotation = modelOffset;

        // MOVE FORWARD (pure transform movement)
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}
