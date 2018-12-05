using System.Collections;
using System.Collections.Generic;
using System.IO;                                                        // The System.IO namespace contains functions related to loading and saving files
using UnityEngine;

public class SaveLoad : MonoBehaviour {

    public bool initLoad;
    public bool deleteSaved;
    public static PhoneController Phone;
    public static GameObject character;
    public static ChangeSong Songs;
    public static GameObject SongIcon1;
    public static GameObject SongIcon2;
    public static GameObject SongIcon3;


    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
        character = GameObject.FindGameObjectWithTag("Player");
        Songs = GameObject.FindGameObjectWithTag("Level").GetComponent<ChangeSong>();
        
        if (Songs == null)
        {
            Songs = GameObject.Find("Nivel2").GetComponent<ChangeSong>();
            Debug.LogError("ERROR: CANT FIND A LEVEL OBJECT");
        }        Phone = GameObject.FindGameObjectWithTag("Phone").GetComponent<PhoneController>();
        GameObject[] list = GameObject.FindGameObjectsWithTag("songIcons");
        switch(list.Length)
        {
            case 3:
                SongIcon3 = list[2];
                SongIcon2 = list[1];
                SongIcon2 = list[0];
                break;
            case 2:
                SongIcon2 = list[1];
                SongIcon2 = list[0];
                break;
            case 1:
                SongIcon2 = list[0];
                break;
            default:
                Debug.Log("Tiene " + list.Length + " elementos. ¿¿WUAT??");
                break;
        }
        if(initLoad)
            LoadGameData();
        if (deleteSaved)
            deleteSavedData();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void deleteSavedData()
    {
        PlayerPrefs.DeleteAll();
    }
    public static void LoadGameData()
    {

        if (!LoadPosition())
            Debug.LogError("No se ha podido cargar la posicion");
        if (PlayerPrefs.HasKey("currentSong"))
            Songs.UptadeSong(PlayerPrefs.GetInt("currentSong"));
        if (PlayerPrefs.HasKey("timeSong"))
            Songs.audios[Songs.SongPlaying].time = PlayerPrefs.GetFloat("timeSong");
        if (SongIcon1 != null && PlayerPrefs.HasKey("songEnable1"))
            SongIcon1.SetActive((PlayerPrefs.GetInt("songEnable1") == 1)? true : false);
        if (SongIcon2 != null && PlayerPrefs.HasKey("songEnable2"))
            SongIcon2.SetActive((PlayerPrefs.GetInt("songEnable2") == 1) ? true : false);
        if (PlayerPrefs.HasKey("maxSongs"))
            Songs.maxSongs = PlayerPrefs.GetInt("maxSongs");
        if (PlayerPrefs.HasKey("volumeSong"))
            Phone.volumen = PlayerPrefs.GetFloat("VolumeSong");

        Phone.ChangeVolume(0f);
        Songs.UpdateLevel();
    }
    private static bool LoadPosition()
    {
        Vector3 position = character.transform.position;
        if (PlayerPrefs.HasKey("position_x") && PlayerPrefs.HasKey("position_y"))
        {
            position.x = PlayerPrefs.GetFloat("position_x");
            position.y = PlayerPrefs.GetFloat("position_y");
        }
        else
            return false;
        character.transform.position = position;
        return true;
    }
    public static void SaveGameData()
    {
        PlayerPrefs.SetFloat("position_x", character.transform.position.x);
        PlayerPrefs.SetFloat("position_y", character.transform.position.y);
        Debug.Log(character.transform.position);
        PlayerPrefs.SetInt("currentSong", Songs.SongPlaying);
        PlayerPrefs.SetFloat("timeSong", Songs.audios[Songs.SongPlaying].time);
        if(SongIcon1 != null)
            PlayerPrefs.SetInt("songEnable1", (SongIcon1.activeSelf) ? 1 : 0);
        if(SongIcon2 != null)
            PlayerPrefs.SetInt("songEnable2", (SongIcon2.activeSelf) ? 1 : 0);
        PlayerPrefs.SetInt("maxSongs", Songs.maxSongs);
        PlayerPrefs.SetFloat("volumeSong", Phone.volumen);

    }
}
