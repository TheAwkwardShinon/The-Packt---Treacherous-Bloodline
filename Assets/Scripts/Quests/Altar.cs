using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt
{
    public class Altar : Bolt.EntityBehaviour<IAltarState>
    {
        private float _completeness;
        [SerializeField] private float _completeValue;
        [SerializeField] private float _increment;
        [SerializeField] private Sprite _completeSprite;
        [SerializeField] protected GameObject _completenessBar;
        [SerializeField] protected Image _completenessImage;
        [SerializeField] protected Gradient _completenessGradient;
        [SerializeField] private GameObject _chargeSfx;
        [SerializeField] private GameObject _activateSfx;
        private Slider _completenessSlider;

        public override void Attached()
        {
            _completenessSlider = _completenessBar.GetComponent<Slider>();
            _completenessImage.color = _completenessGradient.Evaluate(1f);
            _completenessSlider.maxValue = _completeValue;

            if (entity.IsOwner)
            {
                state.Completeness = 0;
                state.State = Constants.NOTCHARGING;
            }
            state.AddCallback("Completeness", CompletenessCallback);
            state.AddCallback("State", StateCallback);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //add player to list
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (BoltNetwork.IsServer)
            {
                Player player = collision.GetComponent<Player>();
                if (player != null)
                {
                    state.Completeness += _increment;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //remove player from list
        }

        private void CompletenessCallback()
        {
            _completeness = state.Completeness;

            _completenessSlider.value = _completeness;
            _completenessImage.color = _completenessGradient.Evaluate(_completenessSlider.normalizedValue);

            if (_completeness >= _completeValue)
            {
                _activateSfx.GetComponent<AudioSource>().Play();
                GetComponent<SpriteRenderer>().sprite = _completeSprite;
            }
        }

        private void StateCallback()
        {
            //handle sound
        }

        public bool IsActivated()
        {
            return _completeness >= _completeValue;
        }
    }
}
