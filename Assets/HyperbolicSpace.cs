using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperbolicSpace : Spacetime
{
    public float curvature;
    public override Metric GetMetric(Vector4 spaceTime)
    {
        float spaceFactor = (float)System.Math.Cosh(((Vector3)spaceTime).magnitude * curvature);
        return new Metric()
        {
            components = new float[4, 4] { { 1f, 0f, 0f, 0f },
                                           { 0f, -spaceFactor, 0f, 0f },
                                           { 0f, 0f, -spaceFactor, 0f },
                                           { 0f, 0f, 0f, -spaceFactor } }
        };
    }
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        return spaceTime;
    }
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        float spaceFactor = 1f / (float)System.Math.Cosh(((Vector3)cartesian).magnitude * curvature);
        return new Vector4(spaceFactor * spaceTimeVel.x, spaceFactor * spaceTimeVel.y, spaceFactor * spaceTimeVel.z, spaceTimeVel.w);
    }
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        return coordSpace;
    }
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        return coordSpace;
    }
    private void OnDrawGizmos()
    {
        if (curvature == 0f)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Vector3.zero, 1.31695789692f / Mathf.Abs(curvature));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Vector3.zero, 2.0634370689f / Mathf.Abs(curvature));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, 2.76865938331f / Mathf.Abs(curvature));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.zero, 3.46475790668f / Mathf.Abs(curvature));
        Gizmos.color = Color.Lerp(Color.yellow, Color.red, 0.5f);
        Gizmos.DrawWireSphere(Vector3.zero, 4.15863885328f / Mathf.Abs(curvature));
    }
}
