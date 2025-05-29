using System.Reflection;
using UnityEngine;

public class Ball_controller : MonoBehaviour
{                                   //Handles ball physics, bat hits, boundary scoring, wicket detection, and destruction.
    Rigidbody rb;
    public float Ball_Speed = 5f;
    [SerializeField]
    private int BallTouchCount = 0;
    bool isHit = false;

    public static bool CurrentShotIsForce = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        //Time.timeScale = ;
        //   this.enabled = false;
        rb.AddForce(Vector3.right * Ball_Speed, ForceMode.Force);
        //Invoke("OnDestroy_Ball",5f);// destroy every ball 5 second
        // OnDestroy_Ball(true);
        Invoke("DestroyBallAfterDelay", 5f);

    }
    private void DestroyBallAfterDelay()
    {
        OnDestroy_Ball(true);
    }

    public void OnDestroy_Ball(bool isthrowball=false)
    {

        //CancelInvoke();CancelInvoke() = cancels all invoked methods in my script------- CancelInvoke(“methodName”) = cancels all invokes of the method with this name “methodName”
        Destroy(this.gameObject);
        if(isthrowball )
        {
            Game_Manager.instance.AutoThrowBall(2);
        }
       
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bat"))
        {
            Debug.Log("Ball hit by bat");
            isHit = true;

            float ballForce, ballHeight;

            if (CurrentShotIsForce)
            {
                // Force shot - aggressive
                ballForce = Random.Range(8f, 10f);
                ballHeight = Random.Range(5f, 6f);
            }
            else
            {
                // Defensive shot - low power
                ballForce = Random.Range(3f, 4f);
                ballHeight = Random.Range(1f, 3f);
            }

            float ballPosition = Random.Range(-5f, 5f);
            Vector3 ballDirection = new Vector3(-ballForce, ballHeight, ballPosition);

            rb.linearVelocity = Vector3.zero; // Reset velocity before applying force
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(ballDirection, ForceMode.Impulse);
        }

        if (other.gameObject.CompareTag("Boundary"))
        {
            Fielder_controller.OnBallReachedBoundary();
            print("Ball Touch  Boundary  ---> " + BallTouchCount);
            if (BallTouchCount < 1)
            {
                Game_Manager.instance.Update_Run(6);
                Game_Manager.instance.ShowSixImage();
                // Destroy(this.gameObject);// ball destroy
                OnDestroy_Ball(true);
                //Debug.Log("SIX...");
            }
            else
            {
                Game_Manager.instance.Update_Run(4);
                Game_Manager.instance.ShowFourImage();
                //Destroy(this.gameObject);// ball destroy
                OnDestroy_Ball(true);
                //Debug.Log("FOUR....");
            }

        }

    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isHit)
        {
            BallTouchCount++;
            Debug.Log("Ball Touch Count: " + BallTouchCount);

        }

        if (collision.gameObject.CompareTag("Out"))
        {
            Game_Manager.instance.BatsmanOut();
            var ss = collision.transform.parent;
            //for (int i = 0; i < ss.childCount; i++)
            //{
            //    var rr = ss.GetChild(i).gameObject.GetComponent<Rigidbody>();
            //    //rr.isKinematic = false;
            //    rr.freezeRotation = false;
            //    //rr.AddForce(Vector3.back);
            //    //print(ss.GetChild(i).gameObject.name);

            //}
            //OnDestroy_Ball();
            //Destroy(this.gameObject); // Destroy the ball after the out
            Debug.Log("Wicket");// get child wicket and then a maninuly force add rigibody
        }

    }
    private void ShowRestart()
    {
        Game_Manager.instance.ShowRestartPage(); // Show page with current Total_Run
        Game_Manager.instance.Total_Run = 0;     // Reset score AFTER showing it
        //Game_Manager.instance.Update_Run(0);     // Update UI for next game
        //Destroy(this.gameObject);                // Destroy ball
       // OnDestroy_Ball();
        Debug.Log("Wicket - Restart page shown");
    }
    public bool IsHit()
    {
        return isHit;
    }

    public int GetTouchCount()
    {
        return BallTouchCount;
    }

}
