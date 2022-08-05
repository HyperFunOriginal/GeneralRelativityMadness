using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisnerSpace : Spacetime
{
    public override Metric GetMetric(Vector4 spaceTime)
    {
        Metric m = new Metric()
        {
            components = new float[4, 4] { { 0f, 1f, 0f, 0f },
                                           { 1f, spaceTime.w, 0f, 0f },
                                           { 0f, 0f, -1f, 0f },
                                           { 0f, 0f, 0f, -1f } }
        };
        return m;
    }
    float Atanh(float x)
    {
        return 0.5f * Mathf.Log((x + 1) / (1 - x));
    }
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        float oldTime = spaceTime.w;
        spaceTime.w = 0.25f * (spaceTime.x * spaceTime.x - oldTime * oldTime);
        spaceTime.x = 2f * Mathf.Log((spaceTime.x - oldTime) / 2f);
        return spaceTime;
    }
    public override Vector4 DelPositionCoords(Vector4 del)
    {
        return new Vector4((del.x + 120f) % 4f, del.y, del.z, del.w);
    }
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        Vector4 wBasis = (ToCoordSystem(new Vector4(cartesian.x, cartesian.y, cartesian.z, cartesian.w + 0.01f)) - ToCoordSystem(cartesian)) * 100f;
        Vector4 xBasis = (ToCoordSystem(new Vector4(cartesian.x + 0.01f, cartesian.y, cartesian.z, cartesian.w)) - ToCoordSystem(cartesian)) * 100f;
        Matrix.NxNMatrix matrix = new Matrix.NxNMatrix(4);
        matrix.entries[0, 0] = wBasis.w;
        matrix.entries[0, 1] = wBasis.x;
        matrix.entries[1, 0] = xBasis.w;
        matrix.entries[1, 1] = xBasis.x;
        matrix.entries[2, 2] = 1f;
        matrix.entries[3, 3] = 1f;
        float[] vel = new float[4] { spaceTimeVel.w, spaceTimeVel.x, spaceTimeVel.y, spaceTimeVel.z };
        vel = matrix.GetInverse().Transform(vel);
        return new Vector4(vel[1], vel[2], vel[3], vel[0]);
    }
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        //float f = 2f * Mathf.Sqrt(Mathf.Abs(coordSpace.w));
        //coordSpace.w = (float)(f * System.Math.Cosh(coordSpace.x / 2f));
        //coordSpace.x = (float)(f * System.Math.Sinh(coordSpace.x / 2f));
        return coordSpace;
    }
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        //float f = 2f * Mathf.Sqrt(Mathf.Abs(coordSpace.w));
        //coordSpace.x = (float)(f * System.Math.Sinh(coordSpace.x / 2f));
        return coordSpace;
    }
}
