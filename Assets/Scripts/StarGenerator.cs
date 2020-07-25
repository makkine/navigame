using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Heavily inspired by https://medium.com/microsoft-design/building-a-virtual-sky-883d4d1080f4

[RequireComponent(typeof(ParticleSystem))]
public class StarGenerator : MonoBehaviour
{
    public TextAsset Data;
    public stars_type[] stars;
    public DayNightManager Cycle;

    protected ParticleSystem.Particle[] particleStars;
    protected ParticleSystem.EmitParams emitParams;
    protected ParticleSystem particleSys;

    // constants 
    float x = 0.0f;
    float y = 0.0f;
    float z = 0.0f;
    float ra = 0.0f;
    float dec = 0.0f;
    float r = 300.0f;
    float deg2rad = 57.296f; // Don't change this unless the fundamental properties of mathematics change

    int maxParticles = 10000;
    private int lastParticleIndex = 0;  // keeps track of our oldest particle (for deletion)



    // Start is called before the first frame update
    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        particleStars = new ParticleSystem.Particle[maxParticles];
        stars = LoadStars();
        for (int a = 0; a < maxParticles; a++)
        {
            int activeParticles = particleSys.GetParticles(particleStars);
            ra = stars[a].ra * -15.0f * deg2rad; // right ascension: also useful for navigation!
            dec = stars[a].dec * deg2rad; // declination: also useful for navigation! 
            SphericalToCartesian(ra, dec, r, ref x, ref y, ref z); // Math to turn spherical coords to cartesian coords
            particleStars[a].startSize = 10.0f * (8.0f - stars[a].mag);
            particleStars[a].startColor = Color.white * 0.1f * ( 8.0f - stars[a].mag) ;
            if (activeParticles >= maxParticles)
            {
                // set lifetime to -1 to kill the particle
                particleStars[lastParticleIndex].remainingLifetime = -1;

                lastParticleIndex++;    // keep track of oldest particle
                if (lastParticleIndex >= maxParticles) lastParticleIndex = 0;

                // have to re-write
                particleSys.SetParticles(particleStars, maxParticles);  // write those pesky particles back into our ParticleSystem
            }
            emitParams.position = new Vector3(x, y, z);
            emitParams.startLifetime = float.MaxValue; // float.MaxValue makes the particle's lifetime infinite
            particleSys.Emit(emitParams, 1);
            particleSys.Play();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Cycle != null)
        {
            if (Cycle.currentTimeOfDay > 0.70 || Cycle.currentTimeOfDay < 0.30)
            {
                particleSys.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                particleSys.GetComponent<Renderer>().enabled = false;
            }
        } else
        {
            particleSys.GetComponent<Renderer>().enabled = true;
        }
    }

    // Turns spherical coords to cartesian coords 
    void SphericalToCartesian(float ra, float dec, float r, ref float x, ref float y, ref float z)
    {
        ////az and alt are in radians
        dec = (Mathf.PI / 2) - dec;
        var rr = r * Mathf.Sin(dec);
        x = rr * Mathf.Cos(ra);
        y = rr * Mathf.Sin(ra);
        z = r * Mathf.Cos(dec);
    }

    stars_type[] LoadStars()
    {
        string[] parser = Data.ToString().Split('\n');
        stars_type[] starArr = new stars_type[maxParticles];
        int i = 1;
        while (i < maxParticles)
        {
            //Processing row
            var line = parser[i];
            var fields = line.Split(',');

            //UnityEngine.Debug.Log("RAscenscion: " + fields[7] + ", Dec: " + fields[8] + ", Mag: " + fields[13]);
            if (fields.Length > 12)
            {
                starArr[i].ra = float.Parse(fields[7]);
                starArr[i].dec = float.Parse(fields[8]);
                starArr[i].mag = float.Parse(fields[13]);
                starArr[i].x = float.Parse(fields[17]);
                starArr[i].y = float.Parse(fields[18]);
                starArr[i].z = float.Parse(fields[19]);
            }
            i++;
        }
        return starArr;
    }

    public struct stars_type
    {
        public float ra;
        public float dec;
        public float mag;
        public float x;
        public float y;
        public float z;
    }

}
