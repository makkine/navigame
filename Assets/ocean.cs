using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ocean : MonoBehaviour
{
    public int baseHeight; // height = (baseheight/2) * sin(x) + baseHeight
    public float Frequency; // Speed of animation
    public float RotateFactor; // Some rotation for naturalness

    protected int currHeight;
    protected double x; //  f(x) = (baseheight/2) * sin(x) + baseHeight
    protected float Rotate;

    // Start is called before the first frame update
    void Start()
    {
        x = 0.0;
        Rotate = 0.0f;
        InvokeRepeating("Rescale", 0.0f, Frequency);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Cheap animation to mimic waves lol
    void Rescale()
    {
        float ScalingFactor = (float)((baseHeight / 2.0) * Math.Sin(x));
        this.gameObject.transform.localScale = new Vector3(50, baseHeight + ScalingFactor, 50);
        var rotationVector = transform.rotation.eulerAngles;
        rotationVector.y = Rotate;
        transform.rotation = Quaternion.Euler(rotationVector);
        // This will overflow eventually, TODO: make it cycle (or, even better TODO: make an actual ocean mesh)
        x += 1.0;
        Rotate += RotateFactor;
    }
}
