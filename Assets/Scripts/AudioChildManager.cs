using UnityEngine;
using System;

//Name: Sam Johnson
//File: AudioChildManager.cs
//Purpose: holds data for the audiomanager to use and is placed on objects with a sound source

public class AudioChildManager : MonoBehaviour
{
    public AudioSource source;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }

    public void playSound(){
        try{
            source.PlayOneShot(source.clip);
        } catch(Exception E){
            source.Play();
        }
    }
}
