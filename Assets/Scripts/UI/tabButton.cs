using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThePackt{

    [RequireComponent(typeof(Image))]
    public class tabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler
    {
        public TabGroup tabGroup;

        public Image background;

        [SerializeField] private GameObject _tooltip;


        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
            _tooltip.SetActive(false);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
            _tooltip.SetActive(false);
        }


        private void Start()
        {
            background = GetComponent<Image>();
            //tabGroup.Subscribe(this);
        }

        private void Update()
        {
            
        }
    }
}
