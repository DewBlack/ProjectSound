using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Invector;
using UnityEngine.EventSystems;

namespace Invector.CharacterController
{
    public abstract class vThirdPersonMotor : MonoBehaviour
    {
        #region Variables        

        #region Layers
        [Header("---! Layers !---")]
        [Tooltip("Layers that the character can walk on")]
        public LayerMask groundLayer = 1 << 0;
        #endregion

        #region Character Variables       
   
        [Header("--- Locomotion Setup ---")]
         
        [Header("Jump Options")]

        [Tooltip("Check to control the character while jumping")]
        public bool jumpAirControl = true;
        [Tooltip("How much time the character will be jumping")]
        public float jumpTimer = 0.3f;
        [HideInInspector]
        public float jumpCounter;
        [Tooltip("Add Extra jump speed, based on your speed input the character will move forward")]
        public float jumpForward = 3f;
        [Tooltip("Add Extra jump height, if you want to jump only with Root Motion leave the value with 0.")]
        public float jumpHeight = 4f;


        [Header("---! AudioSource Setup !---")]

        [Tooltip("Aduio del Cling que quiere Afor")]
        public AudioSource Cling;
        [Tooltip("Aduio de sacar movil que quiere Afor")]
        public AudioSource phoneLook;
        [Tooltip("Aduio de la muerte en el sentits que quiere Afor")]
        public AudioSource deathSentits;
        [Tooltip("Aduio de cuando muere que quiere Afor")]
        public AudioSource death;
        [Tooltip("Aduio de muchedumbre hablando que quiere Afor")]
        public AudioSource talk;
        [Tooltip("Aduio de pasar de nivel que quiere Afor")]
        public AudioSource nextLevel; 




        #region Actions

        // movement bools
        [HideInInspector]
        public bool
            isGrounded,
            isSliding;



        // action bools
        [HideInInspector]
        public bool
            isJumping,
            isLookingPhone;

        #endregion

        protected void RemoveComponents()
        {
            if (_boxCollider2D != null) Destroy(_boxCollider2D);
            if (_rigidbody2D != null) Destroy(_rigidbody2D);
            if (animator != null) Destroy(animator);
            var comps = GetComponents<MonoBehaviour>();
            for (int i = 0; i < comps.Length; i++)
            {
                Destroy(comps[i]);
            }
        }

        #endregion      

        #region Components               

        [HideInInspector]
        public Animator animator;                                   // access the Animator component
        [HideInInspector]
        public Rigidbody2D _rigidbody2D;                                // access the Rigidbody component       
        [HideInInspector]
        public BoxCollider2D _boxCollider2D;                    // access CapsuleCollider information

        #endregion

        #region Hide Variables

        [HideInInspector]
        public Vector2 input;                               // generate input for the controller        
        //[HideInInspector]
        public float speed;    // general variables to the locomotion
        //[HideInInspector]
        public float velocity;                              // velocity to apply to rigidbody    
        //[HideInInspector]
        public Vector2 maxSpeed;
        [HideInInspector]
        public Vector2 scale;
        public float vel;
        public bool stoped;

        #endregion

        #endregion

        public void Init()
        {
            // this method is called on the Start of the ThirdPersonController

            scale = transform.localScale;
            // access components
            animator = GetComponent<Animator>();

            // rigidbody info
            _rigidbody2D = GetComponent<Rigidbody2D>();

            // capsule collider info
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        public virtual void UpdateMotor()
        {
            ControlJumpBehaviour();
            ControlLocomotion();
            if (_rigidbody2D.velocity.y != 0)
                isGrounded = false;
        }

        #region Locomotion 

       
        void ControlLocomotion()
        {
            FreeMovement();     // free directional movement
        }

        void StrafeMovement()
        {
            speed = Mathf.Clamp(input.x, -1f, 1f);
        }

        public virtual void FreeMovement()
        {
            // set speed to both vertical and horizontal inputs
            if (stoped)
                return;

            speed = input.x;
            speed = Mathf.Clamp(speed, -1f, 1f);
            maxSpeed = new Vector2(10, 5);
            if (input != Vector2.zero)
            {                
                // apply free directional rotation while not turning180 animations
                if (isGrounded || (!isGrounded && jumpAirControl)) //Se puede mover y tiene un Imput de moverse
                {
                    if (speed < 0)
                    {
                        scale.x = scale.x < 0 ? -scale.x : scale.x;
                        transform.localScale = new Vector3(scale.x, scale.y, transform.localScale.z);
                    }
                    else
                    {
                        scale.x = scale.x < 0 ? scale.x : -scale.x;
                        transform.localScale = new Vector3(scale.x, scale.y, transform.localScale.z);
                    }
                    _rigidbody2D.velocity = new Vector2(speed * vel, _rigidbody2D.velocity.y);
                }
            }

        }
        #endregion

        #region Jump Methods

        protected void ControlJumpBehaviour()
        {
            if (!isJumping) return;

            jumpCounter -= Time.deltaTime;
            if (jumpCounter <= 0)
            {
                jumpCounter = 0;
                isJumping = false;
            }
            // apply extra force to the jump height   
            var vel = _rigidbody2D.velocity;
            vel.y = jumpHeight;
            _rigidbody2D.velocity = vel;
        }

        
        public void AirControl()
        {
            if (isGrounded) return;
            if (!jumpFwdCondition) return;

            var velY = transform.up * jumpForward * speed;
            velY.y = _rigidbody2D.velocity.y;
            var velX = transform.right * jumpForward;
            velX.x = _rigidbody2D.velocity.x;

            if (jumpAirControl)
            {
                _rigidbody2D.velocity = new Vector2(velX.x, _rigidbody2D.velocity.y);
            }
            else
            {
                _rigidbody2D.velocity = new Vector2(velX.x, _rigidbody2D.velocity.y);
            }
        }

        protected bool jumpFwdCondition
        {
            get
            {
                Vector3 p1 = transform.position + _boxCollider2D.transform.position + Vector3.up * -_boxCollider2D.size.y * 0.5F;
                Vector3 p2 = p1 + Vector3.up * _boxCollider2D.size.y;
                return Physics2D.BoxCastAll(p1, p2, _boxCollider2D.size.y /2 * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
            }
        }

        #endregion

        #region Ground Check    

       
        private void OnCollisionExit2D(Collision2D collision)
        {
            isGrounded = false;
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_rigidbody2D.velocity.y == 0)
            {
                isGrounded = true;
                animator.SetBool("Falling", false);
            }
           
        }
        #endregion
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "songIcons")
                if(Cling != null)
                Cling.Play();
        }
    }
}