using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {

    public AudioMixer audioMixer;
    public AudioSource SFXSource;

    // Use this for initialization
    void Start () {
        GameObject dupeMixer = GameObject.Find("AudioMixer");
        if(dupeMixer != null && dupeMixer != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        loadAudioPref();

        //playTilePlacedSFX();
        DontDestroyOnLoad(this.gameObject);
    }

    public void loadAudioPref()
    {
        float master = PlayerPrefs.GetFloat(OptionController.audioOK[0]);
        float music = PlayerPrefs.GetFloat(OptionController.audioOK[1]);
        float SFX = PlayerPrefs.GetFloat(OptionController.audioOK[2]);

        audioMixer.SetFloat("masterVolume", AudioOptionsController.volFunction(master));
        audioMixer.SetFloat("musicVolume", AudioOptionsController.volFunction(music));
        audioMixer.SetFloat("SFXVolume", AudioOptionsController.volFunction(SFX));
    }

    public void playTilePlacedSFX()
    {
        SFXSource.Play();
    }
}
