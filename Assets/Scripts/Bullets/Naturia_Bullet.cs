using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThePackt{
    public class Naturia_Bullet : PlayerBullet
    {
        [SerializeField] private float _knockbackPower;

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
                    evnt.Power = _knockbackPower;

                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        evnt.Damage = (_attackPower +( _attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction;
                    else evnt.Damage = _attackPower +(_attackPower * _owner.GetPlayerData().damageMultiplier);
                    evnt.Send();
                }
            }
            return isLocalPlayer;
        }

        protected override void EnemyHitReaction(Collider2D collision)
        {
            Enemy enemy;
            enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                EnemyKnockbackEvent evnt;

                // if we are on the server, directly apply the damage to the enemy
                // otherwise we sent an event to the server
                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[NETWORKLOG] server hit enemy");
                    // if the enemy has a dmg reduction debuff thank substract the dmg reduction value before applaying damage
                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        enemy.ApplyDamage((_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction, _owner);
                    else enemy.ApplyDamage((_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)), _owner);

                    enemy.ApplyKnockback(GetComponent<Rigidbody2D>().velocity * 30f, _knockbackPower);
                }
                else
                {
                    Debug.Log("[NETWORKLOG] from client to server");
                    evnt = EnemyKnockbackEvent.Create(BoltNetwork.Server);
                    evnt.HitNetworkId = enemy.entity.NetworkId;
                    evnt.AttackerNetworkId = _owner.entity.NetworkId;
                    evnt.Direction = GetComponent<Rigidbody2D>().velocity * 30f;
                    evnt.KnockbackPower = _knockbackPower;
                    // if the enemy has a dmg reduction debuff thank substract the dmg reduction value before applaying damage

                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        evnt.Damage = (_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction;
                    else evnt.Damage = _attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier);

                    evnt.Send();
                }
            }
        }
    }
}
