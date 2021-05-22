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

			foreach (var col in playersInRange)
			{
				if ((_target.transform.position.x <= transform.position.x && IsFacingLeft()) || (_target.transform.position.x >= transform.position.x && !IsFacingLeft()))
				{
					BoltEntity hitEntity = col.GetComponent<Player>().entity;
					DealDamage(hitEntity);
					SetHitTime(hitEntity);
					_lastAttackTime = Time.time;
				}
			}
		}

		#endregion
	}
}
