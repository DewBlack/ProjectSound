﻿using System.Collections;
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

    public void Update()
    {
        Debug.Log("Esta seleccionada la cancion " + SongPlaying);
        Debug.Log("Es " + audios[SongPlaying].isPlaying + " que este en play");
        Debug.Log("Con un volumen de " + AudioListener.volume);
    }

    public virtual void NextSong()
    {
        int aux = (SongPlaying + 1 >= audios.Length) ? 0 : SongPlaying + 1;
        if (aux < maxSongs)
        {
            audios[SongPlaying].Stop();
            SongPlaying += 1;
            if (audios.Length == SongPlaying)
                SongPlaying = 0;
            PlaySong();
            UpdateLevel();
        }
    }
    public virtual void PreviusSong()
    {
        int aux = (SongPlaying - 1 < 0) ? audios.Length -1 : SongPlaying - 1;
        if (aux < maxSongs)
        {
            audios[SongPlaying].Stop();
            SongPlaying -= 1;
            if (SongPlaying < 0)
                SongPlaying = audios.Length - 1;
            PlaySong();
            UpdateLevel();
        }
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
        Debug.Log(modificador);
        GameObject.FindWithTag("Phone").GetComponent<PhoneController>().ChangeVolume(modificador);
    }

    public virtual void TurnDownVolume()
    {
        Debug.Log(-modificador);
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
