using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace Invector.CharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region variables

        [Header("Default Inputs")]
        public string horizontalInput = "Horizontal";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode lookPhoneInput = KeyCode.Tab;
        
        protected vThirdPersonController cc;                // access the ThirdPersonController component                

        #endregion

        protected virtual void Start()
        {
            CharacterInit();
        }

        protected virtual void CharacterInit()
        {
            cc = GetComponent<vThirdPersonController>();
            if (cc != null)
                cc.Init();      

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected virtual void LateUpdate()
        {
            if (cc == null) return;             // returns if didn't find the controller		    
            InputHandle();                      // update input methods
        }

        protected virtual void FixedUpdate()
        {
            cc.AirControl();
        }

        protected virtual void Update()
        {
            cc.UpdateMotor();                   // call ThirdPersonMotor methods      
        }

        protected virtual void InputHandle()
        {
            ExitGameInput();
            MoveCharacter();
            LookPhoneInput();
            JumpInput();
        }

        #region Basic Locomotion Inputs      

        protected virtual void MoveCharacter()
        {            
            cc.input.x = Input.GetAxis(horizontalInput);
        }

        protected virtual void LookPhoneInput()
        {
            if (Input.GetKeyDown(lookPhoneInput))
                cc.LookPhone();
        }       

        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput))
                cc.Jump();
        }        
        protected virtual void ExitGameInput()
        {
            // just a example to quit the application 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!Cursor.visible)
                    Cursor.visible = true;
            }
        }

        #endregion
    }
}