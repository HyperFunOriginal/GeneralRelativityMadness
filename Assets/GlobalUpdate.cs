using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalUpdate : MonoBehaviour
{
    public float maxTolerance;
    Object[] objs;
    Spacetime global;
    int frameCount = 0;
    bool doneRelaxing;
    public Func<Vector3, Vector4> DistanceForce;
    public Func<Vector3, Vector4, Vector4> VelocityForce;
    // Start is called before the first frame update
    void Start()
    {
        global = GetSpaceTime.spaceTime;
        objs = FindObjectsOfType<Object>();
        DistanceForce = NewtonianGravAndRepulsion;
        VelocityForce = Damping;
        doneRelaxing = false;
        if (global is MinkovskianSpaceTime)
        {
            maxTolerance = 10000f;
            DistanceForce = NewtonianGravAndRepulsionFlat;
        }
    }
    (Vector4, Vector4, bool) GetPositionAtSingleTime(Object target, Object thisObj, Tetrad frame)
    {
        Vector4 time = frame.CoordToFrame(global.DelPositionCoords(target.worldLine.positions[0] - thisObj.spaceTimePos));
        Vector4 v1 = frame.CoordToFrame(target.worldLine.velocities[0] - thisObj.spacetimeVel);
        int maximum = target.worldLine.positions.Count;
        for (int i = 1; i < maximum; i += (maximum - i) / 12 + 1)
        {
            Vector4 newTime = frame.CoordToFrame(global.DelPositionCoords(target.worldLine.positions[i] - thisObj.spaceTimePos));
            Vector4 v2 = frame.CoordToFrame(target.worldLine.velocities[i] - thisObj.spacetimeVel);
            if (newTime.w > 0f)
            {
                float lerp = InverseLerp(0f, time.w, newTime.w);
                return (Vector4.LerpUnclamped(time, newTime, lerp), Vector4.LerpUnclamped(v1, v2, lerp), Tetrad.Minkowskian.absLen(Vector4.LerpUnclamped(time, newTime, lerp)) > maxTolerance);
            }
            time = newTime;
            v1 = v2;
        }
        Vector4 finalTime = frame.CoordToFrame(global.DelPositionCoords(target.spaceTimePos - thisObj.spaceTimePos));
        float newLerp = InverseLerp(0f, time.w, finalTime.w);
        return (Vector4.LerpUnclamped(time, finalTime, newLerp), Vector4.LerpUnclamped(v1, frame.CoordToFrame(target.spacetimeVel - thisObj.spacetimeVel), newLerp), Tetrad.Minkowskian.absLen(Vector4.LerpUnclamped(time, finalTime, newLerp)) > maxTolerance);
    }
    
    //Template Forces
    Vector4 NewtonianGravAndRepulsion(Vector3 del)
    {
        float constant = 1f / (.1f + maxTolerance * maxTolerance * maxTolerance) - .5f / Mathf.Max(0.007f, maxTolerance * maxTolerance * maxTolerance * maxTolerance - 0.1f);
        return del * (1f / (.1f + del.sqrMagnitude * del.magnitude) - constant - .5f / Mathf.Max(0.007f, del.sqrMagnitude * del.sqrMagnitude - 0.1f)) * 5f;
    }
    Vector4 NewtonianGravAndRepulsionFlat(Vector3 del)
    {
        return del * (1f / (del.sqrMagnitude * del.magnitude) - 1f / (del.sqrMagnitude * del.sqrMagnitude)) * 5f;
    }
    Vector4 Damping(Vector3 del, Vector4 vel)
    {
        if (del.magnitude > 3f)
            return Vector4.zero;
        return Vector3.Dot(del.normalized, vel) / del.sqrMagnitude * del * 0.7f;
    }
    
    float InverseLerp(float v, float min, float max) => (v - min) * Mathf.Sign(max - min) / Mathf.Max(Mathf.Abs(max-min), 0.000001f);
    void LoopOverAll()
    {
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i] == null || !objs[i].enabled)
                continue;
            Object a = objs[i];
            if (a.lightlike)
                continue;
            Tetrad curr = a.CurrFrame;
            for (int j = 0; j < objs.Length; j++)
            {
                if (objs[j] == null || i == j || !objs[j].enabled)
                    continue;
                Object b = objs[j];
                if (b.lightlike)
                    continue;
                (Vector4, Vector4, bool) data = GetPositionAtSingleTime(b, a, curr);
                if (data.Item3)
                    continue;
                Vector4 dist = DistanceForce.Invoke(data.Item1);
                Vector4 vel = VelocityForce.Invoke(data.Item1, data.Item2);
                Vector4 force = curr.FrameToCoord(vel + dist);
                a.spacetimeAcc += force;
            }
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (global.paused)
            return;
        if (doneRelaxing)
            LoopOverAll();
        if (frameCount > 20)
        {
            objs = FindObjectsOfType<Object>();
            frameCount = 0;
            doneRelaxing = true;
        }
        frameCount++;
    }
}
