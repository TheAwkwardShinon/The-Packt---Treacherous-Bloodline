using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Fele_Bullet : PlayerBullet
    {


        protected override void EnemyHitReaction(Collider2D collision)
        {
            base.EnemyHitReaction(collision);
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
                    
                    PlayerAttackHitEvent evnt;

                    if (BoltNetwork.IsServer)
                    {
                        evnt = PlayerAttackHitEvent.Create(player.entity.Source);
                    }
                    else
                    {
                        evnt = PlayerAttackHitEvent.Create(BoltNetwork.Server);
                    }

                    evnt.HitNetworkId = player.entity.NetworkId;
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
