    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class Game_Manager : MonoBehaviour
    {                                       //Manages overall game state,Auto ball throws, scoring, UI updates, win/lose conditions, and restart logic.
        public static Game_Manager instance;
        public GameObject Ball_Prefeb;
        public GameObject Current_Ball;

        public Vector3 ball_position;
        public Text Run_Text;
        public int Total_Run = 0;
        public GameObject Restart_Page;
        public Text RestartScoreText;
        public GameObject Home_Page;
        public GameObject SixImage;
        public GameObject FourImage;
        public GameObject OutImage;

        public int Total_Sixes = 0;
        public int Total_Fours = 0;
        public Text Six_Text;
        public Text Four_Text;
        public Text RestartSixText;
        public Text RestartFourText;

        private int TargetScore;
        private int RemainingBalls = 0;
        public Text TargetText;//
        public Text RemainingBallsText;
        public GameObject WinPage;

        public GameObject CatchImage;
        private int TotalWickets = 0;
        public Text WicketText;//
        public Text WicketsFallenText;
        private int WicketsFallen = 0;

        public GameObject Score_Card;
        private bool hasBatsmanBeenOutThisBall = false;
        public Text BallTimerText;
    private void Awake()
        {
            instance = this;

        }
        void Start()
        {
            Time.timeScale = 0f;

            // Ensure target and balls are set even if starting from scene directly
            if (TargetScore <= 0 || RemainingBalls <= 0)
            {
                RemainingBalls = Random.Range(6, 19); // Random between 6 and 18 balls
                TargetScore = Random.Range(RemainingBalls * 1, RemainingBalls * 3); // target between 1x and 3x runs per ball
            }

            TotalWickets = CalculateWickets(TargetScore, RemainingBalls);
            WicketsFallen = 0; // Reset wickets fallen

            Update_Run(0);
            UpdateExtrasUI();
            UpdateTargetUI();
            UpdateWicketUI();
            UpdateWicketsFallenUI(); // Update the UI
            //AutoThrowBall();
        }

        /* void Update()
         {
             if (Input.GetKeyDown(KeyCode.P))
             {
                 Throw_Ball();
             }
         }*/
        public void UpdateExtrasUI()
        {
            if (Six_Text != null)
                Six_Text.text = "6 : " + Total_Sixes;

            if (Four_Text != null)
                Four_Text.text = "4 : " + Total_Fours;
        }
        public void UpdateTargetUI()
        {
            if (TargetText != null)
                TargetText.text = "Target: " + TargetScore;

            if (RemainingBallsText != null)
                RemainingBallsText.text = "Remaining_Balls " + RemainingBalls;
        }
    public void UpdateWicketUI()
    {
        if (WicketText != null)
            WicketText.text = "Remaining Wickets: " + TotalWickets;
        //Debug.Log("TotalWickets: " + TotalWickets + ", WicketsFallen: " + WicketsFallen);
    }
    /* public void AutoThrowBall()
     {
         Invoke("_AutoThrowBall", 5);
     }*/
    public void AutoThrowBall(int addsecond =0)
    {

        StartCoroutine(StartBallCountdown(5+addsecond));
    }

    private IEnumerator StartBallCountdown(int seconds)
    {
        if (BallTimerText != null)
            BallTimerText.gameObject.SetActive(true);

        int remaining = seconds;
        while (remaining > 0)
        {
            if (BallTimerText != null && remaining<=5)
                //BallTimerText.text = "Next Ball in: " + remaining;
                BallTimerText.text = remaining.ToString();

            yield return new WaitForSeconds(1f);
            remaining--;
        }

        if (BallTimerText != null)
        {
            BallTimerText.text = "";
            BallTimerText.gameObject.SetActive(false);
        }

        _AutoThrowBall();
    }
    private void _AutoThrowBall()
        {
            if (Current_Ball == null && RemainingBalls > 0)
            {
                Throw_Ball();
                //StartCoroutine(AutoThrowBall());
            }
        }

        public void Throw_Ball()
        {
            if (Restart_Page.activeSelf || WinPage.activeSelf || Home_Page.activeSelf)
            {
                Debug.Log("Game is not active. Ball cannot be thrown.");
                return;
            }

            if (Current_Ball != null)
            {
                Debug.Log("Wait: Ball is still active!");
                return;
            }

            if (RemainingBalls > 0)
            {
                hasBatsmanBeenOutThisBall = false; // Reset flag for new ball

                Current_Ball = Instantiate(Ball_Prefeb, ball_position, Quaternion.identity);
                RemainingBalls--;
                UpdateTargetUI();

                if (RemainingBalls == 0 && TargetScore > 0)
                {
                    CheckGameResult(); // check win/loss if no balls left
                }
            }
        }

        public void Update_Run(int Score)
        {
            /* Total_Run = Total_Run + Score;
             Run_Text.text = "Runs - " + Total_Run;*/
            Total_Run += Score;
            //TargetScore -= Score;
            TargetScore = Mathf.Max(0, TargetScore - Score);

            Run_Text.text = "Runs - " + Total_Run;
            UpdateTargetUI();

            if (TargetScore <= 0)
            {
                ShowWinPage();
            }

        }
        public void ShowWinPage()
        {
            StartCoroutine(ShowWinPageAfterDelay());
        }
        private IEnumerator ShowWinPageAfterDelay()
        {
            // Wait for 2 seconds to allow SixImage and FourImage to be visible
            yield return new WaitForSeconds(2f);

            // Hide SixImage and FourImage if they are active
            if (SixImage != null && SixImage.activeSelf)
                SixImage.SetActive(false);

            if (FourImage != null && FourImage.activeSelf)
                FourImage.SetActive(false);

            // Show the WinPage
            if (WinPage != null)
                WinPage.SetActive(true);

            // Pause the game
            Time.timeScale = 0f;
        }

        public void CheckGameResult()
        {
            if (TargetScore > 0)
            {
                Restart_Page.SetActive(true);
                Time.timeScale = 0f;
            }
        }
        public void ShowRestartPage()
        {
            Score_Card.SetActive(false);
            Restart_Page.SetActive(true);
            RestartScoreText.text = "Runs : " + Total_Run; // ✅ Now shows actual score

            if (RestartSixText != null)
                RestartSixText.text = "6s : " + Total_Sixes;

            if (RestartFourText != null)
                RestartFourText.text = "4s : " + Total_Fours;
        }
        public void RestartGame()
        {
            SceneManager.LoadScene("Cricket");
            Time.timeScale = 1f;
        }
        public void GoToHome()
        {
            Restart_Page.SetActive(false);
            WinPage.SetActive(false);
            Home_Page.SetActive(true);
        }
        public void StartGameFromHome()
        {

            Home_Page.SetActive(false);
            Score_Card.SetActive(true);

            Total_Run = 0;
            Total_Sixes = 0;
            Total_Fours = 0;

            // Randomize RemainingBalls between 6 and 18 (inclusive)
            RemainingBalls = Random.Range(6, 19); // 6 to 18 balls

            // Randomize TargetScore between RemainingBalls * 1 and RemainingBalls * 3 (inclusive)
            TargetScore = Random.Range(RemainingBalls * 1, RemainingBalls * 3);

            // Calculate TotalWickets based on the new TargetScore and RemainingBalls
            TotalWickets = CalculateWickets(TargetScore, RemainingBalls);
            WicketsFallen = 0;

            Time.timeScale = 1f;

            Update_Run(0);            // Resets runs and updates UI
            UpdateExtrasUI();         // Resets 6s and 4s
            UpdateTargetUI();         // Shows target and balls left
            UpdateWicketUI();         // Updates remaining wickets
            UpdateWicketsFallenUI();  // Updates wickets fallen UI
                                      //Invoke("AutoThrowBall", 5f);
                                      //AutoThrowBall();
            //Invoke("_AutoThrowBall", 5);
            AutoThrowBall();
        }
        public void ShowSixImage()
        {
            if (SixImage != null)//Show SIX image for 2 seconds
            {
                SixImage.SetActive(true);
                Invoke("HideSixImage", 2f);
            }
            Total_Sixes++;        // ✅ Increment six counter
            UpdateExtrasUI();
        }
        private void HideSixImage()
        {
            if (SixImage != null)
            {
                SixImage.SetActive(false);
            //AutoThrowBall();
            }
        }
        public void ShowFourImage()
        {
            if (FourImage != null)//Show FOUR image for 2 seconds
            {
                FourImage.SetActive(true);
                Invoke("HideFourImage", 2f);
            }
            Total_Fours++;        // ✅ Increment six counter
            UpdateExtrasUI();
        }

        private void HideFourImage()
        {
            if (FourImage != null)
            {
                FourImage.SetActive(false);
            //AutoThrowBall();
        }
        }

        public void ShowOutImage()
        {
            if (OutImage != null)
            {
                OutImage.SetActive(true);
                Invoke("HideOutImage", 2f);
            }

        }
        private void HideOutImage()
        {
            if (OutImage != null)
            {
                OutImage.SetActive(false);
            //AutoThrowBall();
        }
        }
        public void ShowCatchImage()
        {
            if (CatchImage != null)
            {
                CatchImage.SetActive(true);
                Invoke("HideCatchImage", 2f);
            }
        }
        private void HideCatchImage()
        {
            if (CatchImage != null)
            {
                CatchImage.SetActive(false);
           // AutoThrowBall();
        }
        }
        public void BatsmanOut()
        {
            if (hasBatsmanBeenOutThisBall)
                return; // prevent multiple outs per ball

            hasBatsmanBeenOutThisBall = true;


            print("Batsman is out --->");
            if (Current_Ball != null)// not a ball ,Current ball is destroy for out image is show
            {
                Current_Ball.GetComponent<Ball_controller>().OnDestroy_Ball();
               //Destroy(Current_Ball);
                Current_Ball = null;
            }

            TotalWickets--;//TargetScore = Mathf.Max(0, TargetScore - Score); 
            //TotalWickets = Mathf.Max(0, TotalWickets - 1);
            WicketsFallen++;
            //TotalWickets = Mathf.Max(0, TotalWickets - 1);

            UpdateWicketUI();
            UpdateWicketsFallenUI();
            ShowOutImage();

            if (TargetScore <= 0)
            {
                ShowWinPage();
                return;
            }

            if (TotalWickets <= 0)
            {
                Invoke(nameof(ShowRestartPage), 2f);
            return;
            }
        AutoThrowBall(2);
        }
        public void UpdateWicketsFallenUI()
        {
            if (WicketsFallenText != null)
                WicketsFallenText.text = WicketsFallen.ToString();

        }
        private int CalculateWickets(int targetScore, int remainingBalls)
        {
        if (targetScore >= 50)
            return 3;
        else if (targetScore >= 30)
            return 2;
        else
            return 1;
        }

    } 

