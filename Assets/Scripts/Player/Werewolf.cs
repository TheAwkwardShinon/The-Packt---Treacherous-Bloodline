using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code design pattern sample: tell me if u like it or not.

namespace ThePackt{  //to be used in every class

    /* 
     * Models the all the playable characters
     */
    public class Werewolf : MonoBehaviour
    {
        //use it in order to make the code cleaner
        #region variables  
        protected Rigidbody2D _rb; // put '_' before every protected or private variable
        protected Animator _anim;
        [SerializeField] protected float _speed; //use serializefield if you want to initialize the variable from unity's inspector.
        [SerializeField] protected float _jumpForce;
        private Vector2 _direction;
        private bool _isGrounded = true;

        #endregion


        #region methods


        #region characterController

        /* method description using '/*' */
        /* 
         * Implements the standard movement
         */
        private void Move()  //method name always uppercase
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal"),0,0) * Time.deltaTime * _speed;
        }

        private void Jump()  //method name always uppercase
        {
            _isGrounded = false;
            _rb.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        }


        #endregion 

        // Start is called before the first frame update
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            //to be done later :  _anim = gameObject.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
            {
                Debug.Log("jump");
                Jump();
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log(collision.collider.gameObject.name);
            _isGrounded = true;
        }

        #endregion

        #region getter

        #endregion 

        #region setter

        #endregion
    }
}
