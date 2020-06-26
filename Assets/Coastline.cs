using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.WSA;
using System.CodeDom.Compiler;

public class Coastline : MonoBehaviour
{
    public boat boat;
    public string myString;
    public Text myText;
    public float fadeTime;
    public bool displayInfo;

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
        if (displayInfo)
        {
            if (Input.GetKeyDown("x"))
            {
                boat.riding = false;
            }

        }
    }

    void OnTriggerEnter()
    {
        if (boat.riding){
            displayInfo = true;

        }
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
