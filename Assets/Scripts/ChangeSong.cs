using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSong : MonoBehaviour
{

    public AudioSource[] audios;
    public GameObject[] levels;
    public GameObject[] moviles;
    public GameObject[] pausableMoviles;
    public int SongPlaying;
    public int maxSongs;
    public float modificador;
    [Range(0.0f, 1.0f)]
    public float posPausable;
    public bool downPausable;
    public bool repiting;

    public virtual void Init()
    {
        audios[SongPlaying].Play();
        UpdateVolumen(audios[0].volume);
        repiting = false;
        if (pausableMoviles.Length > 0 && !repiting)
            InvokeRepeating("UpdatePausablePlataform", 0, 0.03f);
        posPausable = 0;
    }


    public virtual void NextSong()
    {
        if (maxSongs == 0) return;

        int aux = (SongPlaying + 1 > maxSongs) ? 0 : SongPlaying + 1;

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
        if (audios[SongPlaying].isPlaying)
        {
            audios[SongPlaying].Pause();
            if (pausableMoviles.Length > 0)
            {
                CancelInvoke("UpdatePausablePlataform");
                repiting = false;
            }
             InvokeRepeating("RestartPlay", 2f, 0);
        }
        else
        {
            audios[SongPlaying].Play();
            if (pausableMoviles.Length > 0 && !repiting)
                InvokeRepeating("UpdatePausablePlataform", 0, 0.03f);
        }
    }
    public virtual void RestartPlay()
    {
        if (!audios[SongPlaying].isPlaying)
            audios[SongPlaying].Play();
        if (pausableMoviles.Length > 0 && !repiting)
            InvokeRepeating("UpdatePausablePlataform", 0, 0.03f);
    }
    public virtual void LoadisPlaying(bool p)
    {
        if (p && !repiting)
        {
            InvokeRepeating("UpdatePausablePlataform", 0, 0.03f);
        }
    }
    public virtual void PlaySong()
    {
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
        for (int i = 0; i < moviles.Length; i++)
        {
            var diferencia = moviles[i].GetComponent<Moving>().diference();
            diferencia = diferencia * (float)(volumen);
            moviles[i].GetComponent<Moving>().Move(diferencia);
        }
    }

    public virtual void UpdatePausablePlataform()
    {
        repiting = true;
        if (posPausable >= 1)
            downPausable = true;
        if (posPausable <= 0)
            downPausable = false;

        posPausable = (downPausable) ? posPausable - modificador * 0.5f : posPausable + modificador * 0.5f;

        for (int i = 0; i < pausableMoviles.Length; i++)
        {
            var diferencia = pausableMoviles[i].GetComponent<Moving>().diference();
            diferencia = diferencia * (float)(posPausable);
            pausableMoviles[i].GetComponent<Moving>().Move(diferencia);
        }
    }

}


