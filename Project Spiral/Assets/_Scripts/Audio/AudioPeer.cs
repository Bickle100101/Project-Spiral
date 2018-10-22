using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour 
{
    AudioSource audioSource;
    public static float[] samples = new float[512];
    public static float[] freqBands = new float[8];

    //Band buffer will hold values in order to make a smoother shrink on the visualization bars
    //every time the freqBands value for a given frequency band goes above bandBuffer for that frequency band
    //band buffer is set equal to the value of freqBands for that frequency band
    // if the freqBands Value is lower that bandBuffer, bandBuffer will shrink slowly towards that value
    public static float[] bandBuffers = new float[8];
    public static float[] bufferDecreases = new float[8];

    float[] freqBandsHighest = new float[8];
    public static float[] audioBands = new float[8];
    public static float[] audioBandBuffers = new float[8];

    void Start () 
	{
        audioSource = GetComponent<AudioSource>();
	}

	void Update () 
	{
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
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

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples,0, FFTWindow.Blackman);
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
                average += samples[count] * (count + 1);
                count++;
            }

            average /= count;
            freqBands[i] = average * 10;
        }
    }
}