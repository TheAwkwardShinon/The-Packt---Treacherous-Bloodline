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

		///<summary>
		///makes the enemy change to attack state if the target is in _attackRange and could be reached by a bullet
		///</summary>
		private void CheckIfTargetIsInBulletRange()
		{
			Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, _attackRange, LayerMask.GetMask("Players"));

			foreach (var col in playersInRange)
			{
				if (_target == col.GetComponent<Player>().entity)
				{
					//ENHANCE: keep into consideration the bullet dimensions
					var hit = Physics2D.Raycast(_attackPoint.position, col.bounds.center - _attackPoint.position, Vector2.Distance(col.bounds.center, _attackPoint.position) + 0.2f, LayerMask.GetMask("Players", "Ground", "Wall", "Enemies", "Objectives"));
					
					if (hit && hit.collider.GetComponent<Player>() && _target == hit.collider.GetComponent<Player>().entity)
					{
						_attack = true;

						break;
					}
				}
			}
		}

		///<summary>
		///shoot a bullet in the target drection if the enemy is facinf towards the target
		///</summary>
		private void BulletAttack()
		{
			//if i'm facing towards the target
			if ((Time.time >= _lastAttackTime + _attackRate) && ((_target.transform.position.x <= transform.position.x && IsFacingLeft()) || (_target.transform.position.x >= transform.position.x && !IsFacingLeft())))
			{
				var evnt = PlayEnemySoundEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
				evnt.EntityID = entity.NetworkId;
				evnt.Sound = Constants.ATTACK;
				evnt.Send();

				//shoot
				_attackPoint.transform.right = _target.GetComponent<Collider2D>().bounds.center - _attackPoint.position;
				GameObject blt = BoltNetwork.Instantiate(_bulletPrefab, _attackPoint.position, _attackPoint.rotation);
				blt.GetComponent<Bullet>().SetAttackPower(_attackPower);
				blt.GetComponent<EnemyBullet>().SetOwner(this);
				_lastAttackTime = Time.time;
			}
		}
		#endregion
	}
}
