using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // in order to set in inspector use this
public struct BobbingProfile
{
    public float frequency;
    public float amount;
    public float noiseAmount;
    public float tiltAmount;
}

public static class CameraBobbingUtility
{

    public static Vector3 CalculatedBobbingResult(float time, BobbingProfile profile, float noiseSpeed)
    {
        float bobX = Mathf.Cos(time * profile.frequency / 2) * profile.amount * 1.6f;
        float bobY = Mathf.Sin(time * profile.frequency) * profile.amount * 1.4f;

        float noiseX = (Mathf.PerlinNoise(time * noiseSpeed, 0) - 0.5f) * profile.noiseAmount;
        float noiseY = (Mathf.PerlinNoise(0, time * noiseSpeed) - 0.5f) * profile.noiseAmount;

        float tiltX = Mathf.Cos(time * profile.frequency / 2) * profile.tiltAmount;
        float tiltY = Mathf.Sin(time * profile.frequency) * profile.tiltAmount; 

        return new Vector3(bobX + noiseX, bobY + noiseY, tiltX + tiltY);
    }
}
