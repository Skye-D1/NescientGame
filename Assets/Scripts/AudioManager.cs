using UnityEngine;
using System;

//Name: Sam Johnson
//File: AudioManager.cs
//Purpose: manage playing audio

public class AudioManager : MonoBehaviour
{
    float bpm = 145f;
    public int sounds;
    public float[] loopLengths; //length of loop in beats
    int[] beatsIn; //how many beats into the clip it is
    bool[] isPlaying;
    bool[] isLooping;
    float timeSinceLastBeat = 0f;
    public int[] soundToPlayAfter;
    float lastTime = 0f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        beatsIn = new int[sounds];
        isPlaying = new bool[sounds];
        isLooping = new bool[sounds];
        isPlaying[0] = true;
        isLooping[0] = true;
        try{
            loopLengths[0] = 108;
        } catch(Exception E){
            Debug.Log("sound in audiosource 0 is not a clip.");
        }
        
        PlayClip(0, true);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastBeat += Time.realtimeSinceStartup - lastTime;
        lastTime = Time.realtimeSinceStartup;
        if(timeSinceLastBeat > bpm/60f){
            timeSinceLastBeat -= bpm/60f;

            //BEAT
            //Debug.Log("Beat");
            for(int i = 0; i < sounds; i++){
                if(isPlaying[i]){
                    beatsIn[i] += 1;
                    if(beatsIn[i] >= loopLengths[i] && isLooping[i]){
                        PlayClip(i, true);
                    } else if(beatsIn[i] >= loopLengths[i] && soundToPlayAfter[i] != -1){
                        beatsIn[i] = 0;
                        isPlaying[i] = false;

                        PlayClip(soundToPlayAfter[i], isLooping[soundToPlayAfter[i]]);
                    }else if(beatsIn[i] >= loopLengths[i]){
                        beatsIn[i] = 0;
                        isPlaying[i] = false;
                    }
                }
            }
        }
    }

    public void PlayClip(int index, bool loop){
        transform.GetChild(index).GetComponent<AudioChildManager>().playSound();
        isPlaying[index] = true;
        beatsIn[index] = 0;
        isLooping[index] = loop;
    }

    public void StopLooping(int index){
        isLooping[index] = false;
    }
}
