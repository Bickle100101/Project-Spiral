using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParamCube : MonoBehaviour 
{
    public int band;
    public float startScale;
    public float maxScale;
    public bool useBuffer;
    Material material;

    void Start()
    {
        material = GetComponent<MeshRenderer>().materials[0];
    }

    void Update () 
	{
        if (useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (AudioPeer.audioBandBuffers[band] * maxScale) + startScale, transform.localScale.z);
            Color color = new Color(AudioPeer.audioBandBuffers[band], AudioPeer.audioBandBuffers[band], AudioPeer.audioBandBuffers[band]);
            material.SetColor("EmissionColor", color);
        }
        if (!useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (AudioPeer.audioBands[band] * maxScale) + startScale, transform.localScale.z);
            Color color = new Color(AudioPeer.audioBands[band], AudioPeer.audioBands[band], AudioPeer.audioBands[band]);
            material.SetColor("EmissionColor", color);
        }

    }
	
}