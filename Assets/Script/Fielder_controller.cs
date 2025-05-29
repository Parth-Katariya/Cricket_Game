using UnityEngine;

public class Fielder_controller : MonoBehaviour
{
    public Transform BowlerTarget;         // Reference to bowler
    public float ThrowForce = 10f;         // Force of throw
    public float FielderSpeed = 5f;        // Speed to move toward the ball
    private CapsuleCollider capsuleCollider;
    private static GameObject targetBall;  // Shared among all fielders
    private static Fielder_controller activeFielder;

    private Rigidbody rb;
    private bool hasThrown = false;
    private Vector3 originalPosition;
    private bool returningToOrigin = false;
    private static bool ballReachedBoundary = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalPosition = transform.position; // Store origin
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (targetBall == null)
            {
                targetBall = other.gameObject;

                // Find nearest fielder
                Fielder_controller[] allFielders = FindObjectsOfType<Fielder_controller>();
                float closestDistance = Mathf.Infinity;

                foreach (var fielder in allFielders)
                {
                    float distance = Vector3.Distance(fielder.transform.position, targetBall.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        activeFielder = fielder;
                    }
                }
            }

            if (activeFielder == this && !hasThrown)
            {
                Game_Manager.instance.Current_Ball = targetBall;
                Vector3 direction = (targetBall.transform.position - transform.position).normalized;
                rb.MovePosition(transform.position + direction * FielderSpeed * Time.deltaTime);
            }
        }
    }
    public static void OnBallReachedBoundary()
    {
        ballReachedBoundary = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (activeFielder != this || hasThrown) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            Ball_controller ballController = collision.gameObject.GetComponent<Ball_controller>();

            if (ballRb != null && BowlerTarget != null)
            {
                // ✅ If ball was hit and not grounded → it's a catch
                if (ballController != null && ballController.IsHit() && ballController.GetTouchCount() == 0)
                {
                    //Game_Manager.instance.ShowCatchImage();
                    Game_Manager.instance.BatsmanOut(); // Record the wicket
                   // Destroy(this.gameObject);
                    //Invoke(nameof(ShowRestartDueToCatch), 2f);
                    return;
                }

                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;

                capsuleCollider.enabled = false;

                Vector3 direction = (BowlerTarget.position - collision.transform.position).normalized;
                ballRb.AddForce(direction * ThrowForce, ForceMode.Impulse);

                Debug.Log($"{gameObject.name}: Ball thrown to bowler!");

                hasThrown = true;
                returningToOrigin = true;
                Invoke(nameof(ResetFielder), 1f);
            }
        }

    }

    private void ShowRestartDueToCatch()
    {
        Game_Manager.instance.ShowRestartPage();
        Game_Manager.instance.Total_Run = 0;
        //Destroy(targetBall);
        // ballController.OnDestroy_Ball();
        targetBall.GetComponent<Ball_controller>().OnDestroy_Ball();
    }
    private void Update()
    {
        if (ballReachedBoundary && !returningToOrigin)
        {
            returningToOrigin = true;
            ResetFielder();
        }

        if (returningToOrigin)
        {
            float step = FielderSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, step);

            if (Vector3.Distance(transform.position, originalPosition) < 0.09f)
            {
                returningToOrigin = false;
            }
        }
    }

    private void ResetFielder()
    {
        capsuleCollider.enabled = true;
        hasThrown = false;

        if (activeFielder == this)
        {
            targetBall = null;
            activeFielder = null;
            Game_Manager.instance.Current_Ball = null;
            ballReachedBoundary = false; // Reset this only once
        }
    }
  
}