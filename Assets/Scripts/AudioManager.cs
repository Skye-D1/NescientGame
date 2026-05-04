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
    int counter = 0;
    float bpm = 145f;
    public int sounds;
    public float[] loopLengths; //length of loop in beats
    int[] beatsIn; //how many beats into the clip it is
    bool[] isPlaying;
    bool[] isLooping;
    float timeSinceLastBeat = 0f;
    public int[] soundToPlayAfter;
    float lastTime = 5f;
    bool[] queue;
    bool firstBeat = true;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        beatsIn = new int[sounds];
        isPlaying = new bool[sounds];
        isLooping = new bool[sounds];
        queue = new bool[5];

        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach(AudioSource source in sources){
            source.Stop();
        }

        isLooping[28] = true;
        soundToPlayAfter[1] = 28;

        PlayOnBeat(0);

    }

    // Update is called once per frame
    void Update()
    {

        if(Time.realtimeSinceStartup >= 5f){
            // manages when a beat happens
            timeSinceLastBeat += Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;
            if(timeSinceLastBeat > 60f/bpm){
                timeSinceLastBeat -= 60f/bpm;

                if(firstBeat){
                    PlayClip(26, false);
                    firstBeat = false;
                }

                //Beat has happened! for each sound, do what needs to happen that beat (loop, reset, play next sound, etc)
                Debug.Log("Beat");
                for(int i = 0; i < 5; i++){
                    if(queue[i]){
                        if(i == 0){
                            PlayClip(0, true);
                            queue[0] = false;
                        } else if(i == 1 && beatsIn[0] == 111){
                            PlayClip(1, false);
                            queue[1] = false;
                        } else if(i == 2){
                            PlayClip(26, false);
                            queue[2] = false;
                        } else if(i == 3){
                            PlayClip(27, true);
                            queue[3] = false;
                        } else if(i == 4){
                            PlayClip(28, true);
                            queue[4] = false;
                        }
                    }
                }

                for(int i = 0; i < sounds; i++){
                    if(isPlaying[i]){
                        beatsIn[i] += 1;
                        if(beatsIn[i] >= loopLengths[i] && isLooping[i]){
                            PlayClip(i, true);
                        } else if(beatsIn[i] >= loopLengths[i] && soundToPlayAfter[i] != -1){
                            Debug.Log("sound to play after attempt: " + i);
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

    public void PlayOnBeat(int index){
        if(index == 0){
            queue[0] = true;
        } else if(index == 1){
            queue[1] = true;
        } else if(index == 26){
            queue[2] = true;
        } else if(index == 27){
            queue[3] = true;
        } else if(index == 28){
            queue[4] = true;
        }
    }
}
