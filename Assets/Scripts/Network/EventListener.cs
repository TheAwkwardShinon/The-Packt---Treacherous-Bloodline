using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class EventListener : Bolt.GlobalEventListener
    {
        private Player _player;

        public override void EntityAttached(BoltEntity entity)
        {
            if (entity.IsOwner)
            {
                _player = entity.GetComponent<Player>();
            }
        }

        public override void OnEvent(AttackHitEvent evnt)
        {
            Debug.Log("[HEALTH] attack hit with damage: " + evnt.Damage);
            Debug.Log("[HEALTH] my network id recieved: " + evnt.EntityID);
            Debug.Log("[HEALTH] my network id owner: " + _player.entity.NetworkId.GetHashCode());

            if (evnt.EntityID == _player.entity.NetworkId.GetHashCode())
            {
                _player.ApplyDamage(evnt.Damage);
            }
        }
    }
}
