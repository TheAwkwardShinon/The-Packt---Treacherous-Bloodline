using UnityEngine;
using UnityEngine.UI;

namespace ThePackt
{
    public class FogOfWarSetting : MonoBehaviour
    {
        private Material _material;
        [SerializeField] private float _visibleDistance;
        [SerializeField] private float _shadedDistance;
        private CharacterSelectionData _charData;

        private void Start()
        {
            _material = GetComponent<RawImage>().material;
            _charData = CharacterSelectionData.Instance;
            _material.SetFloat("_VisibleDistance", _visibleDistance);
            _material.SetFloat("_ShadedDistance", _shadedDistance);
            GetComponent<RawImage>().material = _material;
        }

        private void Update()
        {
            _material.SetVector("_PlayerPosition", _charData.GetPlayerScript().transform.position);
        }
    }
}
