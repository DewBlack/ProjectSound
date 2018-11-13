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
        [Tooltip("Distance to became not grounded")]
        [SerializeField]
        protected float groundMinDistance = 0.2f;
        [SerializeField]
        protected float groundMaxDistance = 0.5f;
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

        [Header("Sound Efects Options")]

        [Tooltip("Insert your sound effects")]
        public AudioClip[] aClips;
        [Tooltip("Insert the object audio source")]
        public AudioSource myAudioSource;
        [Tooltip("Check to control the character while throwing")]
        public bool throwControl = true;
        [Tooltip("How much time the character will be thrpwing")]
        public float throwTimer = 0.3f;
        [HideInInspector]
        public float throwCounter;
        [Tooltip("Add Extra movement speed, based on your speed input the character will move forward")]
        public float throwForward = 3f;

        [Header("--- Movement Speed ---")]
        [Tooltip("Check to drive the character using RootMotion of the animation")]
        public bool useRootMotion = false;      
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeWalkSpeed = 2.5f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeRunningSpeed = 3f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeSprintSpeed = 4f;
        [Tooltip("Add extra speed for the strafe movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeWalkSpeed = 2.5f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeRunningSpeed = 3f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeSprintSpeed = 4f;

        [Header("--- Grounded Setup ---")]

        [Tooltip("ADJUST IN PLAY MODE - Offset height limit for sters - GREY Raycast in front of the legs")]
        public float stepOffsetEnd = 0.45f;
        [Tooltip("ADJUST IN PLAY MODE - Offset height origin for sters, make sure to keep slight above the floor - GREY Raycast in front of the legs")]
        public float stepOffsetStart = 0.05f;
        [Tooltip("Higher value will result jittering on ramps, lower values will have difficulty on steps")]
        public float stepSmooth = 4f;
        [Tooltip("Max angle to walk")]
        [SerializeField]
        protected float slopeLimit = 45f;       
        [Tooltip("Apply extra gravity when the character is not grounded")]
        [SerializeField]
        protected float extraGravity = -10f;
        protected float groundDistance;
        public RaycastHit2D groundHit;
              

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
        public PhysicMaterial maxFrictionPhysics, frictionPhysics, slippyPhysics;       // create PhysicMaterial for the Rigidbody
        [HideInInspector]
        public BoxCollider2D _boxCollider2D;                    // access CapsuleCollider information

        #endregion

        #region Hide Variables

        [HideInInspector]
        public float colliderHeight;                        // storage capsule collider extra information                
        [HideInInspector]
        public Vector2 input;                               // generate input for the controller        
        [HideInInspector]
        public float speed, verticalVelocity;    // general variables to the locomotion
        [HideInInspector]
        public float velocity;                              // velocity to apply to rigidbody    
        [HideInInspector]
        public Vector2 maxSpeed;

        #endregion

        #endregion

        public void Init()
        {
            // this method is called on the Start of the ThirdPersonController

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
            speed = input.x;
            speed = Mathf.Clamp(speed, -1f, 1f);
            maxSpeed = new Vector2(10, 5);
            if (input != Vector2.zero)
            {                
                // apply free directional rotation while not turning180 animations
                if (isGrounded || (!isGrounded && jumpAirControl)) //Se puede mover y tiene un Imput de moverse
                {
                    if (speed < 0)
                        transform.localScale = new Vector3(-1, 1, 1);
                    else
                        transform.localScale = new Vector3(1, 1, 1);
                        _rigidbody2D.velocity = new Vector2(speed * 5f, _rigidbody2D.velocity.y);
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
                var vel = transform.up * (jumpForward * speed);
                _rigidbody2D.velocity = new Vector2(velX.x, _rigidbody2D.velocity.y);
            }
            else
            {
                var vel = transform.forward * (jumpForward);
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            isGrounded = true;
            animator.SetBool("Falling", false);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            isGrounded = false;
        }
        #endregion
    }
}