 using UnityEngine;
 using System.Collections;
 using UnityEngine.UI;
 using UnityEngine.EventSystems;

namespace ThePackt{
    public class MoveScrollbar : MonoBehaviour {
    
        private RectTransform scrollRectTransform;
        private RectTransform contentPanel;
        private RectTransform selectedRectTransform;
        private GameObject lastSelected;
    
        private void Start() {
            scrollRectTransform = GetComponent<RectTransform>();
            contentPanel = GetComponent<ScrollRect>().content;
        }
    
        private void Update() {
            // Get the currently selected UI element from the event system.
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            Debug.Log("selected is: "+selected);
            selected = selected.transform.parent.gameObject;
            
            // Return if there are none.
            if (selected == null) {
                Debug.Log("selected null");
                return;
            }
            // Return if the selected game object is not inside the scroll rect.
            if (selected.transform.parent != contentPanel.transform) {
                Debug.Log("selected not inside the scroll rect");
                return;
            }
            // Return if the selected game object is the same as it was last frame,
            // meaning we haven't moved.
            if (selected == lastSelected) {
                Debug.Log("selected is last selected");
                return;
            }
    
            // Get the rect tranform for the selected game object.
            selectedRectTransform = selected.GetComponent<RectTransform>();
            // The position of the selected UI element is the absolute anchor position,
            // ie. the local position within the scroll rect + its height if we're
            // scrolling down. If we're scrolling up it's just the absolute anchor position.
            float selectedPositionY = Mathf.Abs(selectedRectTransform.anchoredPosition.y) + selectedRectTransform.rect.height;
    
            // The upper bound of the scroll view is the anchor position of the content we're scrolling.
            float scrollViewMinY = contentPanel.anchoredPosition.y;
            

            // The lower bound is the anchor position + the height of the scroll rect.
            float scrollViewMaxY = contentPanel.anchoredPosition.y + scrollRectTransform.rect.height;
            Debug.Log("scrollViewMiny = "+scrollViewMinY);
            Debug.Log("scrollViewMaxy = "+scrollViewMaxY);
            Debug.Log("selectPositionY = "+selectedPositionY);
            Debug.Log("selectRectTransform = "+selectedRectTransform.name);
        
            
            // If the selected position is below the current lower bound of the scroll view we scroll down.
            if (selectedPositionY > scrollViewMaxY) {
                float newY = selectedPositionY - scrollRectTransform.rect.height;
                contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, newY);
            }
            // If the selected position is above the current upper bound of the scroll view we scroll up.
            else if (Mathf.Abs(selectedRectTransform.anchoredPosition.y) < scrollViewMinY) {
                contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, Mathf.Abs(selectedRectTransform.anchoredPosition.y));
            }
    
            lastSelected = selected;
        }
    }
}
