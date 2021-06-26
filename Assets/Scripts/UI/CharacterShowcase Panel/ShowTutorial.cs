using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThePackt
{
    public class ShowTutorial : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private GameObject _tooltip;

        public void OnDeselect(BaseEventData eventData)
        {
            _tooltip.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tooltip.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tooltip.SetActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            _tooltip.SetActive(true);
        }
    }
}

