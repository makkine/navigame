using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Option : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public bool clickable;
    public Text text;
    public string onClick;
    public GameObject curr_NPC;

    // Start is called before the first frame update
    void Start()
    {
        text.color = Color.clear;
    }


    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickable)
        {
            ExecuteEvents.Execute<INPCMessageTarget>(curr_NPC, null, (x, y) => x.RespondToDialogue(onClick));

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clickable)
        {
            text.color = Color.blue;
        }
    }

    public void OnPointerStay(PointerEventData eventData)
    {
        if (clickable)
        {
            text.color = Color.blue;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (clickable)
        {
            text.color = Color.white;
        }
    }
}
