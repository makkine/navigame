using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkyLight : MonoBehaviour
{
    public PController player;
    public Light l; // 
    public SceneChange SceneTarget;
    public DayNightManager dayNightCycle;

    protected float sunIntensity;
    protected float ticks; // After 10 ticks, leave! 
    protected string NextScene = "openOcean";


    // Start is called before the first frame update
    void Start()
    {
        sunIntensity = l.intensity;
    }

    // Update is called once per frame
    void Update()
    {
    }

    // This could pose an accessibility issue if it's too bright I worry, let's
    // keep thinking about how to handle map edges 
    void Brighten()
    {
        if (l.intensity > 0f) {
            if (l.intensity < 10f)
            {
                l.intensity += 0.01f;
            }
            else
            {
                l.intensity = sunIntensity;
                player.SailBack();

            }
        } else
        {
            if(ticks < 10f)
            {
                ticks += 0.05f;
            }
            else
            {
                ExecuteEvents.Execute<SceneChangeTarget>(SceneTarget.gameObject, null, (x, y) => x.ChangeScene(NextScene));
            }
        }
    }

    void Dim()
    {
        if (l.intensity > sunIntensity)
        {
            l.intensity -= 0.1f;
        }
        else
        {
            dayNightCycle.UnfreezeDay();
            CancelInvoke();
        }
    }

    public void onOpenOcean()
    {
        sunIntensity = dayNightCycle.FreezeDay();
        CancelInvoke();
        InvokeRepeating("Brighten", 0.0f, 0.05f);
    }



    public void backOnLand()
    {
        CancelInvoke();
        InvokeRepeating("Dim", 0.0f, 0.05f);
    }


}
