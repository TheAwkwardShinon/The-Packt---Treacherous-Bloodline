using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePackt
{
    public class SoundCallbacks : NetworkCallbacks
    {
        public override void OnEvent(PlayEnemySoundEvent evnt)
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
                case Constants.DEATH:
                    enemy.PlayDeathSFX();
                    break;
            }
        }

        public override void OnEvent(PlayBulletQuestSoundEvent evnt)
        {
            BoltEntity entity = BoltNetwork.FindEntity(evnt.RoomId);
            BulletQuest quest = entity.GetComponent<BulletQuest>();

            quest.PlayShotSound(evnt.CannonId);
        }
    }
}

