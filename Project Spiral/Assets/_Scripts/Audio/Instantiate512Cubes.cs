using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Instantiate512Cubes : MonoBehaviour 
{
    public GameObject sampleCubePrefab;

    GameObject[] sampleCubes = new GameObject[512];

    public float maxScale;

	void Start () 
	{
        for (int i = 0; i < sampleCubes.Length; i++)
        {
            GameObject instanceSampleCube = (GameObject)Instantiate(sampleCubePrefab);
            instanceSampleCube.transform.position = this.transform.position;
            instanceSampleCube.transform.parent = this.transform;
            instanceSampleCube.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);
            instanceSampleCube.transform.position = Vector3.forward * 100;
            sampleCubes[i] = instanceSampleCube;
        }
	}

	void Update () 
	{
        for (int i = 0; i < sampleCubes.Length; i++)
        {
            if (sampleCubes != null)
            {
                sampleCubes[i].transform.localScale = new Vector3(10, (AudioPeer.samples[i] * maxScale) + 2, 10);
            }
        }
	}
}