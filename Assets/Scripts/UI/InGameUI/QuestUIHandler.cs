using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt{
    public class QuestUIHandler : MonoBehaviour
    {
        protected CharacterSelectionData _selectedData;
        protected int _index;

        [SerializeField] protected Text _questTitle;
        [SerializeField] protected Text _questDescription;
        [SerializeField] protected Text _questExptext;
        [SerializeField] protected Text _questExpValueText;
         [SerializeField] protected Text _questTimetext;
        [SerializeField] protected Text _questTimeValueText;

        [SerializeField] protected Text _mainQuestDescription;

        [SerializeField] protected GameObject _timeManager;

        [SerializeField] protected Text _playerSpendableTime;

        [SerializeField] protected Text _buttonText;

        Player _player;

        private string _defaultDescription= "The aren't active quest yet. Find a \"quest room\" and press \"E\" or \"circle\"  in order to start, or just join another player quest by enter in an already active quest room.";
        private string _defaultTitle = "No active quest";

        private string _impostorMainObjective = "As the impostor your main objective is to make the timer run until it reaches the timeout or just kill all the other players but keep attention they may suspect you are the impostor! Keep an eye on the timer the clock is ticking, but the other players are loading more and more time. It's going to be tough.";
        private string _otherPlayerMainObjective = "There is an impostor among us, and there isn't much time left, the portal will be open if you don't avoid it. Complete room quests in order to load more time and find the portal room then just destroy it! Beware of the impostor, and remember killing him doesn't grant vicotry!";

        private void Awake(){
            _selectedData = CharacterSelectionData.Instance;
            _index = _selectedData.GetCharacterIndex();
            _player = GameObject.FindWithTag(_selectedData.GetCharacterSelected()).GetComponent<Player>();
        }

        private void Start(){
            _questExptext.gameObject.SetActive(false);
            _questExpValueText.gameObject.SetActive(false);
            _questTimetext.gameObject.SetActive(false);
            _questTimeValueText.gameObject.SetActive(false);
            _questDescription.text = _defaultDescription;
            _questTitle.text = _defaultTitle;
            if(_player.isImpostor()){
                _mainQuestDescription.text = _impostorMainObjective;
                _buttonText.text = "Sub Time";
            }
            else{
                 _mainQuestDescription.text = _otherPlayerMainObjective;
                 _buttonText.text = "Add Time";
            }         
        }

        public void Update(){
            _playerSpendableTime.text = _player.GetSpendableTime().ToString();
        }

        public void TimeAction(){
             if(_player.isImpostor())
                SubTime();
             else AddTime();
        }
        private void AddTime(){
            _timeManager.GetComponent<TimerManager>().addTime(_player.GetSpendableTime());
            _player.SetSpendableTime(0f);
        }
        private void SubTime(){
            _timeManager.GetComponent<TimerManager>().subTime(_player.GetSpendableTime());
            _player.SetSpendableTime(0f);
        }

        public void SetActiveQuest(Quest quest){
            _questTitle.text = quest._title;
            _questDescription.text = quest._description;
            _questExpValueText.text = quest._expReward.ToString();
            _questTimeValueText.text = quest._timeReward.ToString();
            _questExptext.gameObject.SetActive(true);
            _questExpValueText.gameObject.SetActive(true);
            _questTimetext.gameObject.SetActive(true);
            _questTimeValueText.gameObject.SetActive(true);
        }
        public void RemoveActiveQuest(){
            _questExptext.gameObject.SetActive(false);
            _questExpValueText.gameObject.SetActive(false);
            _questTimetext.gameObject.SetActive(false);
            _questTimeValueText.gameObject.SetActive(false);
            _questDescription.text = _defaultDescription;
            _questTitle.text = _defaultTitle;
        }
    }
}
