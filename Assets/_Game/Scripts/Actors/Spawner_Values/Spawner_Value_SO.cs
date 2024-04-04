using UnityEngine;

[CreateAssetMenu(fileName = "Spawner_Value_SO", menuName = "Spawner_Value_SO", order = 0)]
public class Spawner_Value_SO : ScriptableObject
{
    [Header("Spawning Parameters")]
    public ValueActor_Value valuePrefab = null;

    public float maxEjectionStrength = 500f;
    public float splitEjectionStrength = 1f;

    public float baseSpawnSpeed = 1.5f;
    public float baseValueToSpawn = 1f;
    
    [Header("Movement Parameters")]
    public float maxXPosition = 7f;
    public float smoothTime = 0.3f;
    public float pixelPerMeter = 100f;
}