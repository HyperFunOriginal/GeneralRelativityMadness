using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schwarzschild : Spacetime
{
    public enum Coordinates
    {
        schwarzschild, eddingtonFinkelsteinIn, gullstrandPainlevé
    }
    public Coordinates usingCoordinateSystem;
    public float schwarzschildRad;
    public float hawkingRadiationMultiplier;
    public GameObject blackholeDisp;
    public GameObject hawkingRadParticle;

    public float Schwarz(float t) => Mathf.Pow(Mathf.Max(0f, schwarzschildRad * schwarzschildRad * schwarzschildRad * schwarzschildRad - hawkingRadiationMultiplier * t), 0.25f);
    public override Metric GetMetric(Vector4 spaceTime)
    {
        float sch = Schwarz(spaceTime.w);
        switch (usingCoordinateSystem)
        {
            case Coordinates.eddingtonFinkelsteinIn:
                return new Metric()
                {
                    components = new float[4, 4] { { 1f - sch / spaceTime.x, -1f, 0f, 0f },
                                           { -1f, 0f, 0f, 0f },
                                           { 0f, 0f, -spaceTime.x * spaceTime.x, 0f },
                                           { 0f, 0f, 0f, -Mathf.Pow(Mathf.Sin(spaceTime.y) * spaceTime.x, 2) } }
                };
            case Coordinates.gullstrandPainlevé:
                float y = Mathf.Sqrt(sch / spaceTime.x);
                return new Metric()
                {
                    components = new float[4, 4] { { 1f - sch / spaceTime.x, -y, 0f, 0f },
                                           { -y, -1f, 0f, 0f },
                                           { 0f, 0f, -spaceTime.x * spaceTime.x, 0f },
                                           { 0f, 0f, 0f, -Mathf.Pow(Mathf.Sin(spaceTime.y) * spaceTime.x, 2) } }
                };
            case Coordinates.schwarzschild:
                return new Metric()
                {
                    components = new float[4, 4] { { 1f - sch / spaceTime.x, 0f, 0f, 0f },
                                           { 0f, -1f / (1f - sch / spaceTime.x), 0f, 0f },
                                           { 0f, 0f, -spaceTime.x * spaceTime.x, 0f },
                                           { 0f, 0f, 0f, -Mathf.Pow(Mathf.Sin(spaceTime.y) * spaceTime.x, 2) } }
                };
        }
        return Tetrad.Minkowskian;
    }
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        Vector3 space = spaceTime;
        float sch = Schwarz(spaceTime.w);
        float rad = space.magnitude;
        float theta = Mathf.Acos(space.normalized.y);
        float phi = Mathf.Atan2(space.z, space.x);
        float timeCorrection = 0f;
        if (usingCoordinateSystem == Coordinates.eddingtonFinkelsteinIn)
            timeCorrection = rad + sch * Mathf.Log(Mathf.Abs(rad / sch - 1f));
        else if (usingCoordinateSystem == Coordinates.gullstrandPainlevé)
        {
            float y = Mathf.Sqrt(rad / sch);
            timeCorrection = sch * Mathf.Log(Mathf.Abs((y + 1)/(y - 1)));
        }
        return new Vector4(rad, theta, phi, spaceTime.w + timeCorrection);
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
        float sch = Schwarz(coordSpace.w);
        float timeCorrection = 0f;
        if (usingCoordinateSystem == Coordinates.eddingtonFinkelsteinIn)
            timeCorrection = -coordSpace.x + sch * Mathf.Log(Mathf.Abs(coordSpace.x / sch - 1f));
        else if (usingCoordinateSystem == Coordinates.gullstrandPainlevé)
        {
            float y = Mathf.Sqrt(coordSpace.x / sch);
            timeCorrection = sch * Mathf.Log(Mathf.Abs((y + 1) / (y - 1)));
        }
        return new Vector4(space.x, space.y, space.z, coordSpace.w + timeCorrection);
    }
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        Vector3 space = new Vector3(Mathf.Cos(coordSpace.z) * Mathf.Sin(coordSpace.y), Mathf.Cos(coordSpace.y), Mathf.Sin(coordSpace.z) * Mathf.Sin(coordSpace.y)) * coordSpace.x;
        return new Vector3(space.x, space.y, space.z);
    }
    float t, tick;
    private void Start()
    {
        t = 0f;
        tick = 0f;
        if (hawkingRadParticle != null && hawkingRadiationMultiplier > 0f)
            StartCoroutine(SlowUpdate());
    }

    IEnumerator SlowUpdate()
    {
        Object o = hawkingRadParticle.GetComponent<Object>();
        while (true)
        {
            while (tick < 1f)
                yield return new WaitForEndOfFrame();

            tick -= 1f;
            Vector4 rng = Random.onUnitSphere;
            rng.w = 1f;
            Vector4 pos = ToCoordSystem(rng * Mathf.Max(Schwarz(t), 0.01f) * 1.01f);
            pos.w = t;
            rng += (Vector4)Random.onUnitSphere * 0.5f;
            Destroy(Object.Instantiate(o, pos, rng, Quaternion.identity).gameObject, Mathf.Clamp(Mathf.Pow(Schwarz(t), 3f) / hawkingRadiationMultiplier, 5f, 25f));
        }
    }

    private void Update()
    {
        if (blackholeDisp == null)
            return;
        if (!paused)
        {
            t += timestep;
            if (Schwarz(t) > 0f)
                tick += timestep / Mathf.Pow(Schwarz(t) * .6f, 3f) * hawkingRadiationMultiplier;
        }
        blackholeDisp.transform.localScale = Vector3.one * Schwarz(t) * 2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, Schwarz(t) * 1.5f);
        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
        Gizmos.DrawWireSphere(Vector3.zero, Schwarz(t) * 2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.zero, Schwarz(t) * 3f);
    }
}
