using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Universe finder; useful for finding the current spacetime all objects should live in.
/// </summary>
public static class GetSpaceTime
{
    public static Spacetime spaceTime => GameObject.FindGameObjectWithTag("Spacetime").GetComponent<Spacetime>();
}

/// <summary>
/// A relativistic object.
/// </summary>
[DisallowMultipleComponent()]
public class Object : MonoBehaviour
{
    public Vector4 spacetimeVel;
    public Vector4 spacetimeAcc;
    public Vector4 spaceTimePos;
    public float properTime;
    internal float properTimeStep;
    public bool lightlike;
    internal float properTimeClock;
    Vector3 lastPosition;
    int frameCount = 0;
    int infracCount = 0;

    public WorldLine worldLine;
    public Metric currentSpace;
    public Christoffel localChristoffel;
    internal Spacetime global;
    public Tetrad CurrFrame => new Tetrad(currentSpace, spacetimeVel);

    private void OnDrawGizmosSelected()
    {
        if (global == null || currentSpace.components == null)
            return;
        Tetrad basis = new Tetrad(currentSpace, spacetimeVel, false);
        Vector4 currPos = global.FromCoordSystem(spaceTimePos);
        Vector4 wBasis = (global.FromCoordSystem(spaceTimePos + basis.compCoordBasis[0] * .05f) - currPos) * 200f;
        Vector4 xBasis = (global.FromCoordSystem(spaceTimePos + basis.compCoordBasis[1] * .05f) - currPos) * 200f;
        Vector4 yBasis = (global.FromCoordSystem(spaceTimePos + basis.compCoordBasis[2] * .05f) - currPos) * 200f;
        Vector4 zBasis = (global.FromCoordSystem(spaceTimePos + basis.compCoordBasis[3] * .05f) - currPos) * 200f;

        Gizmos.color = Color.HSVToRGB(Mathf.Abs(wBasis.w / global.speedOfLight + 50f) % 1f, .6f, .8f);
        Gizmos.DrawRay(transform.position - (Vector3)wBasis, 2f * wBasis);
        Gizmos.color = Color.HSVToRGB(Mathf.Abs(xBasis.w / global.speedOfLight + 50f) % 1f, 1f, 1f);
        Gizmos.DrawRay(transform.position - (Vector3)xBasis, 2f * xBasis);
        Gizmos.color = Color.HSVToRGB(Mathf.Abs(yBasis.w / global.speedOfLight + 50f) % 1f, .5f, 1f);
        Gizmos.DrawRay(transform.position - (Vector3)yBasis, 2f * yBasis);
        Gizmos.color = Color.HSVToRGB(Mathf.Abs(zBasis.w / global.speedOfLight + 50f) % 1f, 1f, .7f);
        Gizmos.DrawRay(transform.position - (Vector3)zBasis, 2f * zBasis);
    }
    /// <summary>
    /// Must <b>always</b> be called! When overriding, must call base.Start() to avoid breaking the spacetime continuum.
    /// </summary>
    internal virtual void Start()
    {
        global = GetSpaceTime.spaceTime;
        properTime = 0f;
        spacetimeVel = global.ToCoordSystemVelocity(spacetimeVel, new Vector4(transform.position.x, transform.position.y, transform.position.z, spaceTimePos.w));
        Vector3 truePosition = global.ToCoordSystem(new Vector4(transform.position.x, transform.position.y, transform.position.z, spaceTimePos.w));
        spaceTimePos.x = truePosition.x;
        spaceTimePos.y = truePosition.y;
        spaceTimePos.z = truePosition.z;
        currentSpace = global.GetMetric(spaceTimePos);
        worldLine = new WorldLine();
        worldLine.Append(spaceTimePos - new Vector4(0, 0, 0, 1f), spacetimeVel);
        worldLine.Append(spaceTimePos, spacetimeVel);
        infracCount = 0;
    }
    void TempSpeed(Metric m)
    {
        float[] newSpacetimeVel = new float[4] { spacetimeVel.w, spacetimeVel.x, spacetimeVel.y, spacetimeVel.z };
        if (m.components[0,0] == 0)
        {
            float gr = 0f;
            for (int i = 1; i < 4; i++)
                gr += 2f * m.components[0, i] * newSpacetimeVel[i];
            float cn = -global.sqC;
            for (int i = 1; i < 4; i++)
                for (int j = 1; j < 4; j++)
                    cn += m.components[i, j] * newSpacetimeVel[i] * newSpacetimeVel[j];
            spacetimeVel.w = -cn / gr;
            return;
        }
        float b = 0f;
        for (int i = 1; i < 4; i++)
            b += 2f * m.components[0, i] * newSpacetimeVel[i];
        float c = -global.sqC;
        for (int i = 1; i < 4; i++)
            for (int j = 1; j < 4; j++)
                c += m.components[i, j] * newSpacetimeVel[i] * newSpacetimeVel[j];
        spacetimeVel.w = Mathf.Abs((-b + Mathf.Sqrt(Mathf.Max(b * b - 4 * m.components[0, 0] * c, 0))) / (2f * m.components[0, 0]));
    }
    public RicciTensors GetCurvatureAround()
    {
        Christoffel[] christoffelNeigh = new Christoffel[4];
        christoffelNeigh[0] = new Christoffel(new Metric[4] { global.GetMetric(spaceTimePos + new Vector4(0f, 0, 0, 0.12f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0.04f, 0, 0, 0.08f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0f, 0.04f, 0, 0.08f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0f, 0f, 0.04f, 0.08f)) }, global.GetMetric(spaceTimePos + new Vector4(0f, 0f, 0f, 0.08f)), .04f);
        
        christoffelNeigh[1] = new Christoffel(new Metric[4] { global.GetMetric(spaceTimePos + new Vector4(0.08f, 0, 0, 0.04f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0.12f, 0, 0, 0f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0.08f, 0.04f, 0, 0f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0.08f, 0f, 0.04f, 0f)) }, global.GetMetric(spaceTimePos + new Vector4(0.08f, 0f, 0f, 0f)), .04f);
        
        christoffelNeigh[2] = new Christoffel(new Metric[4] { global.GetMetric(spaceTimePos + new Vector4(0f, 0.08f, 0, 0.04f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0.04f, 0.08f, 0, 0f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0f, 0.12f, 0, 0f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0f, 0.08f, 0.04f, 0f)) }, global.GetMetric(spaceTimePos + new Vector4(0f, 0.08f, 0f, 0f)), .04f);
        
        christoffelNeigh[3] = new Christoffel(new Metric[4] { global.GetMetric(spaceTimePos + new Vector4(0f, 0f, 0.08f, 0.04f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0.04f, 0f, 0.08f, 0f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0f, 0.04f, 0.08f, 0f)),
                                                              global.GetMetric(spaceTimePos + new Vector4(0f, 0f, 0.12f, 0f)) }, global.GetMetric(spaceTimePos + new Vector4(0f, 0f, 0.08f, 0f)), .04f);
        return new RicciTensors(christoffelNeigh, localChristoffel, 0.08f);
    }
    bool Sanity()
    {
        if (lightlike)
        {
            float speedMul = Mathf.Min(10f, 5000f / ((Vector3)spacetimeVel).magnitude);
            spacetimeVel.x *= speedMul;
            spacetimeVel.y *= speedMul;
            spacetimeVel.z *= speedMul;
        }

        if (currentSpace.absLen(lastPosition - (Vector3)spaceTimePos) > 1.5f && frameCount > 15)
        {
            frameCount = 0;
            lastPosition = spaceTimePos;
            worldLine.Append(spaceTimePos, spacetimeVel);
        }
        frameCount++;

        TempSpeed(currentSpace);
        if (global.banParadoxes && currentSpace.sqLength(spacetimeVel) < 0f && !lightlike)
        {
            infracCount++;
            if (infracCount > 2)
            {
                Debug.LogWarning(name + " is travelling at FTL speeds for too long. That's illegal! Deleting object to save causality");
                Debug.LogWarning(name + " is traversing spacetime with speed dS²:" + currentSpace.sqLength(spacetimeVel));
                Destroy(gameObject, 5f);
                enabled = false;
            }
        }
        if (float.IsNaN(spaceTimePos.z))
            Destroy(gameObject);
        properTimeStep = global.timestep / Mathf.Max(Mathf.Abs(spacetimeVel.w), 1f);
        return float.IsNaN(spaceTimePos.z);
    }
    internal void ProperClockTick()
    {
        if (global.tickmark != null)
            Destroy(Instantiate(global.tickmark, transform.position, Quaternion.identity), 10f);
    }
    internal void GeodesicUpdate()
    {
        Metric[] neighbours = new Metric[4];
        neighbours[0] = global.GetMetric(spaceTimePos + new Vector4(0, 0, 0, 0.002f));
        neighbours[1] = global.GetMetric(spaceTimePos + new Vector4(0.002f, 0, 0, 0f));
        neighbours[2] = global.GetMetric(spaceTimePos + new Vector4(0, 0.002f, 0, 0f));
        neighbours[3] = global.GetMetric(spaceTimePos + new Vector4(0, 0, 0.002f, 0f));
        localChristoffel = new Christoffel(neighbours, currentSpace, 0.002f);
        float[] newSpacetimeVel = new float[4] { spacetimeVel.w, spacetimeVel.x, spacetimeVel.y, spacetimeVel.z };
        float[] newSpacetimeVelCpy = new float[4] { spacetimeVel.w, spacetimeVel.x, spacetimeVel.y, spacetimeVel.z };
        for (int alpha = 0; alpha < 4; alpha++)
            for (int mu = 0; mu < 4; mu++)
                for (int nu = 0; nu < 4; nu++)
                    newSpacetimeVelCpy[alpha] -= localChristoffel.components[alpha, mu, nu] * newSpacetimeVel[mu] * newSpacetimeVel[nu] * properTimeStep;
        spacetimeVel = new Vector4(newSpacetimeVelCpy[1], newSpacetimeVelCpy[2], newSpacetimeVelCpy[3], newSpacetimeVelCpy[0]);
    }
    void Update()
    {
        if (global.paused)
            return;
        spacetimeVel += spacetimeAcc * properTimeStep;
        spacetimeAcc = Vector4.zero;
        bool insane = Sanity();
        spaceTimePos = global.DelPositionCoords(spaceTimePos + spacetimeVel * properTimeStep);
        if (!lightlike)
        {
            properTime += properTimeStep;
            properTimeClock += properTimeStep;
            if (properTimeClock > 2f)
            {
                properTimeClock = 0f;
                ProperClockTick();
            }
        }

        if (!insane)
            transform.position = global.FromCoordSystemCart(spaceTimePos);

        float oldg00 = currentSpace.components[0, 0];
        currentSpace = global.GetMetric(spaceTimePos);
        if (Mathf.Abs(currentSpace.components[0, 0] - oldg00) > 1f) // Singularity
        {
            Destroy(gameObject, 5f);
            enabled = false;
            return;
        }
        GeodesicUpdate();
    }
}
/// <summary>
/// Local Christoffel Symbols around an object of a spacetime.
/// </summary>
public struct Christoffel
{
    public float[,,] components;
    public Christoffel(Metric[] neighbours, Metric curr, float del)
    {
        components = new float[4, 4, 4];
        Matrix.NxNMatrix inverse = new Matrix.NxNMatrix(curr.components).GetInverse();
        for (int sigma = 0; sigma < 4; sigma++)
            for (int gamma = 0; gamma < 4; gamma++)
                for (int alpha = 0; alpha < 4; alpha++)
                    for (int beta = 0; beta < 4; beta++)
                    {
                        float dels = neighbours[beta].components[sigma, alpha] + neighbours[alpha].components[sigma, beta] - neighbours[sigma].components[alpha, beta] - curr.components[sigma, alpha] - curr.components[sigma, beta] + curr.components[alpha, beta];
                        components[gamma, alpha, beta] += inverse.entries[gamma, sigma] / 2f * dels / del;
                    }
    }
}
/// <summary>
/// Tracks the nearby history of an object through spacetime
/// </summary>
public struct WorldLine
{
    public List<Vector4> positions;
    public List<Vector4> velocities;
    public const int maxNodeCount = 96;

    public void Append(Vector4 position, Vector4 velocity)
    {
        if (positions == null)
            positions = new List<Vector4>();
        if (velocities == null)
            velocities = new List<Vector4>();
        positions.Add(position);
        velocities.Add(velocity);
        if (positions.Count > 128)
        {
            positions.RemoveRange(0, positions.Count - maxNodeCount);
            velocities.RemoveRange(0, velocities.Count - maxNodeCount);
        }
    }
}
/// <summary>
/// A local minkowskian basis of an object.
/// </summary>
public struct Tetrad
{
    public Vector4[] compCoordBasis;
    internal Matrix.NxNMatrix invMatrix;

    public static Metric Minkowskian => new Metric()
    {
        components = new float[4, 4] { { 1f, 0f, 0f, 0f },
                                           { 0f, -1f, 0f, 0f },
                                           { 0f, 0f, -1f, 0f },
                                           { 0f, 0f, 0f, -1f } }
    };
    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < 4; i++)
            s += "[" + compCoordBasis[i].ToString() + "]" + (i != 3 ? ", " : "");
        return s;
    }
    public Tetrad(Metric m, Vector4 timeBasis, bool generateInverse = true)
    {
        compCoordBasis = new Vector4[4];
        compCoordBasis[0] = timeBasis / m.absLen(timeBasis);
        Vector4 xBasis = (Vector4)Vector3.right - Project(Vector3.right, timeBasis, m);
        Vector4 yBasis = (Vector4)Vector3.up - Project(Vector3.up, timeBasis, m);
        yBasis -= Project(yBasis, xBasis, m);
        Vector4 zBasis = (Vector4)Vector3.forward - Project(Vector3.forward, timeBasis, m);
        zBasis -= Project(zBasis, xBasis, m);
        zBasis -= Project(zBasis, yBasis, m);
        compCoordBasis[1] = xBasis / m.absLen(xBasis);
        compCoordBasis[2] = yBasis / m.absLen(yBasis);
        compCoordBasis[3] = zBasis / m.absLen(zBasis);

        invMatrix = new Matrix.NxNMatrix(4);
        if (generateInverse)
            GenerateContravariantBasis();
    }
    public void GenerateContravariantBasis()
    {
        for (int i = 0; i < 4; i++)
        {
            invMatrix.entries[i, 0] = compCoordBasis[i].w;
            invMatrix.entries[i, 1] = compCoordBasis[i].x;
            invMatrix.entries[i, 2] = compCoordBasis[i].y;
            invMatrix.entries[i, 3] = compCoordBasis[i].z;
        }
        invMatrix = invMatrix.GetInverse();
    }
    public Vector4 FrameToCoord(Vector4 delVector) => delVector.w * compCoordBasis[0] + delVector.x * compCoordBasis[1] + delVector.y * compCoordBasis[2] + delVector.z * compCoordBasis[3];
    public Vector4 CoordToFrame(Vector4 delVector)
    {
        float[] result = invMatrix.Transform(new float[4] { delVector.w, delVector.x, delVector.y, delVector.z });
        return new Vector4(result[1], result[2], result[3], result[0]);
    }
    public static Vector4 Project(Vector4 what, Vector4 onto, Metric metric) => metric.Dot(what, onto) * onto / metric.sqLength(onto);
}
/// <summary>
/// Describes curvature of spacetime around an object or a point in spacetime
/// </summary>
public struct RicciTensors
{
    public float[,,,] components;
    public float[,] ricciTensor;

    public RicciTensors(Christoffel[] neighbours, Christoffel curr, float del)
    {
        components = new float[4, 4, 4, 4];
        for (int sigma = 0; sigma < 4; sigma++)
            for (int gamma = 0; gamma < 4; gamma++)
                for (int epsil = 0; epsil < 4; epsil++)
                    for (int alpha = 0; alpha < 4; alpha++)
                        for (int beta = 0; beta < 4; beta++)
                        {
                            float d1 = (neighbours[beta].components[sigma, alpha, gamma] - curr.components[sigma, alpha, gamma]) / del;
                            float d2 = (neighbours[gamma].components[sigma, alpha, beta] - curr.components[sigma, alpha, beta]) / del;
                            components[sigma, alpha, beta, gamma] += d1 - d2 + curr.components[sigma, beta, epsil] * curr.components[epsil, alpha, gamma] - curr.components[sigma, gamma, epsil] * curr.components[epsil, alpha, beta];
                        }
        ricciTensor = new float[4, 4];
        for (int epsil = 0; epsil < 4; epsil++)
            for (int alpha = 0; alpha < 4; alpha++)
                for (int beta = 0; beta < 4; beta++)
                    ricciTensor[alpha, beta] += components[epsil, alpha, epsil, beta];
    }
    public float RicciScalar(Metric metr)
    {
        float result = 0f;
        float[,] comp = new Matrix.NxNMatrix(metr.components).GetInverse().entries;
        for (int alpha = 0; alpha < 4; alpha++)
            for (int beta = 0; beta < 4; beta++)
                result += ricciTensor[alpha, beta] * comp[alpha, beta];
        return result;
    }
}
/// <summary>
/// The metre-stick of spacetime. Measures distances and angles between 2 4-vectors.
/// </summary>
public struct Metric
{
    public float[,] components;
    public float sqLength(Vector4 a)
    {
        float[] aComp = new float[4] { a.w, a.x, a.y, a.z };
        float trueLen = 0f;
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                trueLen += components[i, j] * aComp[i] * aComp[j];
        return trueLen;
    }
    public float absLen(Vector4 a) => Mathf.Sqrt(Mathf.Abs(sqLength(a)));
    public float[,] basisProdLen
    {
        get
        {
            float[,] newComp = new float[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    newComp[i, j] = Mathf.Sqrt(Mathf.Abs(components[i, j]));
            return newComp;
        }
    }
    public float Dot(Vector4 a, Vector4 b)
    {
        float[] aPrime = new float[4] { a.w, a.x, a.y, a.z };
        float[] bPrime = new float[4] { b.w, b.x, b.y, b.z };
        float result = 0;
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                result += components[i, j] * aPrime[i] * bPrime[j];
        return result;
    }
}