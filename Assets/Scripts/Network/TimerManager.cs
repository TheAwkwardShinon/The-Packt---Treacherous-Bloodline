using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace ThePackt
{
    public class TimerManager : Bolt.EntityBehaviour<ITimerManagerState>
    {
        #region variables
        float startTime; //start time of the timer
        string timeString;
        public float timerDuration; //duration of warm-up + game in seconds
        public float gameDuration; //duration of game in seconds
        private bool gameStarting;
        public bool gameStarted { get; private set; }
        public bool gameEnded { get; private set; }
        private Text timerText;
        #endregion

        #region methods
        public override void Attached()
        {
            state.AddCallback("StartTime", StartTimeCallback);

            timerText = GameObject.Find("TimerText").GetComponent<Text>();
            gameStarting = false;
            gameStarted = false;
            gameEnded = false;
        }

        private void Update()
        {
            if (!gameEnded)
            {
                float elapsedTime = BoltNetwork.ServerTime - startTime;
                float time = 0;
                if (gameStarted)
                {
                    //if the game has already started, the timer begins from the gameDuration and goes gradually to zero 
                    time = timerDuration - elapsedTime;
                    timeString = ConvertToString(time);
                    timerText.text = timeString;
                }

                if (!gameStarted)
                {
                    //if the game is not started yet, the timer begins from the difference between the gameDuration
                    //and the timerDuration and goes gradually to zero
                    time = timerDuration - gameDuration - elapsedTime;

                    if (time >= 0) {
                        timeString = ConvertToString(time);
                    }
                    else
                    {
                        timeString = ConvertToString(gameDuration);
                        time = 1;
                    }
                       
                    timerText.text = timeString;
                }

                //when the game is not started nor is starting and the timer goes to zero it means that the warm-up time is over
                //so the game pass to the starting state
                if (!gameStarted && !gameStarting && timeString == "00:00")
                {
                    Debug.Log("[TIMER] game starting");
                    gameStarting = true;
                }

                //when the game is starting and the timer is no more zero it means that the game timer is actually ready
                //to start, so the game state pass to the started state
                if (gameStarting && timeString != "00:00")
                {
                    Debug.Log("[TIMER] game started");
                    gameStarted = true;
                    gameStarting = false;

                    if (BoltNetwork.IsServer)
                    {
                        MainQuest.Instance.SetQuestState(Constants.STARTED);
                        MapGenerator.Instance.DestroyMagicObstacles();
                    }
                }

                //when the game has already started and the timer is zero or less it means that the game is over
                if (gameStarted && time <= 0)
                {
                    Debug.Log("[TIMER] game ended");
                    timerText.text = "00:00";
                    gameEnded = true;

                    if (BoltNetwork.IsServer)
                    {
                        Debug.Log("[MAIN] no more time. victory for impostor");

                        MainQuest.Instance.SetQuestState(Constants.FAILED);
                    }
                }
            }
        }

        private string ConvertToString(float time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time - minutes * 60);

            string minutesString = minutes.ToString();
            if(minutesString.Length == 1)
            {
                minutesString = "0" + minutesString;
            }

            string secondsString = seconds.ToString();
            if (secondsString.Length == 1)
            {
                secondsString = "0" + secondsString;
            }

            //Debug.Log("[TIMER] stringa: " + minutesString + ":" + secondsString);

            return minutesString + ":" + secondsString;
        }

        //adds time in seconds (modifies the start time). Only the server can do it
        public void addTime(float time)
        {
            if (BoltNetwork.IsServer)
            {
                state.StartTime += time;
            }
        }

        //subtracts time in seconds (modifies the start time). Only the server can do it
        public void subTime(float time)
        {
            Debug.Log("caia 3" + BoltNetwork.IsServer);
            if (BoltNetwork.IsServer)
            {
                Debug.Log("caia 4" + 0);
                state.StartTime -= time;
            }
        }

        #region callbacks
        private void StartTimeCallback()
        {
            startTime = state.StartTime;

            Debug.Log("[TIMER] callback: " + startTime);

            //insert timer modification notifications here as this is called on clients too
        }
        #endregion

        #endregion

        #region setter

        //sets start time in seconds. Only the server can do it
        public void SetStartTime(float time)
        {
            if (BoltNetwork.IsServer)
            {
                state.StartTime = time;
            }
        }
        #endregion

    }
}
