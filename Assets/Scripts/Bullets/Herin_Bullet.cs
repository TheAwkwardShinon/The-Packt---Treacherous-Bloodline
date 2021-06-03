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
            Enemy enemy;
            enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                EnemySlowEvent evnt;

                // if we are on the server, directly apply the damage to the enemy
                // otherwise we sent an event to the server
                if (BoltNetwork.IsServer)
                {
                    Debug.Log("[NETWORKLOG] server hit enemy");
                    // if the enemy has a dmg reduction debuff thank substract the dmg reduction value before applaying damage
                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        enemy.ApplyDamage((_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction, _owner);
                    else enemy.ApplyDamage((_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)), _owner);

                    enemy.ApplySlow(_timeOfSlow);
                }
                else
                {
                    Debug.Log("[NETWORKLOG] from client to server");
                    evnt = EnemySlowEvent.Create(BoltNetwork.Server);
                    evnt.HitNetworkId = enemy.entity.NetworkId;
                    evnt.AttackerNetworkId = _owner.entity.NetworkId;
                    evnt.SlowTime = _timeOfSlow;
                    // if the enemy has a dmg reduction debuff thank substract the dmg reduction value before applaying damage

                    if (_owner.GetPlayerData().isDmgReductionDebuffActive)
                        evnt.Damage = (_attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier)) - _owner.GetPlayerData().dmgReduction;
                    else evnt.Damage = _attackPower + (_attackPower * _owner.GetPlayerData().damageMultiplier);

                    evnt.Send();
                }
            }
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
