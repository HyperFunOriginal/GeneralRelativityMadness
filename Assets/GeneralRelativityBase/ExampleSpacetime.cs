using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSpacetime : Spacetime
{
    //Possible to declare global variables, just make sure they are consistent.
    public float wavelength;
    public bool customWave;
    //Implement your metric here
    public override Metric GetMetric(Vector4 spaceTime)
    {
        //Always use +--- signature to be safe, untested in -+++ signature.
        //Example çürséd mètrïc
        float x = -1f - Wave(spaceTime.w, spaceTime.z);
        float xWrap = 1f - Mathf.Pow(Mathf.Abs(spaceTime.x * 0.2f), 8f) * 0.5f;
        float yWrap = 1f - Mathf.Pow(Mathf.Abs(spaceTime.y * 0.2f), 8f) * 0.5f;
        float zWrap = 1f - Mathf.Pow(Mathf.Abs(spaceTime.z * 0.1f), 8f) * 0.5f;
        return new Metric()
        {
            components = new float[4, 4] { { 1f / (xWrap * yWrap * zWrap), 0f, 0f, 0f },
                                           { 0f, x * xWrap, 0f, 0f },
                                           { 0f, 0f, yWrap / x, 0f },
                                           { 0f, 0f, 0f, -zWrap } }
        };
    }

    //Implement your own functions
    public float Wave(float t, float z)
    {
        float factor = Mathf.Max(Mathf.Abs(Mathf.Cos(t * 0.005f) * 0.5f) - 0.05f, 0f);
        return Mathf.Sin(t * 0.5f + z * (customWave ? 1f / wavelength : factor)) * factor * factor;
    }
    //Implement a coordinate transform from cartesian (Unity) coordinates
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        return spaceTime;
    }
    //Implement a coordinate transform from cartesian (Unity) coordinates for both velocity and position
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        return spaceTimeVel;
    }
    //Implement a coordinate transform to cartesian (Unity) coordinates
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        float del = 1f + Wave(coordSpace.w, coordSpace.z);
        coordSpace.x *= del;
        coordSpace.y /= del;
        return coordSpace;
    }
    
    //Implement when you have a periodic coordinate system. To ensure that all coordinates around Vector4.zero is mapped to near Vector4.zero.
    public override Vector4 DelPositionCoords(Vector4 del)
    {
        del += new Vector4(15f, 15f, 30f);  
        del.x %= 10f;
        del.y %= 10f;
        del.z %= 20f;
        del -= new Vector4(5f, 5f, 10f);
        return del;
    }

    //For visualisation or other purposes.
    private void OnDrawGizmos()
    {
        for (float i = -10f; i < 11f; i += 1.5f)
        {
            float del = 1f + Wave(timeSlice, i);
            float del2 = 1f + Wave(timeSlice, i + .75f);
            float del3 = 1f + Wave(timeSlice, i + 1.5f);
            for (float j = -4f; j < 5f; j += 1.5f)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector3(j * del, -4f / del, i), new Vector3(j * del, 4f / del, i));
                Gizmos.DrawLine(new Vector3(-4f * del, j / del, i), new Vector3(4f * del, j / del, i));
                for (float k = -4f; k < 5f; k += 1.5f)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(new Vector3(j * del, k / del, i), new Vector3(j * del2, k / del2, i + .75f));
                    Gizmos.DrawLine(new Vector3(j * del2, k / del2, i + .75f), new Vector3(j * del3, k / del3, i + 1.5f));
                }
            }
        }
    }

    private void Start()
    {
        timeSlice = 0f;
    }

    float timeSlice;

    //Example Implementation    
    private new void LateUpdate()
    {
        if (!paused)
            timeSlice += timestep;
        //All implements of LateUpdate() must call base.LateUpdate().
        base.LateUpdate();
    }
    //Implement a coordinate transform to Unity position coordinates
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        float del = 1f + Wave(coordSpace.w, coordSpace.z);
        coordSpace.x *= del;
        coordSpace.y /= del;
        return coordSpace;
    }
}
