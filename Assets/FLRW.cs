using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FLRW : Spacetime
{
    [Header("Assuming Mass Density of 0, Lambda in 10^-6")]
    public float lambda;
    public float iniExpansionRate;
    public float curvature;

    float GetLengthScale(float time)
    {
        time /= 1000f;
        float z = Mathf.Sqrt(lambda);
        float c = (z - iniExpansionRate) / (z + iniExpansionRate);
        if (lambda <= 0f)
            c = 1f;
        return (Mathf.Exp(z * time) + c * Mathf.Exp(-z * time)) / (1f + c);
    }

    public float SK(float r)
    {
        if (curvature == 0f)
            return r * r;
        if (curvature > 0f)
        {
            float t = Mathf.Sin(r * Mathf.Sqrt(curvature));
            return t * t / curvature;
        }
        float c = Mathf.Abs(curvature);
        float y = (float)System.Math.Sinh(r * Mathf.Sqrt(c));
        return y * y / c;
    }
    public override Metric GetMetric(Vector4 spaceTime)
    {
        float scaleFactor = GetLengthScale(spaceTime.w);
        scaleFactor *= scaleFactor;
        if (curvature == 0)
            return new Metric()
            {
                components = new float[4, 4] { { 1f, 0f, 0f, 0f },
                                           { 0f, -scaleFactor, 0f, 0f },
                                           { 0f, 0f, -scaleFactor, 0f },
                                           { 0f, 0f, 0f, -scaleFactor } }
            };

        float SKF = SK(spaceTime.x);
        return new Metric()
        {
            components = new float[4, 4] { { 1f, 0f, 0f, 0f },
                                           { 0f, -scaleFactor, 0f, 0f },
                                           { 0f, 0f, -SKF * scaleFactor, 0f },
                                           { 0f, 0f, 0f, -SKF * Mathf.Sin(spaceTime.y) * Mathf.Sin(spaceTime.y) * scaleFactor } }
        };
    }
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        Vector3 space = spaceTime;
        if (curvature == 0)
        {
            space /= GetLengthScale(spaceTime.w);
            return new Vector4(space.x, space.y, space.z, spaceTime.w);
        }
        float rad = space.magnitude / GetLengthScale(spaceTime.w);
        float theta = Mathf.Acos(space.normalized.y);
        float phi = Mathf.Atan2(space.z, space.x);
        return new Vector4(rad, theta, phi, spaceTime.w);
    }
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        if (curvature == 0)
            return spaceTimeVel;
        Vector3 spaceVel = spaceTimeVel;
        Vector3 perp = Vector3.Cross(cartesian, Vector3.up).normalized;
        float rad = Vector3.Dot(spaceVel, ((Vector3)cartesian).normalized) / GetLengthScale(cartesian.w);
        float phi = Vector3.Dot(spaceVel, perp) / ((Vector3)cartesian).magnitude;
        float theta = Vector3.Dot(spaceVel, Vector3.Cross(cartesian, perp)) / ((Vector3)cartesian).sqrMagnitude;

        return new Vector4(rad, theta, phi, spaceTimeVel.w);
    }
    public override Vector4 DelPositionCoords(Vector4 del)
    {
        if (curvature == 0)
            return base.DelPositionCoords(del);
        return new Vector4(del.x, del.y, (del.z + Mathf.PI * 101f) % (Mathf.PI * 2f) - Mathf.PI, del.w);
    }
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        if (curvature == 0)
        {
            Vector3 cart = coordSpace;
            cart *= GetLengthScale(coordSpace.w);
            return new Vector4(cart.x, cart.y, cart.z, coordSpace.w);
        }
        Vector3 space = new Vector3(Mathf.Cos(coordSpace.z) * Mathf.Sin(coordSpace.y), Mathf.Cos(coordSpace.y), Mathf.Sin(coordSpace.z) * Mathf.Sin(coordSpace.y)) * coordSpace.x * GetLengthScale(coordSpace.w);
        return new Vector4(space.x, space.y, space.z, coordSpace.w);
    }
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        if (curvature == 0)
        {
            Vector3 cart = coordSpace;
            return cart * GetLengthScale(coordSpace.w);
        }
        Vector3 space = new Vector3(Mathf.Cos(coordSpace.z) * Mathf.Sin(coordSpace.y), Mathf.Cos(coordSpace.y), Mathf.Sin(coordSpace.z) * Mathf.Sin(coordSpace.y)) * coordSpace.x;
        return new Vector3(space.x, space.y, space.z) * GetLengthScale(coordSpace.w);
    }

    internal float time;
    private void Update()
    {
        if (!paused)
            time += timestep;
    }

    private void OnDrawGizmos()
    {
        float s = GetLengthScale(time);
        if (curvature > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Vector3.zero, s * Mathf.Sqrt(10f / curvature));
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, s * Mathf.Sqrt(2.5f / curvature));
        }
        else
        {
            Gizmos.color = Color.blue;
            float scale = s / Mathf.Pow(10f, Mathf.FloorToInt(Mathf.Log10(s)));
            for (float i = -4.5f; i < 5; i++)
            {
                Gizmos.DrawRay((Vector3.right * i * 5f - Vector3.forward * 25f) * scale, scale * Vector3.forward * 50f);
                Gizmos.DrawRay((Vector3.forward * i * 5f - Vector3.right * 25f) * scale, scale * Vector3.right * 50f);
            }
            for (float i = -4.5f; i < 5; i++)
            {
                Gizmos.DrawRay((Vector3.right * i * .5f - Vector3.forward * 2.5f) * scale, scale * Vector3.forward * 5f);
                Gizmos.DrawRay((Vector3.forward * i * .5f - Vector3.right * 2.5f) * scale, scale * Vector3.right * 5f);
            }
            for (float i = -4.5f; i < 5; i++)
            {
                Gizmos.DrawRay((Vector3.right * i * 50f - Vector3.forward * 250f) * scale, scale * Vector3.forward * 500f);
                Gizmos.DrawRay((Vector3.forward * i * 50f - Vector3.right * 250f) * scale, scale * Vector3.right * 500f);
            }
            for (float i = -4.5f; i < 5; i++)
            {
                Gizmos.DrawRay((Vector3.right * i * 500f - Vector3.forward * 2500f) * scale, scale * Vector3.forward * 5000f);
                Gizmos.DrawRay((Vector3.forward * i * 500f - Vector3.right * 2500f) * scale, scale * Vector3.right * 5000f);
            }
        }
        float delTime = Mathf.Abs(GetLengthScale(time + 25f) - GetLengthScale(time - 25f)) / s * 0.02f;
        if (delTime < 0.0001f)
            return;
        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
        Gizmos.DrawWireSphere(Vector3.zero, .5f / delTime);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Vector3.zero, 1f / delTime);
    }
}
