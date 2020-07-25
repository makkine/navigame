using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO: DIFFERENT UI OPTIONS IF DAY OR NIGHT

public class EndOfMap : MonoBehaviour
{
    public SceneChange SceneTarget;
    public DayNightManager Cycle;
    public string NextScene;
    public string myString;
    public string nightString;
    public Text myText;
    public SkyLight lght;
    protected bool displayInfo;


    // Start is called before the first frame update
    void Start()
    {
        myText.color = Color.clear;
        displayInfo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (displayInfo)
        {
            if (Input.GetKeyDown("x"))
            {
                ExecuteEvents.Execute<SceneChangeTarget>(SceneTarget.gameObject, null, (x, y) => x.ChangeScene(NextScene));
                displayInfo = false;
            }
            if (Cycle.currentTimeOfDay > 0.75 || Cycle.currentTimeOfDay < 0.25)
            {
                myText.text = nightString;
            } else
            {
                myText.text = myString;
            }
            myText.color = Color.white;

        }
    }

    public void OnTriggerExit()
    {
        // During the day, makes the player dizzy and forces them back on land. TODO: At night, 
        // Tracks player position and eventually makes them respawn on the open ocean
        displayInfo = true;
        lght.onOpenOcean();
    }

    public void OnTriggerEnter()
    {
        myText.color = Color.clear;
        displayInfo = false;
        lght.backOnLand();
    }

}
