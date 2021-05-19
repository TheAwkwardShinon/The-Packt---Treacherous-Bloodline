using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThePackt{
    public class BuyAbilityOnSkillTreeButton : MonoBehaviour, IPointerClickHandler
    {
        #region variables
        private InGameUI_tooltipAbilities _abilityInfo;
        [SerializeField] private SkillTreeData _skillTreeData;
        #endregion


        private void Start(){
            _abilityInfo = GetComponent<InGameUI_tooltipAbilities>();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogError("[BUY ABILITY] trying to buy the ability");
            _skillTreeData.BuyAbility(_abilityInfo.GetAbilityName(),_abilityInfo.GetCost());
        }
    }
}
