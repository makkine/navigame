using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.WSA;
using System.CodeDom.Compiler;

public class boat : MonoBehaviour
{
    public PController rider;
    public string myString;
    public Text myText;
    public float fadeTime;
    public bool displayInfo;

    public bool riding;
    
    // Start is called before the first frame update
    void Start()
    {
        myText = GameObject.Find("Text").GetComponent<Text>();
        myText.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        FadeText();
        if (riding)
        {
            Vector3 tempTransform = rider.transform.position;
            tempTransform.y = -1.3f;
            Quaternion tempRot = rider.transform.rotation;
            this.transform.rotation = tempRot;
            this.transform.position = tempTransform;
        }
        if (displayInfo)
        {
            if (Input.GetKeyDown("x"))
            {
                rider.transform.position = this.transform.position;
                riding = true;
                displayInfo = false;
            }

        }
    }

    void OnTriggerEnter()
    {
        displayInfo = true;
    }

    void OnTriggerExit()
    {
        displayInfo = false;
    }

    void FadeText()

    {
        if (displayInfo)
        {
            myText.text = myString;
            myText.color = Color.Lerp(myText.color, Color.white, fadeTime * Time.deltaTime);

        }

        else
        {
            myText.color = Color.Lerp(myText.color, Color.clear, fadeTime * Time.deltaTime);
        }

    }
}
