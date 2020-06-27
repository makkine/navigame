using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.XR.WSA;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

public interface INPCMessageTarget : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void RespondToDialogue(string s);
}

public class NPC : MonoBehaviour, INPCMessageTarget
{
    public bool talking;
    public TextAsset dialogues;
    public CameraSettings camera; // Reference to the camera, to allow freezing/unfreezing

    // For entry into NPC cmesh
    public string enterString;
    public Text enterText;
    public float fadeTime;
    public bool displayInfo;

    // Text stuff for dialogue tree UI
    protected XmlElement currDialogueNode;
    protected string currDialogue = "";
    public Text diaText;
    public float diaFadeTime;
    public Option option1;
    public Option option2;
    protected List<Option> options;

    // When in dialogue tree
    protected XmlDocument dialogueRoot;

    // Start is called before the first frame update
    void Start()
    {
        enterText.color = Color.clear;
        diaText.color = Color.clear;
        options = new List<Option>();
        options.Add(option1);
        options.Add(option2);
        for (int i = 0; i < options.Count; i++)
        {
            options[i].text.color = Color.clear;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageText();
        if (displayInfo)
        {
            if (Input.GetKeyDown("x"))
            {
                talking = true;
                displayInfo = false;
                ExecuteEvents.Execute<CameraMessageTarget>(camera.gameObject, null, (x, y) => x.FreezeInput());
                LoadText();
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
        if (talking)
        {
            StopTalking();
        }
    }

    void ManageText()

    {
        if (displayInfo)
        {
            enterText.text = enterString;
            enterText.color = Color.white;
        }

        else
        {
            enterText.color = Color.Lerp(enterText.color, Color.clear, fadeTime * Time.deltaTime);
        }

    }

    // My thinking is that the dialogue screen and options should be shared by all NPCs, 
    // and LoadText sets the appropriate values to the NPC currently being interacted with
    void LoadText()
    {
        dialogueRoot = new XmlDocument();
        dialogueRoot.LoadXml(dialogues.ToString());
        currDialogueNode = (XmlElement)dialogueRoot.GetElementsByTagName("dialogues")[0].FirstChild;

        // Now display the first text + options
        diaText.color = Color.white;
        for (int i = 0; i < options.Count; i++)
        {
            options[i].curr_NPC = this.gameObject;
        }
        DisplayCurrentDialogue();
    }

    // Display current dialogue node 
    void DisplayCurrentDialogue()
    {
        currDialogue = currDialogueNode["text"].InnerText;
        diaText.text = currDialogue;
        XmlNodeList nextOptions;
        XmlElement OptionsInTree = currDialogueNode["options"];
        if (OptionsInTree != null)
        {
            nextOptions = OptionsInTree.ChildNodes;
            for (int i = 0; i < nextOptions.Count; i++)
            {
                options[i].text.color = Color.white;
                options[i].text.text = nextOptions[i]["text"].InnerText;
                options[i].onClick = nextOptions[i]["goto"].InnerText;
                options[i].clickable = true;
            }
            for (int j = nextOptions.Count; j < options.Count; j++)
            {
                options[j].text.color = Color.clear;
                options[j].clickable = false;
            }
        } 
        else
        {
            for (int j = 0; j < options.Count; j++)
            {
                options[j].text.color = Color.clear;
                options[j].clickable = false;
            }
        }
    }

    void StopTalking()
    {
        diaText.color = Color.clear;
        for (int i = 0; i < options.Count; i++)
        {
            options[i].text.color = Color.clear;
            options[i].curr_NPC = null;
            options[i].clickable = false;
        }
        ExecuteEvents.Execute<CameraMessageTarget>(camera.gameObject, null, (x, y) => x.UnfreezeInput());
        talking = false;
    }

    // When the PC selects a dialogue option, change current dialogue + options
    public void RespondToDialogue(string NextDialogue)
    {
        if (NextDialogue == "EXIT")
        {
            StopTalking();
        }
        else
        {
            currDialogueNode = dialogueRoot.GetElementById(NextDialogue);
            DisplayCurrentDialogue();
        }
    }
}
