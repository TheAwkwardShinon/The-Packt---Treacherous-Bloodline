using UnityEngine;

namespace ThePackt
{
	public class MageEnemy : BaseEnemy
	{
		#region variables
		[SerializeField] private Transform _attackPoint;
		[SerializeField] private GameObject _bulletPrefab;
		#endregion

		#region methods

		public override void Attached()
		{
			base.Attached();

			_specificAttack = BulletAttack;
			_checkSpecificRange = CheckIfTargetIsInBulletRange;
		}

		private void CheckIfTargetIsInBulletRange()
		{
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

		private void BulletAttack()
		{
			Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, _attackRange, LayerMask.GetMask("Players"));

			foreach (var col in playersInRange)
			{
				if ((_target.transform.position.x <= transform.position.x && IsFacingLeft()) || (_target.transform.position.x >= transform.position.x && !IsFacingLeft()))
				{
					var evnt = PlayEnemySoundEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
					evnt.EntityID = entity.NetworkId;
					evnt.Sound = Constants.ATTACK;
					evnt.Send();

					_attackPoint.transform.right = col.bounds.center - _attackPoint.position;
					GameObject blt = BoltNetwork.Instantiate(_bulletPrefab, _attackPoint.position, _attackPoint.rotation);
					blt.GetComponent<Bullet>().SetAttackPower(_attackPower);
					_lastAttackTime = Time.time;
				}
			}
		}

		#endregion
	}
}
