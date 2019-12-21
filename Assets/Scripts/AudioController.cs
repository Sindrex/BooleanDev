using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {

    public AudioMixer audioMixer;
    public AudioSource SFXSource;
    public AudioSource musicSource;
    public AudioClip[] musicTracks;
    private int currentSong = -1;

    // Use this for initialization
    void Start () {
        //Keep only 1 audiomixer
        GameObject dupeMixer = GameObject.Find("AudioMixer");
        if(dupeMixer != null && dupeMixer != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        loadAudioPref();

        //keep this between scenes
        DontDestroyOnLoad(this.gameObject);
    }

    //Loads audiosettings from playerprefs
    public void loadAudioPref()
    {
        float master = PlayerPrefs.GetFloat(OptionController.audioOK[0]);
        float music = PlayerPrefs.GetFloat(OptionController.audioOK[1]);
        float SFX = PlayerPrefs.GetFloat(OptionController.audioOK[2]);

        audioMixer.SetFloat("masterVolume", AudioOptionsController.volFunction(master));
        audioMixer.SetFloat("musicVolume", AudioOptionsController.volFunction(music));
        audioMixer.SetFloat("SFXVolume", AudioOptionsController.volFunction(SFX));
    }

    //plays SFX clip
    public void playTilePlacedSFX()
    {
        SFXSource.Play();
    }

    //1 per frame
    public void Update()
    {
        //Switch tracks when other is done
        if (!musicSource.isPlaying)
        {
            currentSong++;
            if(currentSong >= musicTracks.Length || currentSong < 0)
            {
                currentSong = 0;
            }
            musicSource.clip = musicTracks[currentSong];
            musicSource.Play();
        }
    }
}
