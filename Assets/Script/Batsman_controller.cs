using System.Collections;
using UnityEngine;

public class Batsman_controller : MonoBehaviour
{
    Animator player_animator;
    [SerializeField] BoxCollider Bat_Collider;

    private void Start()
    {
       // Bat_Collider = GetComponent<BoxCollider>();//not find boxcollider the show error 
        player_animator = GetComponent<Animator>();//animator searching...
    }
    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SwingBat());
        }
    }*/
    public void PlayForceShot()
    {
        Debug.Log("Force Shot Button Pressed");
        StartCoroutine(SwingBat(true)); // force = true
    }

    public void PlayDefenseShot()
    {
        Debug.Log("Defense Shot Button Pressed");
        StartCoroutine(SwingBat(false)); // force = false
    }
    IEnumerator SwingBat(bool isForce)
    {
        player_animator.SetBool("Shot", true);
        Bat_Collider.enabled = true;

        // Tell BallController which kind of shot
        Ball_controller.CurrentShotIsForce = isForce;

        yield return new WaitForSeconds(0.5f);

        Bat_Collider.enabled = false;
        player_animator.SetBool("Shot", false);
       
    }
  
}
