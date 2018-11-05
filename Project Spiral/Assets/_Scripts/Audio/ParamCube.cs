using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParamCube : MonoBehaviour 
{
    public AudioPeer audioPeer;
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
            transform.localScale = new Vector3(transform.localScale.x, (audioPeer.audioBandBuffers[band] * maxScale) + startScale, transform.localScale.z);
            Color color = new Color(audioPeer.audioBandBuffers[band], audioPeer.audioBandBuffers[band], audioPeer.audioBandBuffers[band]);
            material.SetColor("EmissionColor", color* audioPeer.audioBands[0]);
        }
        if (!useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (audioPeer.audioBands[band] * maxScale) + startScale, transform.localScale.z);
            Color color = new Color(audioPeer.audioBands[band], audioPeer.audioBands[band], audioPeer.audioBands[band]);
            material.SetColor("EmissionColor", color * audioPeer.audioBands[0]);
        }

    }
	
}