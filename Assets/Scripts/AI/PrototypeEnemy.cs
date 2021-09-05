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
            _checkSpecificRange = IsTargetInMeleeRange;
        }

        ///<summary>
		///makes the enemy change to attack state if the target is in _attackRange
		///</summary>
        private bool IsTargetInMeleeRange()
        {
            Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, _attackRange, LayerMask.GetMask("Players"));

            foreach (var col in playersInRange)
                if (_target == col.GetComponent<Player>().entity)
                {
                    return true;
                }

            return false;
        }

        ///<summary>
		///deal damage to every player in _attackRange which is in front of the enemy ad that is not behind a wall
		///</summary>
		private void MeleeAttack()
        {
            Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, _attackRange, LayerMask.GetMask("Players"));

            foreach (Collider2D collision in playersInRange)
            {
                //if the player is in front of me
                if ((collision.transform.position.x <= transform.position.x && IsFacingLeft()) || (collision.transform.position.x >= transform.position.x && !IsFacingLeft()))
                {
                    //if the enemy is reachable deal the damage
                    if (IsPlayerReachable(collision))
                    {
                        var evnt = PlayEnemySoundEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
                        evnt.EntityID = entity.NetworkId;
                        evnt.Sound = Constants.ATTACK;
                        evnt.Send();

                        BoltEntity hitEntity = collision.GetComponent<Player>().entity;
                        DealDamage(hitEntity);

                        _lastAttackTime = Time.time;
                        RegisterTargetHit(hitEntity);
                    }
                }
            }
        }

        ///<summary>
		///returns true if the specified collider is not completely behind a wall, false otherwise 
		///</summary>
		private bool IsPlayerReachable(Collider2D collision)
        {
            bool enemyReachable = false;

            //define the increments as a quarter of the colliders
            Collider2D myCollider = GetComponent<Collider2D>();
            float myIncrement = myCollider.bounds.size.y / 4;
            Collider2D otherCollider = collision.GetComponent<Collider2D>();
            float otherIncrement = otherCollider.bounds.size.y / 4;

            //shoot 4 rays starting from the player collider to the enemy collider: 1 from top to top, 1 from 3/4 to 3/4,
            //1 from middle to middle, 1 from 1/4 to 1/4 and 1 from bottom to bottom
            float j = 0;
            float i = 0;
            for (float k = 0; k < 3; k++)
            {
                //determine the ray origin
                float originY = otherCollider.bounds.center.y + otherCollider.bounds.size.y / 2 - j;
                //make the ray start always inside the player collider
                if (originY > otherCollider.bounds.center.y)
                    originY -= 0.01f;
                else
                    originY += 0.01f;

                //determine the ray target
                Vector2 target = new Vector2(myCollider.bounds.center.x, myCollider.bounds.center.y + myCollider.bounds.size.y / 2 - i);
                Vector2 origin = new Vector2(otherCollider.bounds.center.x, originY);

                //shoot the ray
                Vector2 direction = target - origin;
                var hit = Physics2D.Raycast(origin, direction, Vector2.Distance(target, origin) + 0.01f, LayerMask.GetMask("Players", "Ground", "Enemies", "Wall"));

                //if at least one of the rays hits the enemy it means that the player is not completely hidden by a wall and so it is
                //hit by the attack
                if (hit.collider && hit.collider.gameObject == gameObject)
                {
                    enemyReachable = true;
                    break;
                }

                i += myIncrement;
                j += otherIncrement;
            }

            return enemyReachable;
        }
        #endregion
    }
}
