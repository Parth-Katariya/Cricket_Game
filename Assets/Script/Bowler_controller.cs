using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour// Bowler_controller
{
    public float detectionRadius = 1f;
    public float moveSpeed = 6f;
    public float minThrowSpeed = 6f;  // Increased minimum to avoid light touches
    public float returnDelay = 0f;

    private GameObject ballTarget;
    private Vector3 startPosition;
    private bool isReturning = false;
    private bool hasCaughtBall = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (hasCaughtBall) return;

        if (ballTarget == null)
        {
            FindIncomingBall();
        }
        else
        {
            MoveTowardBall();
        }

        if (isReturning)
        {
            ReturnToStart();
        }
    }

    void FindIncomingBall()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb == null) continue;

            float speed = rb.linearVelocity.magnitude;
            if (speed < minThrowSpeed) continue; // Ignore slow balls

            Vector3 toBowler = (transform.position - ball.transform.position).normalized;
            float directionCheck = Vector3.Dot(rb.linearVelocity.normalized, toBowler);

            // Only respond to balls moving TOWARD the bowler
            if (directionCheck < 0.8f) continue;

            float distance = Vector3.Distance(transform.position, ball.transform.position);
            if (distance > detectionRadius) continue;

            ballTarget = ball;
            break;
        }
    }

    void MoveTowardBall()
    {
        if (ballTarget == null) return;

        Vector3 targetPos = new Vector3(ballTarget.transform.position.x, transform.position.y, ballTarget.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    void ReturnToStart()
    {
        transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, startPosition) < 0.1f)
        {
            isReturning = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;
        if (hasCaughtBall) return;

        Rigidbody ballRb = collision.rigidbody;
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Bowler: Ball caught!");
        hasCaughtBall = true;
        collision.gameObject.GetComponent<Ball_controller>().OnDestroy_Ball(true);
        //Destroy(collision.gameObject, returnDelay);
        Invoke(nameof(BeginReturnToStart), returnDelay);
    }

    void BeginReturnToStart()
    {
        ballTarget = null;
        isReturning = true;
        hasCaughtBall = false;
    }
}
