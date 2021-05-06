using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThePackt{
    public class tabVerticalButton : MonoBehaviour, IPointerClickHandler,ISelectHandler
    {
        [SerializeField] public VerticalTabGroup tabGroup;

        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void OnSelect(BaseEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }
    }
}

