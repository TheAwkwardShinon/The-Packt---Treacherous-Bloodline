using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class BulletSpawnPoint : MonoBehaviour
    {
        [SerializeField] private int id;
        [SerializeField] private float _fireRate;
        [SerializeField] private float _maxDegreeOfFire;
        private float _lastFireTime;

        public void Shoot(GameObject bulletPrefab)
        {
            if (Time.time >= _lastFireTime + _fireRate)
            {
                var evnt = PlayBulletQuestSoundEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
                evnt.RoomId = GetComponentInParent<BulletQuest>().entity.NetworkId;
                evnt.CannonId = id;
                evnt.Send();

                if (BoltNetwork.IsServer)
                {
                    float angle = UnityEngine.Random.Range(-_maxDegreeOfFire, _maxDegreeOfFire);
                    BoltNetwork.Instantiate(bulletPrefab, transform.position, transform.rotation * Quaternion.Euler(0, 0, angle));

                    _lastFireTime = Time.time;
                }
            }
        }

        public int GetId()
        {
            return id;
        }
    }
}
