using UnityEngine;
using System.Collections;
namespace Invector.CharacterController
{
    public abstract class vThirdPersonAnimator : vThirdPersonMotor
    {
        public virtual void UpdateAnimator()
        {

        }
        public void OnAnimatorMove()
        {

            if (!isGrounded)
                if (_rigidbody2D.velocity.y <= 0)
                    animator.SetBool("Falling", true);
            if (_rigidbody2D.velocity.y == 0 && input != Vector2.zero)
                animator.SetBool("Walking", true);
            else
                animator.SetBool("Walking", false);

        }
    }
}