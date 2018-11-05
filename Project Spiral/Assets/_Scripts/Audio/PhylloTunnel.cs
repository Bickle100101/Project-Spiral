using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhylloTunnel : MonoBehaviour 
{
    public Transform tunnel;
    public AudioPeer audioPeer;

    public float tunnelSpeed;
    public float cameraDistance;

	void Update () 
	{
        tunnel.position = new Vector3(tunnel.position.x, tunnel.position.y,
            tunnel.position.z + (audioPeer.amplitudeBuffer * tunnelSpeed));

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, tunnel.position.z + cameraDistance);

	}
	
}