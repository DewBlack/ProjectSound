using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSong : MonoBehaviour {
    
    public AudioSource[] audios;
    public GameObject[] levels;
    public GameObject[] moviles;
    public int SongPlaying;
    public int maxSongs;
    public float modificador;

    public virtual void Init()
    {
        audios[SongPlaying].Play();
    
    }
    

    public virtual void NextSong()
    {
        if (maxSongs == 0) return;
        
        int aux = (SongPlaying + 1 >= maxSongs) ? 0 : SongPlaying + 1;

        audios[SongPlaying].Stop();
        SongPlaying = aux;
        PlaySong();
        UpdateLevel();

    }
    public virtual void PreviusSong()
    {
        if (maxSongs == 0) return;
        
        int aux = (SongPlaying - 1 < 0) ? maxSongs - 1 : SongPlaying - 1;
        audios[SongPlaying].Stop();
        SongPlaying = aux;
        PlaySong();
        UpdateLevel();

    }
    public virtual void PauseSong()
    {
        if(audios[SongPlaying].isPlaying)
            audios[SongPlaying].Pause(); 
         else
            audios[SongPlaying].Play();
    }
    public virtual void PlaySong()
    {
        Debug.Log(SongPlaying);
        audios[SongPlaying].Play();
    }
    public virtual void UpdateLevel()
    {
        foreach (var go in levels)
            go.SetActive(false);
        levels[SongPlaying].SetActive(true);
    }

    public virtual void UptadeSong(int song)
    {
        audios[SongPlaying].Stop();
        SongPlaying = song;
        audios[song].Play();
    }

    public virtual void TurnUpVolume()
    {
        GameObject.FindWithTag("Phone").GetComponent<PhoneController>().ChangeVolume(modificador);
    }

    public virtual void TurnDownVolume()
    {
        GameObject.FindWithTag("Phone").GetComponent<PhoneController>().ChangeVolume(-modificador);
    }

    public virtual void UpdateVolumen(double volumen)
    {
        for(int i = 0; i < moviles.Length; i++)
        {
            var diferencia = moviles[i].GetComponent<Moving>().diference();
            diferencia = diferencia * (float)(volumen);
            moviles[i].transform.localPosition = moviles[i].GetComponent<Moving>().Start + diferencia;
        }
    }


}
