using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSong : MonoBehaviour {
    
    public AudioSource[] audios;
    public GameObject[] levels;
    public int SongPlaying;
    public int maxSongs;
    
    public virtual void Init()
    {
        audios[SongPlaying].Play(0);
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
        audios[SongPlaying].Play(0);
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


}
