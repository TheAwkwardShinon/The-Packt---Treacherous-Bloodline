using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt{
    public class HiddenCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject firstPanel;

        [SerializeField] private tabButton _firstTab;

        [SerializeField] private TabGroup _tabGroup;

        [SerializeField]private Text _questTitleText;
        [SerializeField]private Text _questDescriptionText;
        [SerializeField]private Text _questReward;
        [SerializeField]private Text _questAction;
        [SerializeField]private GameObject _questPanel;

        [SerializeField] private QuestUIHandler _questHandler;

        [SerializeField] private GameObject _objectiveMessage;

        [SerializeField] private GameObject _tooltip;

        public GameObject GetMenu(){
            return menu;
        }

        public GameObject GetFirstPanel(){
            return firstPanel;
        }

        public tabButton GetFirstTab(){
            return _firstTab;
        }

        public TabGroup GetTabGroup(){
            return _tabGroup;
        }

        public GameObject GetQuestPanel(){
            return _questPanel;
        }

        public Text GetTitle(){
            return _questTitleText;
        }
        public Text GetDescription(){
            return _questDescriptionText;
        }
        public Text GetAction(){
            return _questAction;
        }

        public Text GetReward(){
            return _questReward;
        }

        public GameObject GetTooltip(){
            return _tooltip;
        }
        public QuestUIHandler GetQuestHandler(){
            return _questHandler;
        }

        public GameObject getObjectiveMessage(){
            return _objectiveMessage;
        }

    }
}
