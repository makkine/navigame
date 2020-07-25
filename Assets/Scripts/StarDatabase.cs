using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Policy;

public class StarDatabase : MonoBehaviour
{

    public TextAsset Data;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Let's start by loading 500 stars
    public stars_type[] LoadStars()
    {
        string[] parser = Data.ToString().Split('\n');
        stars_type[] stars = new stars_type[119615];
        int i = 1;
        while (i < 119615)
        {
            //Processing row
            var line = parser[i];
            var fields = line.Split(',');

            //UnityEngine.Debug.Log("RAscenscion: " + fields[7] + ", Dec: " + fields[8] + ", Mag: " + fields[13]);
            if (fields.Length > 12)
            {
                stars[i].ra = float.Parse(fields[7]);
                stars[i].dec = float.Parse(fields[8]);
                stars[i].mag = float.Parse(fields[13]);
                stars[i].x = float.Parse(fields[17]);
                stars[i].y = float.Parse(fields[18]);
                stars[i].z = float.Parse(fields[19]);
            }
            i++;
        }
        return stars;
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
