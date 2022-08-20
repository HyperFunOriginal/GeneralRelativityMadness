using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitrarySpace : Spacetime
{
    public override Metric GetMetric(Vector4 spaceTime)
    {
        return new Metric() { components = new float[4, 4] { { 1f, -spaceTime.w, 0f, 0f }, { -spaceTime.w, 1f, 0f, spaceTime.y }, { 0f, 0f, -1f, 0f }, { 0f, spaceTime.y, 0f, -1f } } };
    }
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        return coordSpace;
    }
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        return coordSpace;
    }
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        return spaceTime;
    }
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        return spaceTimeVel;
    }
    public override Vector4 DelPositionCoords(Vector4 del)
    {
        del.w = (del.w + Mathf.PI * 5f) % (Mathf.PI * 2f) - Mathf.PI;
        del.x = (del.x + Mathf.PI * 5f) % (Mathf.PI * 2f) - Mathf.PI;
        del.y = (del.y + Mathf.PI * 5f) % (Mathf.PI * 2f) - Mathf.PI;
        del.z = (del.z + Mathf.PI * 5f) % (Mathf.PI * 2f) - Mathf.PI;
        return del;
    }
}
