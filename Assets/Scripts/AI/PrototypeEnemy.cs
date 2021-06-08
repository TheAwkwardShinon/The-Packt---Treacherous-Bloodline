using UnityEngine;

namespace ThePackt
{
	public class PrototypeEnemy : BaseEnemy
	{
        #region variables
        
        #endregion

        #region methods

        public override void Attached()
		{
			base.Attached();

			_specificAttack = MeleeAttack;
			_checkSpecificRange = CheckIfTargetIsInMeleeRange;
        }

        private void CheckIfTargetIsInMeleeRange()
		{
			if(Time.time >= _lastAttackTime + _attackRate){

				Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, _attackRange, LayerMask.GetMask("Players"));

				foreach (var col in playersInRange)
				{
					if (_target == col.GetComponent<Player>().entity)
					{
						_attack = true;

						break;
					}
				}
			}
		}

		private void MeleeAttack()
        {
			Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, _attackRange, LayerMask.GetMask("Players"));

            foreach (Collider2D collision in playersInRange)
            {
                bool enemyReachable = false;
                if ((_target.transform.position.x <= transform.position.x && IsFacingLeft()) || (_target.transform.position.x >= transform.position.x && !IsFacingLeft()))
                {
                    Collider2D myCollider = GetComponent<Collider2D>();
                    float myIncrement = myCollider.bounds.size.y / 4;
                    Collider2D otherCollider = collision.GetComponent<Collider2D>();
                    float otherIncrement = otherCollider.bounds.size.y / 4;

                    float j = 0;
                    float i = 0;
                    for (float k = 0; k < 3; k++)
                    {
                        float originY = otherCollider.bounds.center.y + otherCollider.bounds.size.y / 2 - j;
                        if (originY > otherCollider.bounds.center.y)
                        {
                            originY -= 0.01f;
                        }
                        else
                        {
                            originY += 0.01f;
                        }

                        Vector2 target = new Vector2(myCollider.bounds.center.x, myCollider.bounds.center.y + myCollider.bounds.size.y / 2 - i);
                        Vector2 origin = new Vector2(otherCollider.bounds.center.x, originY);
                        Vector2 direction = target - origin;
                        var hit = Physics2D.Raycast(origin, direction, Vector2.Distance(target, origin) + 0.01f, LayerMask.GetMask("Players", "Ground", "Enemies", "Wall"));

                        if (hit.collider != null && hit.collider.gameObject == gameObject)
                        {
                            enemyReachable = true;
                            break;
                        }

                        i += myIncrement;
                        j += otherIncrement;
                    }

                    if (enemyReachable)
                    {
                        var evnt = PlayEnemySoundEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
                        evnt.EntityID = entity.NetworkId;
                        evnt.Sound = Constants.ATTACK;
                        evnt.Send();

                        BoltEntity hitEntity = collision.GetComponent<Player>().entity;
                        DealDamage(hitEntity);
                        SetHitTime(hitEntity);
                        _lastAttackTime = Time.time;
                    }
                }
            }
		}

        #endregion
    }
}
