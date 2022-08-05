using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schwarzschild : Spacetime
{
    public float schwarzschildRad;
    public GameObject blackholeDisp;
    public override Metric GetMetric(Vector4 spaceTime)
    {
        return new Metric()
        {
            components = new float[4, 4] { { 1f - schwarzschildRad / spaceTime.x, -1f, 0f, 0f },
                                           { -1f, 0f, 0f, 0f },
                                           { 0f, 0f, -spaceTime.x * spaceTime.x, 0f },
                                           { 0f, 0f, 0f, -Mathf.Pow(Mathf.Sin(spaceTime.y) * spaceTime.x, 2) } }
        };
    }
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        Vector3 space = spaceTime;
        float rad = space.magnitude;
        float theta = Mathf.Acos(space.normalized.y);
        float phi = Mathf.Atan2(space.z, space.x);
        return new Vector4(rad, theta, phi, spaceTime.w + rad + schwarzschildRad * Mathf.Log(Mathf.Abs(rad / schwarzschildRad - 1f)));
    }
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        Vector3 spaceVel = spaceTimeVel;
        Vector3 perp = Vector3.Cross(cartesian, Vector3.up).normalized;
        float rad = Vector3.Dot(spaceVel, ((Vector3)cartesian).normalized);
        float phi = Vector3.Dot(spaceVel, perp) / ((Vector3)cartesian).magnitude;
        float theta = Vector3.Dot(spaceVel, Vector3.Cross(cartesian, perp)) / ((Vector3)cartesian).sqrMagnitude;

        return new Vector4(rad, theta, phi, spaceTimeVel.w);
    }
    public override Vector4 DelPositionCoords(Vector4 del)
    {
        return new Vector4(del.x, del.y, (del.z + Mathf.PI * 101f) % (Mathf.PI * 2f) - Mathf.PI, del.w);
    }
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        Vector3 space = new Vector3(Mathf.Cos(coordSpace.z) * Mathf.Sin(coordSpace.y), Mathf.Cos(coordSpace.y), Mathf.Sin(coordSpace.z) * Mathf.Sin(coordSpace.y)) * coordSpace.x;
        return new Vector4(space.x, space.y, space.z, coordSpace.w - coordSpace.x + schwarzschildRad * Mathf.Log(Mathf.Abs(coordSpace.x / schwarzschildRad - 1f)));
    }
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        Vector3 space = new Vector3(Mathf.Cos(coordSpace.z) * Mathf.Sin(coordSpace.y), Mathf.Cos(coordSpace.y), Mathf.Sin(coordSpace.z) * Mathf.Sin(coordSpace.y)) * coordSpace.x;
        return new Vector3(space.x, space.y, space.z);
    }
    private void Update()
    {
        if (blackholeDisp == null)
            return;
        blackholeDisp.transform.localScale = Vector3.one * schwarzschildRad * 2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, schwarzschildRad * 1.5f);
        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
        Gizmos.DrawWireSphere(Vector3.zero, schwarzschildRad * 2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.zero, schwarzschildRad * 3f);
    }
}
