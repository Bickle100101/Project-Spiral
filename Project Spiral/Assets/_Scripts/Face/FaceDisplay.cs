using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceDisplay : MonoBehaviour
{
    public Face face;

    public Text faceNameText;
    public Text jobText;
    public Image artImage;

 
    public Text aggressionText;
    public Text ambitionText;
    public Text integrityText;
    public Text introspectionText;
    public Text leadershipText;
    public Text luckText;
    public bool isAlive;

    void Start()
    {
        faceNameText.text = ("Name\n\n" + face.faceName);
        jobText.text = ("Job\n\n" + face.job);
        artImage.sprite = face.artwork;

        aggressionText.text = ("Agression\n\n" + face.agression.ToString());
        ambitionText.text = ("Ambition\n\n" + face.ambition.ToString());
        integrityText.text = ("Integrity\n\n" + face.integrity.ToString());
        introspectionText.text = ("Introspection\n\n" + face.introspection.ToString());
        leadershipText.text = ("Leadership\n\n" + face.leadership.ToString());
        luckText.text = ("Luck\n\n" + face.luck.ToString());
        isAlive = face.isAlive = true;
    }
}

