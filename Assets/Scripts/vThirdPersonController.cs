using UnityEngine;
using System.Collections;
using System;

namespace Invector.CharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator

    {
        //private float nextFire;
        
        protected virtual void Start()
        {

        }
        
        public virtual void Jump()
        {
            // conditions to do this action
            bool jumpConditions = isGrounded && !isJumping && !isLookingPhone; 
            // return if jumpCondigions is false
            print ("la condicion para saltar es"+jumpConditions);
            print("ya esta saltando?"+ isJumping);
            if (!jumpConditions) return;
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            animator.SetTrigger("Jumping");
            isJumping = true;   
        }
        public virtual void LookPhone()
        {
            // conditions to do this action
            bool lookPhoneConditions = isGrounded && !isJumping && _rigidbody2D.velocity == Vector2.zero;

            if (!lookPhoneConditions) return;

            isLookingPhone = !isLookingPhone;
            animator.SetBool("LookPhone", isLookingPhone);
        }
    }
    
}