using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Phyllotaxis, the regular arrangement of leaves or flowers around a plant stem, is an example of developmental
/// pattern formation and organogenesis. Phyllotaxis is characterized by the divergence angles between the
/// organs, the most common angle being 137.5-, the golden angle.The quantitative aspects of phyllotaxis have stimulated
/// research at the interface between molecular biology, physics and mathematics.This review documents
/// the rich history of different approaches and conflicting hypotheses, and then focuses on recent molecular
/// work that establishes a novel patterning mechanism based on active transport of the plant hormone auxin.
/// Finally, it shows how computer simulations can help to formulate quantitative models that in turn can be tested
/// by experiment. The accumulation of ever increasing amounts of experimental data makes quantitative modeling
/// of interest for many developmental systems.
///                 --Phyllotaxis: Cris Kuhlemeier
///                 --Institute of Plant Sciences, University of Bern, CH-3013 Bern, Switzerland
///                 --http://www.botany.unibe.ch/deve/publications/reprint/TrendsPlantSci_12_143.pdf
/// 
/// This came from PeerPlay's Procedural Phyllotaxis Series
///     https://youtu.be/YCFt0L5KNWE
///     
/// Vogels Rule:  A planar model of Phyllotaxis
///  
///     n = ordering number of a seed counting outward from the center in a polar coordinate system
///     phi = n * angle in degrees (ex: 137.5 Degrees)
///     c = center (a Scaling Parameter)
/// 
///     angle: phi = n * angle in degrees (ex: 137.5 Degrees)
///     radius: r = c * sqrRoot(n)
///  
///     The numbers above are for a polar coordinate system.  We need the information in cartesian coordinate system (x,y)
///     polar->cartesian conversion
///         x axis value = r * cos(angle)
///         y axis value = r * sin(angle)
///         
///     Note:  137.5 is a special degree.  When plotted, this degree is the only one tha keeps all dots perfectly aligned to eachother
/// </summary>
/// 
public class Phyllotaxis: MonoBehaviour
{

    #region Properties


    public AudioPeer audioPeer;
    private Material trailMaterial;
    public Color trailColor;

    public float degree, scale;       
    public int numberStart;
    public int stepSize;
    public int maxIteration;

    //Lerping Values
    public bool useLerping;
    private bool isLerping;
    private Vector3 startPosition, endPosition;
    private float lerpPosTimer, lerpPosSpeed;
    public Vector2 lerpPosSpeedMinMax;
    public AnimationCurve lerpPosAnimCurve;
    public int lerpPosBand;

    private int number;
    private int currentIteration;
    private TrailRenderer trailRenderer;
    
    
    //Used to assign the position we have found for each dot
    private Vector2 phyllotaxisPosition;

    private bool forward;
    public bool repeat, invert;

    //Scaling
    public bool useScaleAnimation, useScaleCurve;
    public Vector2 scaleAnimMinMax;
    public AnimationCurve scaleAnimCurve;
    public float scaleAnimSpeed;
    public int scaleBand;
    private float scaleTimer, currentScale;

    #endregion

    private void Awake()
    {
        currentScale = scale;
        forward = true;
        trailRenderer = GetComponent<TrailRenderer>();
        trailMaterial = new Material(trailRenderer.material);
        trailMaterial.SetColor("_TintColor", trailColor);
        trailRenderer.material = trailMaterial;

        number = numberStart;
        transform.localPosition = CalculatePhyllotaxis(degree, currentScale, number);

        if (useLerping)
        {
            isLerping = true;
            SetLerpPositions();
        }
    }

    private void Update()
    {

        if (useScaleAnimation)
        {
            if (useScaleCurve)
            {
                scaleTimer += (scaleAnimSpeed * audioPeer.audioBands[scaleBand]) * Time.deltaTime;
                if (scaleTimer >= 1)
                {
                    scaleTimer -= 1;
                }

                currentScale = Mathf.Lerp(scaleAnimMinMax.x, scaleAnimMinMax.y, scaleAnimCurve.Evaluate(scaleTimer));
            }
            else
            {
                currentScale = Mathf.Lerp(scaleAnimMinMax.x, scaleAnimMinMax.y, audioPeer.audioBands[scaleBand]);
            }
        }

        if (useLerping)
        {
            if (isLerping)
            {
                lerpPosSpeed = Mathf.Lerp(lerpPosSpeedMinMax.x, lerpPosSpeedMinMax.y, lerpPosAnimCurve.Evaluate(audioPeer.audioBands[lerpPosBand]));
                lerpPosTimer += Time.deltaTime * lerpPosSpeed;
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01(lerpPosTimer));
                if (lerpPosTimer >= 1)
                {
                    lerpPosTimer -= 1;

                    if (forward)
                    {
                        number += stepSize;
                        currentIteration++;
                    }
                    if (!forward)
                    {
                        number -= stepSize;
                        currentIteration--;
                    }

                    if ((currentIteration > 0) && (currentIteration < maxIteration))
                    {
                        SetLerpPositions();
                    }
                    else //current iteration has hit 0 or maxiteration
                    {
                        if (repeat)
                        {
                            if (invert)
                            {
                                forward = !forward;
                                SetLerpPositions();
                            }
                            else
                            {
                                number = numberStart;
                                currentIteration = 0;
                                SetLerpPositions();
                            }
                        }
                        else
                        {
                            isLerping = false;
                        }
                    }
                }
            }
        }
        if (!useLerping)
        {
            phyllotaxisPosition = CalculatePhyllotaxis(degree, currentScale, number);
            transform.localPosition = new Vector3(phyllotaxisPosition.x, phyllotaxisPosition.y, 0);
            number += stepSize;
            currentIteration++;
        }

    }

    private Vector2 CalculatePhyllotaxis(float degree, float scale, int number)
    {
        //Using the Double type for angle because float only allows us a 23-bit precision
        //this needs a higher degree of precision than a standard float.  this can be converted back to a float later when needed
        //
        //Degree must be in radians so we are using the Mathf function Deg2Rad
        double angle = number * (degree * Mathf.Deg2Rad);
        float r = scale * Mathf.Sqrt(number);

        //using system.math instead of Mathf because Mathf does not handle doubles
        float xCoordinate = r * (float)System.Math.Cos(angle);
        float yCoordinate = r * (float)System.Math.Sin(angle);

        Vector2 vec2 = new Vector2(xCoordinate, yCoordinate);
        return vec2;
    }

    private void SetLerpPositions()
    {
        phyllotaxisPosition = CalculatePhyllotaxis(degree, currentScale, number);
        startPosition = this.transform.localPosition;
        endPosition = new Vector3(phyllotaxisPosition.x, phyllotaxisPosition.y, 0);
    }
}
