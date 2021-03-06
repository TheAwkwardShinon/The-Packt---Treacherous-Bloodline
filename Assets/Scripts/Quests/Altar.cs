using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePackt
{
    public class Altar : Bolt.EntityBehaviour<IAltarState>
    {
        private float _completeness;
        [SerializeField] private float _fogOfWarDiameter;
        [SerializeField] private float _completeValue;
        [SerializeField] private float _completeValueLevelIncrement;
        [SerializeField] private float _increment;
        [SerializeField] private Sprite _completeSprite;
        [SerializeField] protected GameObject _completenessBar;
        [SerializeField] protected Image _completenessImage;
        [SerializeField] protected Gradient _completenessGradient;
        [SerializeField] private GameObject _chargeSfx;
        [SerializeField] private GameObject _activateSfx;
        private List<BoltEntity> _players;
        private bool _chargeStarted;

        private Slider _completenessSlider;

        public override void Attached()
        {
            _players = new List<BoltEntity>();
            _chargeStarted = false;

            var token = (LevelDataToken)entity.AttachToken;

            if (token != null)
            {
                _completeValue += _completeValueLevelIncrement * (token._level - 1);
            }

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

        public override void SimulateOwner()
        {
            foreach(var p in _players)
            {
                if (p.IsAttached)
                {
                    state.State = Constants.CHARGING;
                }
                else
                {
                    _players.Remove(p);
                }
            }

            Debug.Log("[CHARGE] " + _players.Count);

            if(_players.Count == 0)
            {
                state.State = Constants.NOTCHARGING;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                if(_completeness < _completeValue)
                {
                    player.SetFogOfWarDiameter(_fogOfWarDiameter);
                }

                if (BoltNetwork.IsServer)
                {
                    _players.Add(player.entity);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (BoltNetwork.IsServer && _completeness < _completeValue)
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
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.SetFogOfWarDiameter(player.GetPlayerData().standardCircleSize);

                if (BoltNetwork.IsServer)
                {
                    _players.Remove(player.entity);
                }
            }
        }

        private void CompletenessCallback()
        {
            _completeness = state.Completeness;

            _completenessSlider.value = _completeness;
            _completenessImage.color = _completenessGradient.Evaluate(_completenessSlider.normalizedValue);

            if (_completeness >= _completeValue)
            {
                _chargeSfx.GetComponent<AudioSource>().Stop();
                _activateSfx.GetComponent<AudioSource>().Play();
                GetComponent<SpriteRenderer>().sprite = _completeSprite;
            }
        }

        private void StateCallback()
        {
            if(_completeness < _completeValue)
            {
                if (state.State == Constants.NOTCHARGING && _chargeSfx.GetComponent<AudioSource>().isPlaying)
                {
                    _chargeSfx.GetComponent<AudioSource>().Pause();
                }
                else if (!_chargeSfx.GetComponent<AudioSource>().isPlaying)
                {
                    if (_chargeStarted)
                    {
                        _chargeSfx.GetComponent<AudioSource>().UnPause();
                    }
                    else
                    {
                        _chargeSfx.GetComponent<AudioSource>().Play();
                        _chargeStarted = true;
                    }
                }
            }
        }

        public bool IsActivated()
        {
            return _completeness >= _completeValue;
        }
    }
}
