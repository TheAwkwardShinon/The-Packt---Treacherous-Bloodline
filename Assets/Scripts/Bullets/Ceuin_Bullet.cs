using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Ceuin_Bullet : PlayerBullet
    { 

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
                _owner.NextBullet(player.gameObject.tag);
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
