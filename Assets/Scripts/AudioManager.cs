/*
* Name: AudioManager.cs
* Author: Sam Johnson
* Email: samuel.johnson
* Desc: manages playing audio
*/

using UnityEngine;
using System;

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

        //piano 108
        //play piano
        PlayClip(0, true);

        //wind 809
        //play wind tail
        PlayClip(26, false);

    }

    // Update is called once per frame
    void Update()
    {
        // manages when a beat happens
        timeSinceLastBeat += Time.realtimeSinceStartup - lastTime;
        lastTime = Time.realtimeSinceStartup;
        if(timeSinceLastBeat > bpm/60f){
            timeSinceLastBeat -= bpm/60f;

            //Beat has happened! for each sound, do what needs to happen that beat (loop, reset, play next sound, etc)
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

    /*
    Name: playClip
    Inputs: index - index of the sound to play
            loop - whether to loop or not
    Desc: plays the specified clip from the child object that has it
    */
    public void PlayClip(int index, bool loop){
        transform.GetChild(index).GetComponent<AudioChildManager>().playSound();
        isPlaying[index] = true;
        beatsIn[index] = 0;
        isLooping[index] = loop;
    }

    /*
    Name: stopLooping
    Desc: stops a specific sound from looping
    */
    public void StopLooping(int index){
        isLooping[index] = false;
    }
}
