using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace ThePackt
{
    public class Bullet : Bolt.EntityBehaviour<ICustomBulletState>
    {
        #region variables
        [SerializeField] protected float _speed;
        [SerializeField] protected float _range;
        [SerializeField] protected float _attackPower;
        // private NetworkId _playerNetworkId;
        private Rigidbody2D _rb;
        private Vector2 _startPos;
        #endregion

        #region methods
        // executed when the player prefab is instatiated (quite as Start())
        public override void Attached()
        {
            state.SetTransforms(state.Transform, transform);
        }

        // Start is called before the first frame update
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _rb.velocity = transform.right * _speed;
            _startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector2.Distance(transform.position, _startPos) >= _range)
            {
                Destroy(gameObject);
            }
        }

  
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Enemy enemy;
            Player player;
            bool isLocalPlayer = false;
            bool isLocalBullet = true;
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemies")
            {
                enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    EnemyAttackHitEvent evnt;
                    if (BoltNetwork.IsServer)
                    {
                        Debug.Log("[NETWORKLOG] server hit enemy");
                        enemy.ApplyDamage(_attackPower);
                    }
                    else
                    {
                        Debug.Log("[NETWORKLOG] from client to server");
                        evnt = EnemyAttackHitEvent.Create(BoltNetwork.Server);
                        evnt.HitNetworkId = enemy.entity.NetworkId;
                        evnt.Damage = _attackPower;
                        evnt.Send();
                    }
                }
            }
            else if (LayerMask.LayerToName(collision.gameObject.layer) == "Players")
            {
                player = collision.GetComponent<Player>();
                if (player != null)
                {
                    isLocalPlayer = player.entity.IsOwner;
                    isLocalBullet = entity.IsOwner;
                    Debug.Log("player is owner: " + isLocalPlayer);
                    Debug.Log("bullet is owner: " + isLocalBullet);

                    //If the player and the bullet have different owners a damage must be applied.
                    //When two player fight and neither of them is the owner, both the bullet and hit player will be false,
                    //so the bullet will not be destroyed or the damage applied -> which is wrong.
                    //Anyway there is a machine in which the owner is the hit player and that machine
                    //will identify that bullet and player have different owners and the damage will be applied,
                    //health will be synchronized and the bullet destroyed for every player (thanks to BoltNetwork.Destroy)
                    if (isLocalPlayer != isLocalBullet)
                    {
                        Debug.Log("[HEALTH] hit other player: " + collision.gameObject.name);

                        /*
                        Debug.Log("[HEALTH] other network id: " + player.entity.NetworkId.GetHashCode());
                        Debug.Log("[HEALTH] my player network id: " + _playerNetworkId);
                        */

                        PlayerAttackHitEvent evnt;
                        if (BoltNetwork.IsServer)
                        {
                            Debug.Log("[NETWORKLOG] from server to connection: " + player.getConnectionID());
                            evnt = PlayerAttackHitEvent.Create(player.entity.Source);
                        }
                        else
                        {
                            Debug.Log("[NETWORKLOG] from client to server. must redirect to: " + player.getConnectionID());
                            evnt = PlayerAttackHitEvent.Create(BoltNetwork.Server);
                            evnt.HitConnectionID = (int) player.getConnectionID();
                        }

                        //evnt.HitNetworkID = player.entity.NetworkId;
                        evnt.Damage = _attackPower;
                        evnt.Send();
                    }
                }
            }
            
            // Does not destroy bullets on impact with other bullets or the local player
            if (!(LayerMask.LayerToName(collision.gameObject.layer) == "Bullets") && isLocalPlayer != isLocalBullet)
            {
                BoltNetwork.Destroy(gameObject);
            }

            /*
            // Destroy bullet on impact with ground or walls 
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground" || LayerMask.LayerToName(collision.gameObject.layer) == "Wall")
            {
                BoltNetwork.Destroy(gameObject);
            }
            */
        }
 

        public void Die()
        {
            BoltNetwork.Destroy(gameObject);
        }

        #endregion

        #region getter

        public float GetSpeed()
        {
            return _speed;
        }

        public float GetAttackPower()
        {
            return _attackPower;
        }

        #endregion

        #region setter

        public void SetSpeed(float value)
        {
            _speed = value;
        }

        public void SetAttackPower(float value)
        {
            _attackPower = value;
        }

        /*
        public void SetPlayerNetworkId(NetworkId value)
        {
            _playerNetworkId = value;
        }
        */

        #endregion
    }

}
