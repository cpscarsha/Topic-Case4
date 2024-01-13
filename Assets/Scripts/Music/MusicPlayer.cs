using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource g_self_audio_source;
    public AudioClip[] g_music;
    public int g_playing_music_idx = 0;
    // Start is called before the first frame update
    void Start()
    {
        g_self_audio_source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private int g_last_idx = 0;
    void Update()
    {
        g_self_audio_source.clip = g_music[g_playing_music_idx];
        if(g_last_idx != g_playing_music_idx)Play();
        g_last_idx = g_playing_music_idx;
    }

    public void Play(){
        g_self_audio_source.Play();
    }
    public void Stop(){
        g_self_audio_source.Pause();
    }
}
