using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region variables
    [SerializeField] protected float _speed;
    [SerializeField] protected float _range;
    private Rigidbody2D _rb;
    private float _attackPower;
    private Vector2 _startPos;
    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _rb.velocity = transform.right * _speed;
        _startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, _startPos) >= _range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ApplyDamage(_attackPower);
        }

        // Does not destroy bullets on impact with player or other bullets 
        if (!collision.gameObject.CompareTag("Player") & !(LayerMask.LayerToName(collision.gameObject.layer) == "Bullets"))
        {
            Destroy(gameObject);
        }
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
