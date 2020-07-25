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
using System.Threading;

public interface INPCMessageTarget : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void RespondToDialogue(int optionIndex, bool bettable, float bet);
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

    // SocialStats scriptable objects for social mechanics
    [SerializeField] SocialStats playerStats;
    [SerializeField] SocialStats npcStats;

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
                options[i].optionText = nextOptions[i]["text"].InnerText;
                options[i].clickable = true;
                options[i].bettable = false;
                options[i].optionIndex = i;
                if (nextOptions[i].ChildNodes.Count == 5)
                {
                    string skill = nextOptions[i]["skill"].InnerText;
                    int difficulty = System.Convert.ToInt32(nextOptions[i]["difficulty"].InnerText);
                    // Bettable if there are multiple paths in dialogue tree and player's relevant stat
                    // is not high enough
                    if ((playerStats.GetStat(skill).x)/100 < difficulty)
                    {
                        options[i].bettable = true;
                        options[i].playerStat = playerStats.GetStat(skill).x;
                    }
                }

                if (options[i].bettable)
                {
                    options[i].bet = 0;
                    options[i].text.text = options[i].optionText + "; Bet: " + options[i].bet.ToString();
                }
                else
                {
                    options[i].text.text = options[i].optionText;
                }

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
    
    // Given a skill area, difficulty, and bet for a question, returns true if the player is successful.
    bool IsSuccessful(string skill, float difficulty, float bet)
    {
        float playerLevel = playerStats.GetStat(skill).x / 100;
        float levelDifference = difficulty - playerLevel;
        float npcContribution;
        if (skill == "curiosity")
        {
            npcContribution = npcStats.GetStat("strength").x + npcStats.GetStat("agility").x;
        }
        else if (skill == "observation")
        {
            npcContribution = npcStats.GetStat("agility").x + npcStats.GetStat("strength").x;
        }
        else if (skill == "reasoning")
        {
            npcContribution = 100 - npcStats.GetStat("curiosity").x - npcStats.GetStat("reasoning").x;
        }
        else if (skill == "agility")
        {
            npcContribution = npcStats.GetStat("observation").x + npcStats.GetStat("reasoning").x;
        }
        else if (skill == "strength")
        {
            npcContribution = npcStats.GetStat("curiosity").x + npcStats.GetStat("observation").x;
        }
        else if (skill == "socialSense")
        {
            npcContribution = 100 - npcStats.GetStat("socialSense").x - npcStats.GetStat("socialSense").x;
        }
        else
        {
            Debug.Log("invalid skill");
            npcContribution = 0;
        }
        float probability = 100 * Mathf.Pow((1 - 1 / (bet + 1)), 3 * (npcContribution + 5 * levelDifference));
        if (probability > UnityEngine.Random.Range(0f, 100f))
        {
            return true;
        }
        return false;
    }

    // When the PC selects a dialogue option, change current dialogue + options
    public void RespondToDialogue(int optionIndex, bool bettable, float bet)
    {
        string nextDialogue;

        XmlNodeList nextOptions;
        XmlElement OptionsInTree = currDialogueNode["options"];
        nextOptions = OptionsInTree.ChildNodes;

        if (nextOptions[optionIndex].ChildNodes.Count == 2)
        {
            nextDialogue = nextOptions[optionIndex]["goto"].InnerText;
        }

        else if (nextOptions[optionIndex].ChildNodes.Count == 5)
        {
            string skill = nextOptions[optionIndex]["skill"].InnerText;
            int difficulty = System.Convert.ToInt32(nextOptions[optionIndex]["difficulty"].InnerText);
            if (bettable)
            {
                if (IsSuccessful(skill, difficulty, bet))
                {
                    nextDialogue = nextOptions[optionIndex]["success"].InnerText;
                }
                else
                {
                    nextDialogue = nextOptions[optionIndex]["failure"].InnerText;
                }
            }
            else
            {
                nextDialogue = nextOptions[optionIndex]["success"].InnerText;
            }
        }

        else
        {
            nextDialogue = "EXIT";
            Debug.Log("Dialogue text has either too many or too few properties.");
        }
        
        if (nextDialogue == "EXIT")
        {
            StopTalking();
        }

        else
        {
            currDialogueNode = dialogueRoot.GetElementById(nextDialogue);
            DisplayCurrentDialogue();
        }
    }
}
