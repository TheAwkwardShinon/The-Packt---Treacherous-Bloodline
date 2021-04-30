using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace ThePackt
{
    public class TimerManager : Bolt.EntityBehaviour<INetworkManagerState>
    {
        float startTime;
        string timeString;
        public float timerDuration; //in seconds
        public float gameDuration; //in seconds
        private string gameDurationString;
        private bool gameStarting;
        private bool gameStarted;
        private bool gameEnded;
        [SerializeField] private Text timerText;

        public override void Attached()
        {
            state.AddCallback("StartTime", StartTimeCallback);

            gameDurationString = ConvertToString(gameDuration);
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
                    time = timerDuration - elapsedTime;
                    timeString = ConvertToString(time);
                    timerText.text = timeString;
                }

                if (!gameStarted)
                {
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

                if (!gameStarted && !gameStarting && timeString == "00:00")
                {
                    Debug.Log("[TIMER] game starting");
                    gameStarting = true;
                }

                if (gameStarting && timeString != "00:00")
                {
                    Debug.Log("[TIMER] game started");
                    gameStarted = true;
                    gameStarting = false;
                }

                if (gameStarted && time <= 0)
                {
                    Debug.Log("[TIMER] game ended");
                    timerText.text = "00:00";
                    gameEnded = true;
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

        private void StartTimeCallback()
        {
            startTime = state.StartTime;

            Debug.Log("[TIMER] callback: " + startTime);

            //insert timer modification notifications here
        }

        //adds time in seconds
        public void addTime(float time)
        {
            if (BoltNetwork.IsServer)
            {
                state.StartTime += time;
            }
        }

        //subtracts time in seconds
        public void subTime(float time)
        {
            Debug.Log("caia 3" + BoltNetwork.IsServer);
            if (BoltNetwork.IsServer)
            {
                Debug.Log("caia 4" + 0);
                state.StartTime -= time;
            }
        }

        public void SetStartTime(float time)
        {
            if (BoltNetwork.IsServer)
            {
                state.StartTime = time;
            }
        }

    }
}
