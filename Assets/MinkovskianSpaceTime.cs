using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spacetimes in general.
/// </summary>
public abstract class Spacetime : MonoBehaviour
{
    /// <summary>
    /// Always set to true unless causality paradoxes are to be permitted.
    /// </summary>
    public bool banParadoxes = true;
    public GameObject tickmark;
    public float speedOfLight;
    public float timestep;
    internal bool paused;
    public float sqC
    {
        get
        {
            return speedOfLight * speedOfLight;
        }
    }
    public abstract Vector4 ToCoordSystem(Vector4 spaceTime);
    public virtual Vector4 DelPositionCoords(Vector4 del) => del;
    public abstract Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian);
    public abstract Metric GetMetric(Vector4 spaceTime);
    public abstract Vector4 FromCoordSystem(Vector4 coordSpace);
    public abstract Vector3 FromCoordSystemCart(Vector4 coordSpace);
    /// <summary>
    /// Must <b>always<\b> be called in LateUpdate() or an implementation of it. Will break the simulation otherwise.
    /// </summary>
    internal virtual void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            paused = !paused;
    }
}
/// <summary>
/// The most run-on-the-mill spacetime there is.
/// </summary>
public class MinkovskianSpaceTime : Spacetime
{
    public override Metric GetMetric(Vector4 spaceTime)
    {
        return new Metric()
        {
            components = new float[4, 4] { { 1f, 0f, 0f, 0f },
                                           { 0f, -1f, 0f, 0f },
                                           { 0f, 0f, -1f, 0f },
                                           { 0f, 0f, 0f, -1f } }
        };
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
}
