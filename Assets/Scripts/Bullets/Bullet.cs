using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region variables
    [SerializeField] protected float _speed;
    private Rigidbody2D _rb;
    private float _attackPower;
    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _rb.velocity = transform.right * _speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ApplyDamage(_attackPower);
        }

        Destroy(gameObject);
    }

    #endregion

    #region getter

    public float GetSpeed()
    {
        return _speed;
    }

    public float GetAttackPower()
    {
        return _attackPower;
    }

    #endregion

    #region setter

    public void SetSpeed(float value)
    {
        _speed = value;
    }

    public void SetAttackPower(float value)
    {
        _attackPower = value;
    }

    #endregion
}
