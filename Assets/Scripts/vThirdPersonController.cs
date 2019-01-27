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
        private void Update()
        {

        }

        public virtual void Jump()
        {
            // conditions to do this action
            bool jumpConditions = isGrounded && !isJumping;
            // return if jumpCondigions is false
            if (!jumpConditions) return;
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            animator.SetTrigger("Jumping");
            isJumping = true;   
        }
        
    }
    
}