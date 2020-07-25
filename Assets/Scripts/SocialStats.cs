using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SocialStats")]
public class SocialStats : ScriptableObject
{
    // Configuration Parameters

    // represent the social stats of a player or an NPC;
    // the x value is the social component and the y value is the physical component.
    // stats are capped at 5000; MAKE SURE TO CLAMP WHEN RAISING THEM
    // currently no fibonacci ascension; the level is the stat/100

    [SerializeField] Vector2 curiosity = new Vector2(10, 10);
    [SerializeField] Vector2 observation = new Vector2(10, 10);
    [SerializeField] Vector2 reasoning = new Vector2(10, 10);
    [SerializeField] Vector2 agility = new Vector2(10, 10);
    [SerializeField] Vector2 strength = new Vector2(10, 10);
    [SerializeField] Vector2 socialSense = new Vector2(10, 10);

    // get method for the social stats
    public Vector2 GetStat(string skill)
    {
        if (skill == "curiosity") return curiosity;
        if (skill == "observation") return observation;
        if (skill == "reasoning") return reasoning;
        if (skill == "agility") return agility;
        if (skill == "strength") return strength;
        if (skill == "socialSense") return socialSense;
        else
        {
            Debug.Log("getStat was called with an invalid skill area \"" + skill + "\"");
            return new Vector2(0, 0);
        }
    }
}
