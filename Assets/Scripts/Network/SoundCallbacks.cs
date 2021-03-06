using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class SoundCallbacks : NetworkCallbacks
    {
        [SerializeField] protected GameObject _enemyDeathSfx;
        [SerializeField] protected GameObject _humanDownSfx;
        [SerializeField] protected GameObject _wolfDownSfx;

        public override void OnEvent(PlayEnemySoundEvent evnt)
        {
            if(evnt.Sound != Constants.DEATH)
            {
                BoltEntity entity = BoltNetwork.FindEntity(evnt.EntityID);
                Enemy enemy = entity.GetComponent<Enemy>();
                switch (evnt.Sound)
                {
                    case Constants.ATTACK:
                        enemy.PlayAttackSFX();
                        break;
                    case Constants.HURT:
                        enemy.PlayHurtSFX();
                        break;
                    case Constants.WALK:
                        enemy.PlayWalkSFX();
                        break;
                }
            }
            else
            {
                Instantiate(_enemyDeathSfx, evnt.Position, Quaternion.identity);
            }
        }

        public override void OnEvent(PlayPlayerDeathSoundEvent evnt)
        {
            /*
            if (evnt.IsHuman)
            {
                Instantiate(_humanDownSfx, evnt.Position, Quaternion.identity);
            }
            else
            {
                Instantiate(_wolfDownSfx, evnt.Position, Quaternion.identity);
            }*/

            Instantiate(_wolfDownSfx, evnt.Position, Quaternion.identity);
        }

        public override void OnEvent(PlayBulletQuestSoundEvent evnt)
        {
            BoltEntity entity = BoltNetwork.FindEntity(evnt.RoomId);
            BulletQuest quest = entity.GetComponent<BulletQuest>();

            quest.PlayShotSound(evnt.CannonId);
        }
    }
}

