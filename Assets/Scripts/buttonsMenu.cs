using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class buttonsMenu : MonoBehaviour {

    public Button Windowed;
    public Sprite[] buttons;
    public Slider Volumen;
    public TMP_Dropdown GraphicSettings;

    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Volumen.onValueChanged.AddListener(delegate { SlideVolumen(); });
        GraphicSettings.onValueChanged.AddListener(delegate { QualitySettingsUpdate(); });
    }
    public void WindowedButton()
    {
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Windowed.GetComponent<Image>().sprite = buttons[1];
           
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Windowed.GetComponent<Image>().sprite = buttons[0];
        }
    }
    private void SlideVolumen()
    {
        AudioListener.volume = Volumen.value / 100;
    }

    private void QualitySettingsUpdate() //Sergi
    {
        QualitySettings.SetQualityLevel(GraphicSettings.value - 1, true);
        Debug.Log("La configuracion ha cambiado ha: " + QualitySettings.names[GraphicSettings.value - 1]);
    }
    public void Play()
    {
        PantallaDeCarga.Instancia.CargarEscena(Escenas.Nivel1.ToString());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
