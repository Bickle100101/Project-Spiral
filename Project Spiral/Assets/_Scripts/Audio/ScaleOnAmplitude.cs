using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScaleOnAmplitude : MonoBehaviour 
{
    public AudioPeer audioPeer;
    public float startScale;
    public float maxScale;
    public bool useBuffer;
    Material material;
    public float red;
    public float green;
    public float blue;


	void Start () 
	{
        material = GetComponent<MeshRenderer>().materials[0];
	}

	void Update () 
	{
        if (!useBuffer)
        {
            transform.localScale = new Vector3((audioPeer.amplitude * maxScale) + startScale, (audioPeer.amplitude * maxScale) + startScale, (audioPeer.amplitude * maxScale) + startScale);
            Color color = new Color(red * audioPeer.amplitude, green * audioPeer.amplitude, blue * audioPeer.amplitude);
            material.SetColor("EmissionColor", color);
        }

        if (useBuffer)
        {
            transform.localScale = new Vector3((audioPeer.amplitudeBuffer * maxScale) + startScale, (audioPeer.amplitudeBuffer * maxScale) + startScale, (audioPeer.amplitudeBuffer * maxScale) + startScale);
            Color color = new Color(red * audioPeer.amplitudeBuffer, green * audioPeer.amplitudeBuffer, blue * audioPeer.amplitudeBuffer);
            material.SetColor("EmissionColor", color);
        }
    }
	
}