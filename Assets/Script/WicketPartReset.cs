using UnityEngine;

public class WicketPartReset : MonoBehaviour// wicket stump move[force] to ball touch
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        ResetNow();
    }

    public void ResetPositionAfterDelay(float delay)
    {
        Invoke(nameof(ResetNow), delay);
    }

    private void ResetNow()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Temporarily disable physics
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            rb.isKinematic = false;
            Invoke(nameof(ResetNow), 2f);
        }
    }

}
