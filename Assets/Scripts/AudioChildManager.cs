using UnityEngine;
using System;

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
