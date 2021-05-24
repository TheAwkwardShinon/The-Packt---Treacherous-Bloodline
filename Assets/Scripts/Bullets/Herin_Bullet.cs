using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Herin_Bullet : PlayerBullet
    {
        //[SerializeField] private float _slowValue;
        [SerializeField] private float _timeOfSlow;

        protected override void EnemyHitReaction(Collider2D collision)
        {
            base.EnemyHitReaction(collision); //TODO SLOWARE ANCHE I NEMICI
        }

        protected override bool PlayerHitReaction(Collider2D collision)
        {
            Player player;
            bool isLocalPlayer = false;
            bool isLocalBullet = true;

            player = collision.GetComponent<Player>();

            if (player != null)
            {
                isLocalPlayer = player.entity.IsOwner;
                isLocalBullet = entity.IsOwner;

                if (isLocalPlayer != isLocalBullet && isLocalBullet)
                {
                    
                    ApplicateSlowDebuffEvent evnt;

                    if (BoltNetwork.IsServer)
                    {
                        evnt = ApplicateSlowDebuffEvent.Create(player.entity.Source);
                    }
                    else
                    {
                        evnt = ApplicateSlowDebuffEvent.Create(BoltNetwork.Server);
                    }

                    evnt.TargetPlayerNetworkID = player.entity.NetworkId;
                    evnt.TimeOfSlow = _timeOfSlow;
                     if(_owner.GetPlayerData().isDmgReductionDebuffActive)
                        evnt.Damage = (_attackPower +( _attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction;
                    else evnt.Damage = _attackPower +(_attackPower * _owner.GetPlayerData().damageMultiplier);
                    evnt.Send();
                }
            }
            return isLocalPlayer;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void Start()
        {
            base.Start();
        }


    }
}
