using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KerrSchild : Spacetime
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
    public override Metric GetMetric(Vector4 spaceTime)
    {
        Metric m = new Metric()
        {
            components = new float[4, 4] { { 1f, 0f, 0f, 0f },
                                           { 0f, -1f, 0f, 0f },
                                           { 0f, 0f, -1f, 0f },
                                           { 0f, 0f, 0f, -1f } }
        };
        float f = FFactor(spaceTime);
        Vector4 k = K(spaceTime);
        float[] kArray = new float[4] { k.w, k.x, k.y, k.z };
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                m.components[i, j] -= f * kArray[i] * kArray[j];
        return m;
    }
    public float FFactor(Vector3 position)
    {
        float r = RFactor(position);
        return schwarzschildRad * r * r * r / (r * r * r * r + a * a * position.y * position.y);
    }
    public float RFactor(Vector3 position)
    {
        float b = (position.x * position.x + position.y * position.y + position.z * position.z - a * a) / 2f;
        float c = a * a * position.y * position.y;
        return Mathf.Sqrt(b + Mathf.Sqrt(b * b + c));
    }
    public Vector4 K(Vector3 position)
    {
        float r = RFactor(position);
        float kx = r * position.x + a * position.z;
        float ky = position.y / r;
        float kz = r * position.z - a * position.x;
        return new Vector4(kx / (r * r + a * a), ky, kz / (r * r + a * a), 1f);
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

    public Vector4 FCSDebug(Vector4 coordSpace)
    {
        float n = Mathf.Sqrt(coordSpace.x * coordSpace.x + a * a);
        Vector3 space = new Vector3(n * Mathf.Sin(coordSpace.y) * Mathf.Cos(coordSpace.z), coordSpace.x * Mathf.Cos(coordSpace.y), n * Mathf.Sin(coordSpace.y) * Mathf.Sin(coordSpace.z));
        return new Vector4(space.x, space.y, space.z, coordSpace.w);
    }

    public Vector3 FCSCDebug(Vector4 coordSpace)
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
        float eq = FCSDebug(new Vector4(rad, Mathf.PI / 2f, 0f, 0f)).magnitude;
        blackholeDisp.transform.localScale = new Vector3(eq, FCSDebug(new Vector4(rad, 0f, 0f, 0f)).magnitude, eq) * 2f;
    }

    private void OnDrawGizmos()
    {
        spinParam = 2 * a / schwarzschildRad;
        float rad = FCSCDebug(new Vector4(0, Mathf.PI / 2f, 0f)).magnitude;
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