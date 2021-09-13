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
			_checkSpecificRange = IsTargetInBulletRange;
		}

		/*
        private void OnDrawGizmos()
        {
			Matrix4x4 rotationMatrix = Matrix4x4.TRS(_attackPoint.position, Quaternion.FromToRotation(Vector2.right, new Vector3(1f,1f,0f) - _attackPoint.position), Vector2.one);
			Gizmos.matrix = rotationMatrix;

			Gizmos.color = Color.magenta;
			Gizmos.DrawCube(_attackPoint.position, new Vector2(0.1f, _bulletPrefab.GetComponent<BoxCollider2D>().size.y * _bulletPrefab.transform.lossyScale.y));
		}
		*/

        ///<summary>
        ///makes the enemy change to attack state if the target is in _attackRange and could be reached by a bullet
        ///</summary>
        private bool IsTargetInBulletRange()
		{
			Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, _attackRange, LayerMask.GetMask("Players"));

			foreach (var col in playersInRange)
			{
				if (_target == col.GetComponent<Player>().entity)
				{
					float bulletY = _bulletPrefab.GetComponent<BoxCollider2D>().size.y * _bulletPrefab.transform.lossyScale.y;
					float angle = Vector2.Angle(Vector2.right, col.bounds.center - _attackPoint.position);
					var hit = Physics2D.BoxCast(_attackPoint.position, new Vector2(0.1f, bulletY), angle, col.bounds.center - _attackPoint.position, Vector2.Distance(col.bounds.center, _attackPoint.position), LayerMask.GetMask("Players", "Ground", "Wall"));
					
					if (hit && hit.collider.GetComponent<Player>() && _target == hit.collider.GetComponent<Player>().entity)
					{ 
						return true;
					}
				}
			}

			return false;
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
