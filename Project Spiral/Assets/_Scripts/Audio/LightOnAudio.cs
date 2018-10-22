using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightOnAudio : MonoBehaviour 
{
    public int band;
    public float minIntensity;
    public float maxIntensity;
    Light audioLight;

	void Start () 
	{
        audioLight = GetComponent<Light>();
	}

	void Update () 
	{
		audioLight.intensity = (AudioPeer.audioBandBuffers[band] * (maxIntensity - minIntensity)) + minIntensity;
	}
}