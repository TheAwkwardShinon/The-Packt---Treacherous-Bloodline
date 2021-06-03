using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Naturia_Bullet : PlayerBullet
    {
        

        //apply damage event (with low damage) + apply force
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
                player.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * 30f,ForceMode2D.Force);

                if (isLocalPlayer != isLocalBullet && isLocalBullet)
                {
                    
                    ApplicateforceInDirection evnt;

                    if (BoltNetwork.IsServer)
                    {
                        evnt = ApplicateforceInDirection.Create(player.entity.Source);
                    }
                    else
                    {
                        evnt = ApplicateforceInDirection.Create(BoltNetwork.Server);
                    }

                    evnt.TargetPlayerNetworkID = player.entity.NetworkId;
                    evnt.Direction = GetComponent<Rigidbody2D>().velocity * 30f;

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
