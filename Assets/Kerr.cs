using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kerr : Spacetime
{
    public float schwarzschildRad;
    public float angularMomentum;
    public float spinParam;
    public GameObject blackholeDisp;

    float mass
    {
        get
        {
            return sqC / 2f;
        }
    }
    float a
    {
        get
        {
            return angularMomentum / (speedOfLight * mass);
        }
    }
    float sigma(float r, float theta)
    {
        return r * r + a * a * Mathf.Cos(theta) * Mathf.Cos(theta);
    }
    float delta(float r, float theta)
    {
        return r * r - schwarzschildRad * r + a * a;
    }

    public override Metric GetMetric(Vector4 spaceTime)
    {
        float sigma = this.sigma(spaceTime.x, spaceTime.y);
        float phiComponent = spaceTime.x * spaceTime.x + a * a + schwarzschildRad * spaceTime.x * a * a / sigma * Mathf.Pow(Mathf.Sin(spaceTime.y), 2);
        float kerrCurlComponent = schwarzschildRad * spaceTime.x * a * Mathf.Pow(Mathf.Sin(spaceTime.y), 2) / sigma;
        return new Metric()
        {
            components = new float[4, 4] { { 1f - schwarzschildRad * spaceTime.x / sigma, 0f, 0f, kerrCurlComponent },
                                           { 0f, -sigma / delta(spaceTime.x, spaceTime.y), 0f, 0f },
                                           { 0f, 0f, -sigma, 0f },
                                           { kerrCurlComponent, 0f, 0f, -phiComponent * Mathf.Pow(Mathf.Sin(spaceTime.y), 2) } }
        };
    }
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        Vector3 space = spaceTime;
        float factor = (space.sqrMagnitude - a * a) / 2f;
        float rad = Mathf.Sqrt(factor + Mathf.Sqrt(factor * factor + space.y * space.y));
        float theta = Mathf.Acos(space.y / rad);
        float phi = Mathf.Atan2(space.z, space.x);
        return new Vector4(rad, theta, phi, spaceTime.w);
    }
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        Vector3 spaceVel = spaceTimeVel;

        float sqrMagnitude = ((Vector3)cartesian).sqrMagnitude;
        Vector3 radialBasis;
        {
            float factor = (sqrMagnitude - a * a) / 2f;
            float r = Mathf.Sqrt(factor + Mathf.Sqrt(factor * factor + cartesian.y * cartesian.y)) + 0.01f;
            float t = Mathf.Acos(cartesian.y / r);
            float p = Mathf.Atan2(cartesian.z, cartesian.x);
            radialBasis = (Vector3)(FromCoordSystem(new Vector4(r, t, p, 0f)) - cartesian) * 100f;
        }
        Vector3 thetaBasis;
        {
            float factor = (sqrMagnitude - a * a) / 2f;
            float r = Mathf.Sqrt(factor + Mathf.Sqrt(factor * factor + cartesian.y * cartesian.y));
            float t = Mathf.Acos(cartesian.y / r) + 0.01f;
            float p = Mathf.Atan2(cartesian.z, cartesian.x);
            thetaBasis = (Vector3)(FromCoordSystem(new Vector4(r, t, p, 0f)) - cartesian) * 100f;
        }
        Vector3 phiBasis;
        {
            float factor = (sqrMagnitude - a * a) / 2f;
            float r = Mathf.Sqrt(factor + Mathf.Sqrt(factor * factor + cartesian.y * cartesian.y));
            float t = Mathf.Acos(cartesian.y / r);
            float p = Mathf.Atan2(cartesian.z, cartesian.x) + 0.01f;
            phiBasis = (Vector3)(FromCoordSystem(new Vector4(r, t, p, 0f)) - cartesian) * 100f;
        }

        float rad = Vector3.Dot(spaceVel, radialBasis) / radialBasis.sqrMagnitude;
        float phi = Vector3.Dot(spaceVel, phiBasis) / phiBasis.sqrMagnitude;
        float theta = Vector3.Dot(spaceVel, thetaBasis) / thetaBasis.sqrMagnitude;

        return new Vector4(rad, theta, phi, spaceTimeVel.w);
    }
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        float n = Mathf.Sqrt(coordSpace.x * coordSpace.x + a * a);
        Vector3 space = new Vector3(n * Mathf.Sin(coordSpace.y) * Mathf.Cos(coordSpace.z), coordSpace.x * Mathf.Cos(coordSpace.y), n * Mathf.Sin(coordSpace.y) * Mathf.Sin(coordSpace.z));
        return new Vector4(space.x, space.y, space.z, coordSpace.w);
    }
    public override Vector4 DelPositionCoords(Vector4 del)
    {
        return new Vector4(del.x, del.y, (del.z + Mathf.PI * 101f) % (Mathf.PI * 2f) - Mathf.PI, del.w);
    }
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        float n = Mathf.Sqrt(coordSpace.x * coordSpace.x + a * a);
        Vector3 space = new Vector3(n * Mathf.Sin(coordSpace.y) * Mathf.Cos(coordSpace.z), coordSpace.x * Mathf.Cos(coordSpace.y), n * Mathf.Sin(coordSpace.y) * Mathf.Sin(coordSpace.z));
        return new Vector3(space.x, space.y, space.z);
    }

    private void Update()
    {
        spinParam = 2 * a / schwarzschildRad;
        if (blackholeDisp == null)
            return;
        blackholeDisp.SetActive(Mathf.Abs(spinParam) <= 1f);
        if (Mathf.Abs(spinParam) > 1f)
            return;
        float rad = (schwarzschildRad + Mathf.Sqrt(schwarzschildRad * schwarzschildRad - 4 * a * a))/2f;
        float eq = FromCoordSystem(new Vector4(rad, Mathf.PI / 2f, 0f, 0f)).magnitude;
        blackholeDisp.transform.localScale = new Vector3(eq, FromCoordSystem(new Vector4(rad, 0f, 0f, 0f)).magnitude, eq) * 2f;
    }

    private void OnDrawGizmos()
    {
        spinParam = 2 * a / schwarzschildRad;
        float rad = FromCoordSystemCart(new Vector4(0, Mathf.PI / 2f, 0f)).magnitude;
        Gizmos.color = Color.red;
        for (int i = 0; i < 15; i++)
        {
            Vector3 oldPos = new Vector3(Mathf.Sin(i * Mathf.PI / 7.5f), 0, Mathf.Cos(i * Mathf.PI / 7.5f)) * rad;
            Vector3 newPos = new Vector3(Mathf.Sin((i + 1) * Mathf.PI / 7.5f), 0, Mathf.Cos((i + 1) * Mathf.PI / 7.5f)) * rad;
            Gizmos.DrawLine(oldPos, newPos);
        }
        if (Mathf.Abs(spinParam) > 1f)
            return;
        float z1 = 1f + Mathf.Pow(1f - spinParam * spinParam, 0.333333333333f);
        float z2 = Mathf.Sqrt(3f * spinParam * spinParam + z1 * z1);
        float iscoPro = schwarzschildRad * (3f + z2 - Mathf.Sqrt((3f - z1) * (3f + z1 + 2 * z2)));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.zero, iscoPro);
    }
}