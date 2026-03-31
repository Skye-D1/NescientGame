using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource source;
    float bpm = 145f;
    public AudioClip[] sounds;
    public float[] loopLengths; //length of loop in beats
    int[] beatsIn; //how many beats into the clip it is
    bool[] isPlaying;
    bool[] isLooping;
    float timeSinceLastBeat = 0f;
    public int[] soundToPlayAfter;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
        beatsIn = new int[sounds.Length];
        isPlaying = new bool[sounds.Length];
        isLooping = new bool[sounds.Length];
        isPlaying[0] = true;
        isLooping[0] = true;
        loopLengths[0] = sounds[0].length;
        PlayClip(0, true);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastBeat += Time.deltaTime;
        if(timeSinceLastBeat > bpm/60f){
            timeSinceLastBeat = 0;

            //BEAT
            for(int i = 0; i < sounds.Length; i++){
                if(isPlaying[i]){
                    beatsIn[i] += 1;
                    if(beatsIn[i] >= loopLengths[i] && isLooping[i]){
                        source.PlayOneShot(sounds[i]);
                        beatsIn[i] = 0;
                    } else if(beatsIn[i] >= loopLengths[i] && soundToPlayAfter[i] != -1){
                        beatsIn[i] = 0;
                        isPlaying[i] = false;

                        source.PlayOneShot(sounds[soundToPlayAfter[i]]);
                        isPlaying[soundToPlayAfter[i]] = true;
                        beatsIn[soundToPlayAfter[i]] = 0;
                    }else if(beatsIn[i] >= loopLengths[i]){
                        beatsIn[i] = 0;
                        isPlaying[i] = false;
                    }
                }
            }
        }
    }

    public void PlayClip(int index, bool loop){
        source.PlayOneShot(sounds[index]);
        isPlaying[index] = true;
        beatsIn[index] = 0;
        isLooping[index] = loop;
    }

    public void StopLooping(int index){
        isLooping[index] = false;
    }
}
