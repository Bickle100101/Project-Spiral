using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightOnAudio : MonoBehaviour 
{
    public AudioPeer audioPeer;
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
		audioLight.intensity = (audioPeer.audioBandBuffers[band] * (maxIntensity - minIntensity)) + minIntensity;
	}
}