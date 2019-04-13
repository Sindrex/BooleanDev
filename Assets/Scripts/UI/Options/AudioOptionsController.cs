using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioOptionsController : MonoBehaviour {

    public AudioController audioCon;
    public AudioMixer audioMixer;

    //for getting when we save
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider SFXSlider;

    public void Start()
    {
        audioCon = GameObject.Find("AudioMixer").GetComponent<AudioController>();
        print(audioCon);
        audioMixer = audioCon.audioMixer;
    }

    public void setMasterVol(float vol)
    {
        audioMixer.SetFloat("masterVolume", volFunction(vol));
    }
    public void setMusicVol(float vol)
    {
        audioMixer.SetFloat("musicVolume", volFunction(vol));
    }
    public void setSFXVol(float vol)
    {
        audioMixer.SetFloat("SFXVolume", volFunction(vol));
    }

    public void saveAudioSettings()
    {
        PlayerPrefs.SetFloat(OptionController.audioOK[0], getMaster());
        PlayerPrefs.SetFloat(OptionController.audioOK[1], getMusic());
        PlayerPrefs.SetFloat(OptionController.audioOK[2], getSFX());
    }

    public void loadAudioSettingsUI()
    {
        float master = PlayerPrefs.GetFloat(OptionController.audioOK[0]);
        float music = PlayerPrefs.GetFloat(OptionController.audioOK[1]);
        float SFX = PlayerPrefs.GetFloat(OptionController.audioOK[2]);

        masterSlider.value = master;
        musicSlider.value = music;
        SFXSlider.value = SFX;

        setMasterVol(master);
        setMusicVol(music);
        setSFXVol(SFX);
    }

    public float getMaster()
    {
        return masterSlider.value;
    }
    public float getMusic()
    {
        return musicSlider.value;
    }
    public float getSFX()
    {
        return SFXSlider.value;
    }

    public static float volFunction(float x)
    {
        if(x >= 0)
        {
            return 0;
        }
        return -Mathf.Pow(1.0565f, -x);
    }
}
