using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Face", menuName = "Custom/New Face")]
public class Face : ScriptableObject 
{

    #region FaceDescription
    public string faceName;
    public string job;
    public Sprite artwork;
    #endregion

    #region FaceAttributes
    [Range(-1, 1)]
    public float agression;
    [Range(-1, 1)]
    public float ambition;
    [Range(-1, 1)]
    public float integrity;
    [Range(-1, 1)]
    public float introspection;
    [Range(-1, 1)]
    public float leadership;
    [Range(-1, 1)]
    public float luck;
    public bool isAlive;

    #endregion

}
