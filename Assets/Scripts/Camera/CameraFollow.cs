using System.Collections;
using UnityEngine;

namespace ThePackt
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float _yOffset;
        private Transform _followTransform;

        //to have the camera bumping into the map bounds we have to set a boxcollider or the map bounds and give its reference
        //to this script. The yOffset would be useless at that point
        /*
        public BoxCollider2D mapBounds;
        private float xMin, xMax, yMin, yMax;
        private float camY, camX;
        private float camOrthsize;
        private float cameraRatio;
        private Camera mainCam;

        private void Start()
        {
            xMin = mapBounds.bounds.min.x;
            xMax = mapBounds.bounds.max.x;
            yMin = mapBounds.bounds.min.y;
            yMax = mapBounds.bounds.max.y;
            mainCam = Camera.main;
            camOrthsize = mainCam.orthographicSize;
            cameraRatio = (xMax + camOrthsize) / 2.0f;
        }
        */

        // Update is called once per frame
        void FixedUpdate()
        {
            if(_followTransform != null)
            {
                //camY = Mathf.Clamp(_followTransform.position.y, yMin + camOrthsize, yMax - camOrthsize);
                //camX = Mathf.Clamp(_followTransform.position.x, xMin + cameraRatio, xMax - cameraRatio);
                transform.position = new Vector3(_followTransform.position.x, _followTransform.position.y + _yOffset, transform.position.z);
            }
        }

        public void SetFollowTransform(Transform value)
        {
            _followTransform = value;
            Debug.Log("[CAMERA] setted to " + value);
        }
    }
}