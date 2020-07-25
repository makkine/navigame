using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Option : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public bool clickable;
    public bool bettable;
    public Text text;
    public string optionText;
    public int optionIndex;
    public GameObject curr_NPC;
    bool mouseIsHovering = false;
    public float playerStat;
    public float bet;

    // Start is called before the first frame update
    void Start()
    {
        text.color = Color.clear;
    }


    // Update is called once per frame
    void Update()
    {
        ChangeBet();   
    }
    
    // Gets player input for incrementing and decrementing the bet.
    // TODO: Switch to arrow keys?
    private void ChangeBet()
    {
        if (mouseIsHovering && bettable)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                bet = Mathf.Clamp(bet - 20, 0, playerStat);
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                bet = Mathf.Clamp(bet + 20, 0, playerStat);
            }
            text.text = optionText + "; Bet: " + bet.ToString();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickable)
        {
            ExecuteEvents.Execute<INPCMessageTarget>(curr_NPC, null, (x, y) => x.RespondToDialogue(optionIndex, bettable, bet));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsHovering = true;
        if (clickable && bettable)
        {
            text.color = Color.blue;
        }
        else if (clickable)
        {
            text.color = Color.green;
        }
    }

    // TODO: Get rid of this; not working?
    public void OnPointerStay(PointerEventData eventData)
    {
        if (clickable)
        {
            text.color = Color.blue;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIsHovering = false;
        if (clickable)
        {
            text.color = Color.white;
        }
    }
}
