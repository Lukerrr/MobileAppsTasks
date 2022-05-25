using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    public static float ClampAngle(float val, float min, float max)
    {
        bool bUnclamp = false;
        if (val > 180)
        {
            val -= 360;
            bUnclamp = true;
        }

        val = Mathf.Clamp(val, min, max);

        if (bUnclamp)
        {
            val += 360;
        }

        return val;
    }

    public static float InterpConstantTo(float current, float target, float deltaTime, float interpSpeed)
    {
        float dist = target - current;

        if (dist * dist < 1e-8f)
        {
            return target;
        }

        float step = interpSpeed * deltaTime;
        return current + Mathf.Clamp(dist, -step, step);
    }

    public static float InterpAngleConstantTo(float current, float target, float deltaTime, float interpSpeed)
    {
        float dist = target - current;

        while (dist > 180f)
        {
            dist -= 360f;
        }

        while (dist < -180f)
        {
            dist += 360f;
        }

        if (dist * dist < 1e-8f)
        {
            return target;
        }

        float step = interpSpeed * deltaTime;
        return current + Mathf.Clamp(dist, -step, step);
    }

    public static bool NearlyEqual(float f1, float f2, float eps = 1e-3f)
    {
        return Mathf.Abs(f1 - f2) < eps;
    }

}
