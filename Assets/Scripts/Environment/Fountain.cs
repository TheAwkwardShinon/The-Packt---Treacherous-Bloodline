using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class Fountain : Bolt.EntityBehaviour<IFountainState>
    {
        [SerializeField] protected Sprite _readySprite;
        [SerializeField] protected Sprite _notReadySprite;

        [SerializeField] private float _healAmount;
        [SerializeField] private float _cooldown;
        private float _lastUseTime;
        private int _state;

        // Start is called before the first frame update
        void Start()
        {
            if (entity.IsOwner)
            {
                state.State = Constants.READY;
            }

            state.AddCallback("State", StateCallback);
        }

        public override void SimulateOwner()
        {
            if (_state == Constants.COMPLETED && Time.time >= _lastUseTime + _cooldown)
            {
                Debug.Log("[FOUNTAIN] fountain reactivated");

                state.State = Constants.READY;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(BoltNetwork.IsServer && _state == Constants.READY)
            {
                Player enteredPlayer = collision.GetComponent<Player>();
                if (enteredPlayer != null && !enteredPlayer._isDowned && enteredPlayer.GetPlayerData().currentLifePoints < enteredPlayer.GetPlayerData().maxLifePoints)
                {
                    if (enteredPlayer.entity.IsOwner)
                    {
                        CharacterSelectionData.Instance.GetPlayerScript().FountainHeal(_healAmount);
                    }
                    else
                    {
                        FountainHealEvent evnt = FountainHealEvent.Create(enteredPlayer.entity.Source);
                        evnt.Amount = _healAmount;
                        evnt.TargetPlayerNetworkID = enteredPlayer.entity.NetworkId;
                        evnt.Send();
                    }

                    _lastUseTime = Time.time;
                    state.State = Constants.COMPLETED;
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (BoltNetwork.IsServer && _state == Constants.READY)
            {
                Player enteredPlayer = collision.GetComponent<Player>();
                if (enteredPlayer != null && !enteredPlayer._isDowned &&  enteredPlayer.GetPlayerData().currentLifePoints < enteredPlayer.GetPlayerData().maxLifePoints)
                {
                    if (enteredPlayer.entity.IsOwner)
                    {
                        CharacterSelectionData.Instance.GetPlayerScript().FountainHeal(_healAmount);
                    }
                    else
                    {
                        FountainHealEvent evnt = FountainHealEvent.Create(enteredPlayer.entity.Source);
                        evnt.Amount = _healAmount;
                        evnt.TargetPlayerNetworkID = enteredPlayer.entity.NetworkId;
                        evnt.Send();
                    }

                    _lastUseTime = Time.time;
                    state.State = Constants.COMPLETED;
                }
            }
        }

        private void StateCallback()
        {
            _state = state.State;

            if(_state == Constants.READY)
            {
                GetComponent<SpriteRenderer>().sprite = _readySprite;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = _notReadySprite;
            }
        }
    }
}

