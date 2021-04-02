using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code design pattern sample: tell me if u like it or not.

namespace ThePackt{  //to be used in every class
    public class Werewolf : MonoBehaviour
    {
        //use it in order to make teh code cleaner
        #region variables  
        protected Rigidbody2D _rb; // put '_' before every protected or private variable
        protected Animator _anim;
        [SerializeField] protected float _speed; //use serializefield if you want to initialize the variable from unity's inspector.
        private Vector2 _direction;

        #endregion
        

        #region methods


        #region characterController

        /* method description using '/*' */
        private void Move()  //method name always uppercase
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal"),0,0) * Time.deltaTime * _speed;
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
        }

        #endregion

        #region getter

        #endregion 

        #region setter

        #endregion
    }
}
