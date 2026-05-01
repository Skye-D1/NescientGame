/*
* Name: AudioChildManager.cs
* Author: Sam Johnson
* Email: samuel.johnson
* Desc: holds data for the audiomanager to use and is placed on objects with a sound source
*/

using UnityEngine;
using System;

public class AudioChildManager : MonoBehaviour
{
    public AudioSource source;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        source = gameObject.GetComponent<AudioSource>();
    }

    /*
    Name: playSound
    Desc: Plays the sound in the audio source. uses PlayOneShot if it is an AudioClip or Play if it isn't.
    */
    public void playSound(){
        if(source.clip != null){
            source.PlayOneShot(source.clip);
        } else{
            source.Play();
        }
    }
}
