using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// I am not even going to pretend to understand this math.
/// This came from PeerPlay's Procedural Phyllotaxis Series
///     https://youtu.be/YCFt0L5KNWE
/// </summary>
public class Phyllotaxis: MonoBehaviour {

    //public GameObject dot;

    //  angle: = number * degree
    //  radius = scale * sqrRoot(Number)

    public float degree;        //Degree
    public float scale;         //Scale
    public int numberStart;          //Number
    public int stepSize;
    public int maxIteration;

    //Lerping Values
    public bool useLerping;
    public float intervalLerp;
    private bool isLerping;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float timeStartedLerping;


    private int currentIteration;
    private int number;

    private Vector2 phyllotaxisPosition;

    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        number = numberStart;

        transform.localPosition = CalculatePhyllotaxis(degree, scale, number);

        if (useLerping)
        {
            StartLerping();
        }
    }

    private void FixedUpdate()
    {
        if (useLerping)
        {
            if (isLerping)
            {
                float timeSinceStarted = Time.time - timeStartedLerping;
                float percentageComplete = timeSinceStarted / intervalLerp;
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, percentageComplete);

                if (percentageComplete >= 0.97f)
                {
                    transform.localPosition = endPosition;
                    number += stepSize;
                    currentIteration++;
                    if (currentIteration <= maxIteration)
                    {
                        StartLerping();
                    }
                    else
                    {
                        isLerping = false;
                    }
                }
            }
        }
        else
        {
            phyllotaxisPosition = CalculatePhyllotaxis(degree, scale, number);
            transform.localPosition = new Vector3(phyllotaxisPosition.x, phyllotaxisPosition.y, 0);

            number += stepSize;
            currentIteration++;
        }
    }

    private Vector2 CalculatePhyllotaxis(float degree, float scale, int number)
    {
        //Using the Double type because we need a higher degree of precision than a standard float
        //Degree must be in radians so we are using the Mathf function Deg2Rad
        double angle = number * (degree * Mathf.Deg2Rad);

        float r = scale * Mathf.Sqrt(number);

        float xCoordinate = r * (float)System.Math.Cos(angle);
        float yCoordinate = r * (float)System.Math.Sin(angle);

        Vector2 vec2 = new Vector2(xCoordinate, yCoordinate);

        return vec2;
    }

    private void StartLerping()
    {
        isLerping = true;
        timeStartedLerping = Time.time;
        phyllotaxisPosition = CalculatePhyllotaxis(degree, scale, number);
        startPosition = this.transform.localPosition;
        endPosition = new Vector3(phyllotaxisPosition.x, phyllotaxisPosition.y, 0);
    }
}
