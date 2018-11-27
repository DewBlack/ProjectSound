using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Screens
{
    Main,
    Options,
    Options_Unhide,
    Controller
}
public class PhoneController : MonoBehaviour {

    public Screens currently;

    //[HideInInspector]    
    public GameObject[] screens;
    //[HideInInspector]
    public ChangeSong Songs;
    //[HideInInspector]
    public Button Windowed;
    //[HideInInspector]
    public Slider Brillo;
    //[HideInInspector]
    public Image Opacidad;
    //[HideInInspector]
    public Slider Volumen;
    //[HideInInspector]
    public Sprite[] buttons;
    //[HideInInspector]
    public TMP_Dropdown GraphicSettings;
    //[HideInInspector]
    public GameObject player;

    public KeyCode pauseInput = KeyCode.Escape;



    // Use this for initialization
    void Start()
    {
        DisablePhone();
        SlideVolumen();
        Brillo.onValueChanged.AddListener(delegate { SlideShine(); }); //Sergi
        Volumen.onValueChanged.AddListener(delegate { SlideVolumen(); }); //Sergi
        GraphicSettings.onValueChanged.AddListener(delegate { QualitySettingsUpdate(); });
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(pauseInput))
        {
            if (gameObject.GetComponent<Canvas>().enabled)
                DisablePhone();
            else
                ActivePhone();
            Debug.Log("Entra");
        }
    }

    private void ActivePhone()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameObject.GetComponent<Canvas>().enabled = true;
        player.GetComponent<vThirdPersonInput>().enabled = false;

        foreach (var s in screens)
            s.SetActive(false);
        screens[(int)currently].SetActive(true);
    }

   public void DisablePhone()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.GetComponent<vThirdPersonInput>().enabled = true;
        gameObject.GetComponent<Canvas>().enabled = false;
        foreach (var s in screens)
            s.SetActive(false);

    }

    public void BackButton()
    {
        if (currently != Screens.Main)
        {
            screens[(int)currently].SetActive(false);
            currently = ToScreen((int)currently - 1);
            screens[(int)currently].SetActive(true);
        }
        else
        {
            currently = Screens.Main;
            DisablePhone();

        }
    }

    public void PreviusSongButton()
    {
        Songs.PreviusSong();
    }

    public void NextSongButton()
    {
        Songs.NextSong();
    }

    public void PausePlaySongButton()
    {
        Songs.PauseSong();
    }

    public void NextPhoneButton()
    {
        screens[(int)currently].SetActive(false);
        currently = ToScreen((int)currently + 1);
        screens[(int)currently].SetActive(true);
    }
    
    private void SlideShine()
    {
        var tempColor = Opacidad.color;
        tempColor.a = (100 - Brillo.value) / 100;
        Opacidad.color = tempColor;
    }
    
    private void SlideVolumen() //Sergi
    {
        if (Volumen.value < 25)        
            Volumen.value = 25;
        
        AudioListener.volume = Volumen.value - 100;
        Songs.UpdateVolumen((Volumen.value - 25) / 75);
    }
    public void ChangeVolume(float vol)
    {
        AudioListener.volume += vol;
        if (AudioListener.volume <= 0.25f)
            AudioListener.volume = 0.25f;
        Volumen.value = AudioListener.volume;
        Songs.UpdateVolumen((Volumen.value - 25) / 75);
    }

    private void QualitySettingsUpdate() //Sergi
    {
        QualitySettings.SetQualityLevel(GraphicSettings.value - 1, true);
        Debug.Log("La configuracion ha cambiado ha: " + QualitySettings.names[GraphicSettings.value - 1]);
    }

    public void RestartButton()
    {
        currently = Screens.Main;
        DisablePhone();
    }

    public void MainMenuButton()
    {
        //SceneManager.LoadScene("Main Menu", LoadSceneMode.);
    }

    public void WindowedButton()
    {
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Windowed.GetComponent<Image>().sprite = buttons[1];
            Debug.Log("Estas en modo FullScreen");
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Windowed.GetComponent<Image>().sprite = buttons[0];
            Debug.Log("Estas en modo Windowed");
        }

    }

    private Screens ToScreen(int i)
    {
        switch (i)
        {
            case 0:
                return Screens.Main;
            case 1:
                return Screens.Options;
            case 2:
                return Screens.Options_Unhide;
            case 3:
                return Screens.Controller;
            default:
                Debug.LogError("Se ha intetnado ir a la pantalla numero " + i + " que no existe");
                return Screens.Main;
        }
    }
}
