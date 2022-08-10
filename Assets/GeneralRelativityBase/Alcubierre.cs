using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Alcubierre : Spacetime
{
    public float alcubierreVel;
    public float rs, rho;
    public float AlcubierrePosition(float time) => alcubierreVel * time;
    public float AlcubierreDistance(Vector4 pos) => ((Vector3)pos - Vector3.right * AlcubierrePosition(pos.w)).magnitude;
    public float AlcubierreFunction(float r) => (float)((Math.Tanh(rho * (r + rs)) - Math.Tanh(rho * (r - rs))) * .5d / Math.Tanh(rs * rho));

    public override Metric GetMetric(Vector4 spaceTime)
    {
        float f = AlcubierreFunction(AlcubierreDistance(spaceTime));
        return new Metric()
        {
            components = new float[4, 4] { { 1f - alcubierreVel * f * alcubierreVel * f, alcubierreVel * f, 0f, 0f },
                                           { alcubierreVel * f, -1f, 0f, 0f },
                                           { 0f, 0f, -1f, 0f },
                                           { 0f, 0f, 0f, -1f } }
        };
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.right * timeSlice * alcubierreVel, rs);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.right * timeSlice * alcubierreVel, Mathf.Max(0, rs - 1f / (rho + 1f / rs)));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Vector3.right * timeSlice * alcubierreVel, rs + 1f / rho);
    }
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        return spaceTime;
    }
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        return spaceTimeVel;
    }
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        return coordSpace;
    }
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        return coordSpace;
    }

    private void Start()
    {
        timeSlice = 0f;
    }
    float timeSlice;
    private new void LateUpdate()
    {
        if (!paused)
            timeSlice += timestep;
        base.LateUpdate();
    }
}
