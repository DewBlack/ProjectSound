using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace Invector.CharacterController
{
    public class SongInput : MonoBehaviour
    {
        #region variables

        [Header("Default Inputs")]
        public KeyCode nextSongInput = KeyCode.E;
        public KeyCode previusSongInput = KeyCode.Q;
        public KeyCode pauseSongInput = KeyCode.R;
        
        protected ChangeSong cs;                // access the ThirdPersonController component                

        #endregion

        protected virtual void Start()
        {
            cs = GetComponent<ChangeSong>();
            if (cs != null)
                cs.Init();
        }
        
        protected virtual void LateUpdate()
        {
            if (cs == null) return;             // returns if didn't find the controller		    
            InputHandle();                      // update input methods
        }
        

        protected virtual void Update()
        {
            cs.UpdateSongs();                   // call ThirdPersonMotor methods      
        }

        protected virtual void InputHandle()
        {
            PauseSongInput();
            NextSongInput();
            PreviusSongInput();
        }

        #region Basic Locomotion Inputs      

        protected virtual void PauseSongInput()
        {
            if (Input.GetKeyDown(pauseSongInput))
                
        }

        protected virtual void NextSongInput()
        {
            if (Input.GetKeyDown(nextSongInput))
        }       

        protected virtual void PreviusSongInput()
        {
            if (Input.GetKeyDown(previusSongInput))
        }  

        #endregion
    }
}