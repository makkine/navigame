using UnityEngine;
using System.Collections;

public class DayNightManager : MonoBehaviour
{

    public Light sun;
    public Light moon;
    public Skybox sbox;
    public float secondsInFullDay = 120f;
    [Range(0, 1)]
    public float currentTimeOfDay = 0;
    [HideInInspector]
    public float timeMultiplier = 1f;

    float sunInitialIntensity;
    float moonInitialIntensity;

    //Constants 
    protected Color Midnight = Color.black;
    protected Color Dawn = new Color(0.6037f, 0.4876f, 0.1617f, 1f);
    protected Color Sunrise = new Color(0.8867f, 0.8247f, 0.4969f, 1f);
    protected Color Noon = new Color(0.1879f, 0.1948f, 0.2830f, 1f);
    protected Color Sunset = new Color(0.5021f, 0.8396f, 0.8256f, 1f);
    protected Color Evening = new Color(0.1f, 0.54f, 0f, 1f);
    //Constants for moon illumination
    protected Color moonMorning = new Color(0.485f, 0.703f, 0.896f, 1f);
    protected Color moonSunset = new Color(1f, 0.75f, 0f, 1f);
    protected Color moonEvening = new Color(0.7506f, 0.556f, 0.8392f, 1f);
    protected Color moonDay = new Color(1f, 0.984f, 0.645f, 1f);

    //Is the day/night cycle frozen?
    protected bool m_dayFrozen;


    void Start()
    {
        sunInitialIntensity = sun.intensity;
        moonInitialIntensity = moon.intensity;
    }

    void Update()
    {
        if (!m_dayFrozen)
        {
            UpdateSky();

            currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

            if (currentTimeOfDay >= 1)
            {
                currentTimeOfDay = 0;
            }
        }
    }

    void UpdateSky()
    {
        //Sun rotation offset by 90 deg
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);
        // Moon rotation is the opposite of the sun rotation. Is this astronomically accurate? No but it's okay 

        float sIntensityMultiplier = 1;
        // Moon
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.77f) // nighttime
        {
            if (currentTimeOfDay >= 0.21f && currentTimeOfDay <= 0.23f) // Dawn
            {
                moon.color = moonMorning;
                sbox.material.SetColor("_SkyTint", Color.Lerp(Midnight, Dawn, (currentTimeOfDay - 0.21f) * (1 / 0.02f)));
            } else if (currentTimeOfDay >= 0.77f && currentTimeOfDay <= 0.79f) // Evening
            {
                sbox.material.SetColor("_SkyTint", Color.Lerp(Evening, Midnight, (currentTimeOfDay - 0.77f) * (1 / 0.02f)));
                moon.color = Color.Lerp(moonEvening, moonMorning, (currentTimeOfDay - 0.77f) * (1 / 0.02f));
            }
            else // Midnight
            {
                moon.color = moonMorning;
                sbox.material.SetColor("_SkyTint", Midnight);
            }
            sIntensityMultiplier = 0;
        }
        //Sun
        else{
            if (currentTimeOfDay <= 0.25f) // Sunrise
            {
                sbox.material.SetColor("_SkyTint", Color.Lerp(Dawn, Sunrise, (currentTimeOfDay - 0.23f) * (1 / 0.02f)));
                sIntensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
            }
            else
            {
                if (currentTimeOfDay <= 0.30f) // morning
                {
                    sbox.material.SetColor("_SkyTint", Color.Lerp(Sunrise, Noon, (currentTimeOfDay - 0.25f) * (1 / 0.05f)));
                    moon.color = Color.Lerp(moonMorning, moonDay, (currentTimeOfDay - 0.25f) * (1 / 0.05f));
                }
                else if (currentTimeOfDay >= 0.70f && currentTimeOfDay <= 0.75f) //afternoon{
                {
                    sbox.material.SetColor("_SkyTint", Color.Lerp(Noon, Sunset, (currentTimeOfDay - 0.70f) * (1 / 0.05f)));
                    moon.color = Color.Lerp(moonDay, moonSunset, (currentTimeOfDay - 0.70f) * (1 / 0.05f));
                }
                else if (currentTimeOfDay >= 0.75f) // sunset
                {
                    sbox.material.SetColor("_SkyTint", Color.Lerp(Sunset, Evening, (currentTimeOfDay - 0.75f) * (1 / 0.02f)));
                    moon.color = Color.Lerp(moonSunset, moonEvening, (currentTimeOfDay - 0.75f) * (1 / 0.05f));
                    sIntensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.75f) * (1 / 0.02f)));
                }
                else
                {
                    sbox.material.SetColor("_SkyTint", Noon);
                }
            }
        }

        sun.intensity = sunInitialIntensity * sIntensityMultiplier;
    }

    public float FreezeDay()
    {
        m_dayFrozen = true;
        return sun.intensity; 
    }

    public void UnfreezeDay()
    {
        m_dayFrozen = false;
    }

}
