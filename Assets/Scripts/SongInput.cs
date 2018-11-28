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
        public KeyCode TurnUpVolume = KeyCode.V;
        public KeyCode TurnDownVoulume = KeyCode.C;
        
        protected ChangeSong cs;                // access the ThirdPersonController component                

        #endregion

        protected virtual void Start()
        {
            cs = GetComponent<ChangeSong>();
            if (cs != null)
                cs.Init();
            else
                Debug.LogError("No se encuentra change Song");
        }
        
        protected virtual void LateUpdate()
        {
            if (cs == null) return;             // returns if didn't find the controller		    
                InputHandle();                  // update input methods
        }
        

        protected virtual void Update()
        { 
        }

        protected virtual void InputHandle()
        {
            PauseSongInput();
            NextSongInput();
            PreviusSongInput();
            TurnUpVolumeInput();
            TurnDownVolumeInput();
        }

        #region Basic Options Inputs      

        protected virtual void PauseSongInput()
        {

            if (Input.GetKeyDown(pauseSongInput))
                cs.PauseSong();
        }

        protected virtual void NextSongInput()
        {
            Debug.Log("NextSongInput");
            if (Input.GetKeyDown(nextSongInput))
                cs.NextSong();
        }       

        protected virtual void PreviusSongInput()
        {
            Debug.Log("PreviousSongInput");
            if (Input.GetKeyDown(previusSongInput))
                cs.PreviusSong();
        }  

        protected virtual void TurnUpVolumeInput()
        {
            if (Input.GetKey(TurnUpVolume))
                cs.TurnUpVolume();
        }

        protected virtual void TurnDownVolumeInput()
        {
            if (Input.GetKey(TurnDownVoulume))
                cs.TurnDownVolume();
        }

        #endregion
    }
}