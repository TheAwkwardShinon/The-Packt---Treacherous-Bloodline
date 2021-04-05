using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThePackt.Werewolf;


namespace ThePackt{
    public class CombatSystem : MonoBehaviour
    {

        #region variables
        private Werewolf _player;
        #endregion

        #region methods

        void Start()
        {
            _player = GetComponent<Werewolf>();
        }

        public void BaseHumanAttack()  //method name always uppercase
        {
            Debug.Log("human attacking");

            GameObject blt = Instantiate(_player.GetBullet(), _player.GetAttackPoint().position, _player.GetAttackPoint().rotation);
            blt.GetComponent<Bullet>().SetAttackPower(_player.GetPowerBaseHumanAttack());

            _player.SetCurrentState(State.IDLE);
        }

        public void BaseWereWolfAttack()  //method name always uppercase
        {
            Debug.Log("werewolf attacking");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_player.GetAttackPoint().position, _player.GetRangeBaseWerewolfAttack(), 1 << LayerMask.NameToLayer("Enemies"));

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log(enemy.gameObject.name + " hit");
                enemy.gameObject.GetComponent<Enemy>().ApplyDamage(_player.GetPowerBaseWerewolfAttack());
                Debug.Log(enemy.gameObject.name + " health: " + enemy.gameObject.GetComponent<Enemy>().GetHealth());
            }

            _player.SetCurrentState(State.IDLE);
        }

        #endregion
    }
}