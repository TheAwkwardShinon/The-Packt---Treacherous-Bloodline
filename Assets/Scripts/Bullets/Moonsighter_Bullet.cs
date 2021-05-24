using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt{
    public class Moonsighter_Bullet : PlayerBullet
    {
        [SerializeField] private float _debuffTime;
        [SerializeField] private float _damageReductionValue;

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
                    
                    ApplicateDamageReductionEvent evnt;

                    if (BoltNetwork.IsServer)
                    {
                        evnt = ApplicateDamageReductionEvent.Create(player.entity.Source);
                    }
                    else
                    {
                        evnt = ApplicateDamageReductionEvent.Create(BoltNetwork.Server);
                    }

                    evnt.TargetPlayerNetworkID = player.entity.NetworkId;
                    evnt.TimeOfDebuff = _debuffTime;
                    evnt.DamageReduction = _damageReductionValue;
                    if(_owner.GetPlayerData().isDmgReductionDebuffActive)
                        evnt.Damage = (_attackPower +( _attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction;
                    else evnt.Damage = _attackPower +(_attackPower * _owner.GetPlayerData().damageMultiplier);
                    evnt.Send();
                }
            }
           

            return isLocalPlayer;
        }
    
    }
}
