using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Ayatana_Bullet1 : PlayerBullet
    {


        [SerializeField] private float _circleSize;
        [SerializeField] private float _debuffTime;




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
                    
                    ApplicateFogOfWarDebuffEvent evnt;

                    if (BoltNetwork.IsServer)
                    {
                        evnt = ApplicateFogOfWarDebuffEvent.Create(player.entity.Source);
                    }
                    else
                    {
                        evnt = ApplicateFogOfWarDebuffEvent.Create(BoltNetwork.Server);
                    }

                    evnt.TargetPlayerNetworkID = player.entity.NetworkId;
                    evnt.TimeOfDebuff = _debuffTime;
                    evnt.CircleSize = _circleSize;
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
