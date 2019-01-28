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
    Controller,
    SMS
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
    public GameObject[] SMS;
    //[HideInInspector]
    public int smsRecived;
    //[HideInInspector]
    public TMP_Dropdown GraphicSettings;
    //[HideInInspector]
    public GameObject player;
    //[HideInInspector]
    public Sprite notification;
    //[HideInInspector]
    public Image phoneIMG;
    [Range(25, 100)]
    public float volumen;
    public GameObject HUD;
    public GameObject redCircle;

    private GameObject AutoScroll;

    public KeyCode pauseInput = KeyCode.Escape;



    // Use this for initialization
    void Start()
    {
    
        volumen = 100;
        AudioListener.volume = 1;
        DisablePhone();
        SlideVolumen();
        Brillo.onValueChanged.AddListener(delegate { SlideShine(); }); //Sergi
        Volumen.onValueChanged.AddListener(delegate { SlideVolumen(); }); //Sergi
        GraphicSettings.onValueChanged.AddListener(delegate { QualitySettingsUpdate(); });
        GraphicSettings.value = QualitySettings.GetQualityLevel() + 1;
        Windowed.GetComponent<Image>().sprite = buttons[1];
        player = GameObject.FindGameObjectWithTag("Player");
        AutoScroll = FindObjectOfType<AutoScroll>().gameObject;
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
        }
    }

    private void ActivePhone()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        HUD.SetActive(false);
        gameObject.GetComponent<Canvas>().enabled = true;
        player.GetComponent<vThirdPersonInput>().enabled = false;
        player.GetComponent<vThirdPersonController>().input = Vector2.zero;
        player.GetComponent<vThirdPersonController>()._rigidbody2D.velocity = Vector2.zero;
        if (AutoScroll)
            AutoScroll.GetComponent<AutoScroll>().vel = 0;

        foreach (var s in screens)
            s.SetActive(false);
        screens[(int)currently].SetActive(true);
    }

    public void DisablePhone()
    {
        HUD.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currently = Screens.Main;
        player.GetComponent<vThirdPersonInput>().enabled = true;
        if (AutoScroll)
            AutoScroll.GetComponent<AutoScroll>().vel = 0.006f;

        gameObject.GetComponent<Canvas>().enabled = false;
        foreach (var s in screens)
            s.SetActive(false);

    }
    
    public void BackButton()
    {
        if (currently != Screens.Main)
        {
            if (currently == Screens.SMS)
            {
                screens[(int)currently].SetActive(false);
                currently = Screens.Main;
                screens[(int)currently].SetActive(true);
            }
            else
            {
                screens[(int)currently].SetActive(false);
                currently = ToScreen((int)currently - 1);
                screens[(int)currently].SetActive(true);
            }


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

    private void SlideVolumen() 
    {
        AudioListener.volume = Volumen.value / 100;
    }
    public void ChangeVolume(float vol)
    {
        volumen += vol;
        if (volumen >= 100)
            volumen = 100;
        if (volumen <= 25)
            volumen = 25;
        foreach(AudioSource audio in Songs.audios)
            audio.volume = volumen / 100;
        Songs.UpdateVolumen((volumen - 25) / 75);
    }

    private void QualitySettingsUpdate()
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
        PantallaDeCarga.Instancia.CargarEscena(Escenas.Menu.ToString());
    }

    public void SMSButton()
    {
        screens[(int)currently].SetActive(false);
        currently = Screens.SMS;
        screens[(int)currently].SetActive(true);

        phoneIMG.sprite = notification;
        CancelInvoke("DifuminePhoneIcon");
        CancelInvoke("FuminePhoneIcon");

        Color c = phoneIMG.color;
        c.a = 1;
        phoneIMG.color = c;

        redCircle.SetActive(false);

        for (int i = 0; i < SMS.Length; i++)
        {
            if (smsRecived > i)
                SMS[i].SetActive(true);
        }
    }
    public void MessageArrived()
    {
        InvokeRepeating("DifuminePhoneIcon", 0, 0.1f);
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
            case 4:
                return Screens.SMS;
            default:
                Debug.LogError("Se ha intetnado ir a la pantalla numero " + i + " que no existe");
                return Screens.Main;
        }
    }


    private void DifuminePhoneIcon()
    {
        Color c = phoneIMG.color;
        c.a -= 0.1f;
        phoneIMG.color = c;

        if (c.a < 0)
        {
            CancelInvoke("DifuminePhoneIcon");
            InvokeRepeating("FuminePhoneIcon", 0, 0.1f);            
        }
    }
    private void FuminePhoneIcon()
    {
        Color c = phoneIMG.color;
        c.a += 0.1f;
        phoneIMG.color = c;
        
        if (c.a > 1)
        {
            CancelInvoke("FuminePhoneIcon");
            InvokeRepeating("DifuminePhoneIcon", 0, 0.1f);
        }
    }
    public void BreakHeadphones()
    {
        foreach (AudioSource audio in Songs.audios)
            audio.panStereo = -1f;
    }
    public void FixedHeadphones()
    {
        foreach (AudioSource audio in Songs.audios)
            audio.panStereo = 0f;
    }
}
