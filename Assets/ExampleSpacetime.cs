using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSpacetime : Spacetime
{
    //Implement your metric here
    public override Metric GetMetric(Vector4 spaceTime)
    {
        //Example çürséd mètrïc
        return new Metric()
        {
            components = new float[4, 4] { { 1f, 0f, 0f, 0f },
                                           { 0f, -spaceTime.x * spaceTime.x - 1f, -spaceTime.x * spaceTime.y, 0f},
                                           { 0f, -spaceTime.x * spaceTime.y, -spaceTime.y * spaceTime.y - 1f, spaceTime.y * spaceTime.z },
                                           { 0f, 0f, spaceTime.y * spaceTime.z, -spaceTime.z * spaceTime.z - 1f } }
        };
    }
    //Implement a coordinate transform from cartesian (Unity) coordinates
    public override Vector4 ToCoordSystem(Vector4 spaceTime)
    {
        return spaceTime;
    }
    //Implement a coordinate transform from cartesian (Unity) coordinates for both velocity and position
    public override Vector4 ToCoordSystemVelocity(Vector4 spaceTimeVel, Vector4 cartesian)
    {
        return spaceTimeVel;
    }
    //Implement a coordinate transform to cartesian (Unity) coordinates
    public override Vector4 FromCoordSystem(Vector4 coordSpace)
    {
        return coordSpace;
    }
    
    //Implement when you have a periodic coordinate system. To ensure that all coordinates around Vector4.zero is mapped to near Vector4.zero.
    public override Vector4 DelPositionCoords(Vector4 del) => del;
    
    //Example Implementation    
    private new void LateUpdate()
    {
        //All implements of LateUpdate() must call base.LateUpdate().
        base.LateUpdate();
    }
    //Implement a coordinate transform to Unity position coordinates
    public override Vector3 FromCoordSystemCart(Vector4 coordSpace)
    {
        return coordSpace;
    }
}
