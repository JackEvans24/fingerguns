using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimedVariables
{
    public static void UpdateTimeVariable(ref float currentTime, float timeLimit)
    {
        if (currentTime <= timeLimit)
            currentTime += Time.deltaTime;
    }
}
