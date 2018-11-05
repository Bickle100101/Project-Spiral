using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour 
{
    AudioSource audioSource;

    //Microphone input
    public AudioClip audioClip;
    public string selectedDevice;
    public AudioMixerGroup mixerGroupMicrophone;
    public AudioMixerGroup mixerGroupMaster;

    public bool useMicrophone;

    float[] samplesLeft = new float[512];
    float[] samplesRight = new float[512];

    float[] freqBands = new float[8];

    //Band buffer will hold values in order to make a smoother shrink on the visualization bars
    //every time the freqBands value for a given frequency band goes above bandBuffer for that frequency band
    //band buffer is set equal to the value of freqBands for that frequency band
    // if the freqBands Value is lower that bandBuffer, bandBuffer will shrink slowly towards that value
    float[] bandBuffers = new float[8];
    float[] bufferDecreases = new float[8];
    float[] freqBandsHighest = new float[8];

    //Values for Audio 64 bands
    float[] freqBands64 = new float[64];
    float[] bandBuffers64 = new float[64];
    float[] bufferDecreases64 = new float[64];
    float[] freqBandsHighest64 = new float[64];

    [HideInInspector]
    public float[] audioBands;
    [HideInInspector]
    public float[] audioBandBuffers;

    [HideInInspector]
    public float[] audioBands64;
    [HideInInspector]
    public float[] audioBandBuffers64;

    [HideInInspector]
    public float amplitude;
    [HideInInspector]
    public float amplitudeBuffer;

    float amplitudeHighest;
    public float audioProfile;

    public enum Channel { Stereo, Left, Right};
    public Channel channel = new Channel();

    void Start () 
	{
        audioBands = new float[8];
        audioBandBuffers = new float[8];

        audioBands64 = new float[64];
        audioBandBuffers64 = new float[64];

        audioSource = GetComponent<AudioSource>();
        AudioProfile(audioProfile);

        //Microphone Input

        if (useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {

                selectedDevice = Microphone.devices[0].ToString();
                audioSource.outputAudioMixerGroup = mixerGroupMicrophone;
                audioSource.clip = Microphone.Start(selectedDevice, true, 10, AudioSettings.outputSampleRate);
            }
            else
            {
                useMicrophone = false;
            }
        }
        else if (!useMicrophone)
        {
            audioSource.outputAudioMixerGroup = mixerGroupMaster;
            audioSource.clip = audioClip;
        }

        audioSource.Play();
    }

	void Update () 
	{
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        MakeFrequencyBands64();
        BandBuffer();
        BandBuffer64();
        CreateAudioBands();
        CreateAudioBands64();
        GetAmplitude();
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < freqBands.Length; i++)
        {
            freqBandsHighest[i] = 0;
        }
    }

    void GetAmplitude()
    {
        float currentAmplitude = 0f;
        float currentAmplitudeBuffer = 0f;

        for (int i = 0; i < audioBands.Length; i++)
        {
            currentAmplitude += audioBands[i];
            currentAmplitudeBuffer += audioBandBuffers[i];
        }

        if (currentAmplitude > amplitudeHighest)
        {
            amplitudeHighest = currentAmplitude;
        }
        amplitude = currentAmplitude/amplitudeHighest;
        amplitudeBuffer = currentAmplitudeBuffer/ amplitudeHighest;
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < freqBands.Length; i++)
        {
            if (freqBands[i] > freqBandsHighest[i])
            {
                freqBandsHighest[i] = freqBands[i];
            }
            audioBands[i] = (freqBands[i] /freqBandsHighest[i]);
            audioBandBuffers[i] = (bandBuffers[i] / freqBandsHighest[i]);
    }
    }

    void CreateAudioBands64()
    {
        for (int i = 0; i < freqBands64.Length; i++)
        {
            if (freqBands64[i] > freqBandsHighest64[i])
            {
                freqBandsHighest64[i] = freqBands64[i];
            }
            audioBands64[i] = (freqBands64[i] / freqBandsHighest64[i]);
            audioBandBuffers64[i] = (bandBuffers64[i] / freqBandsHighest64[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samplesLeft,0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(samplesRight, 1, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int g = 0; g < bufferDecreases.Length; g++)
        {
            if ((freqBands[g] > bandBuffers [g]))
            {
                bandBuffers[g] = freqBands[g];
                bufferDecreases[g] = 0.005f;
            }
            if ((freqBands[g] < bandBuffers[g]))
            {
                bandBuffers[g] -= bufferDecreases[g];
                bufferDecreases[g] *= 1.2f;
            }
        }
    }

    void BandBuffer64()
    {
        for (int g = 0; g < bufferDecreases64.Length; g++)
        {
            if ((freqBands64[g] > bandBuffers64[g]))
            {
                bandBuffers64[g] = freqBands64[g];
                bufferDecreases64[g] = 0.005f;
            }
            if ((freqBands64[g] < bandBuffers64[g]))
            {
                bandBuffers64[g] -= bufferDecreases64[g];
                bufferDecreases64[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        //to determine the number of frequency Bands
        //devide the Hz of the song by the number of instantiated objects in the visualizer
        //  example: 22050Hz / 512 Cubes = 43Hz per sample

        //Cubes then need to be divided into channels equal to the number of Bands you are using
        //Here are the Standard Bands
        //    20-60 Hertz
        //    60-250 Hertz
        //    250-500 Hertz
        //    500-2000 Hertz
        //    2000-4000 Hertz
        //    4000-6000 Hertz
        //    6000-20000 Hertz
        //We will need one more band to make 8 so these will need to be divided up and some numbers multiplied by the 43Hz number derived above
        //In these, the low end will be 1+ the high end of the Previous Number
        //    Band 0: 2 * 43Hz = 86 Hertz (1Hz - 86Hz)
        //    Band 1: 4 * 43Hz = 172 Hertz (87Hz - 258Hz) 
        //    Band 2: 8 * 43Hz = 344 Hertz (259Hz - 602Hz)
        //    Band 3: 16 * 43Hz = 688 Hertz (603Hz - 1290Hz)
        //    Band 4: 32 * 43Hz = 1376 Hertz (1291Hz - 2666Hz)
        //    Band 5: 64 * 43Hz = 2752 Hertz (2667Hz - 5418Hz)
        //    Band 6: 128 * 43Hz = 5504 Hertz (5419Hz - 10922Hz)
        //    Band 7: 256 * 43Hz = 11008 Hertz (10923Hz - 21930Hz)

        //    This Gives us 510 out of 512 Instantiated objects
        //    we can add 2 more at the end

        //Nested Loop Specifies How Many Samples go in each Band
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {

                if (channel == Channel.Stereo)
                {
                    average += samplesLeft[count] + samplesRight[count] * (count + 1);
                    count++;
                }
                else if (channel == Channel.Left)
                {
                    average += samplesLeft[count] * (count + 1);
                    count++;
                }
                else if (channel == Channel.Right)
                {
                    average += samplesRight[count] * (count + 1);
                    count++;
                }
            }

            average /= count;
            freqBands[i] = average * 10;
        }
    }

    void MakeFrequencyBands64()
    {

            //0-15 = 1 sample =     16
            //16-31 = 2 samples =   32
            //32-39 = 4 samples =   32
            //40-47 = 6 samples =   48
            //48-55 = 16 samples = 128
            //56-63 = 32 samples = 256
            //                     ---
            //                     512 samples

        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for (int i = 0; i < freqBands64.Length; i++)
        {
            float average = 0;

            if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);

                if (power == 3)
                {
                    sampleCount -= 2;   //because the 40-47 band has 6 samples instead of 8 as it would if it had a true power relationship
                }

            }
            for (int j = 0; j < sampleCount; j++)
            {

                if (channel == Channel.Stereo)
                {
                    average += samplesLeft[count] + samplesRight[count] * (count + 1);
                    count++;
                }
                else if (channel == Channel.Left)
                {
                    average += samplesLeft[count] * (count + 1);
                    count++;
                }
                else if (channel == Channel.Right)
                {
                    average += samplesRight[count] * (count + 1);
                    count++;
                }
            }

            average /= count;
            freqBands64[i] = average * 8;

        }
    }
}