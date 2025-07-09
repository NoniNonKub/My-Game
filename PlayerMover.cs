using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }

    void FixedUpdate()
    {
        if (!hasTarget) return;

        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance < stopDistance)
        {
            rb.linearVelocity = Vector3.zero;
            hasTarget = false;
            return;
        }

        Vector3 moveDir = direction.normalized;
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }
}
