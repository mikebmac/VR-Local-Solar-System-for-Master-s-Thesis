using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;
using MacKay.Data;
using MacKay.PlayerController.Ship;
using Random = UnityEngine.Random;

public class CelestialSpawner : SgtFloatingSpawner
{
    [SerializeField]
    private List<CelestialSatelliteData> satellites = new List<CelestialSatelliteData>();
    public List<CelestialSatelliteData> Satellites
    {
        get { return satellites; }
        set { satellites = value; }
    }

    [SerializeField] 
    [Range(0.0f, 180.0f)] 
    private float tiltMax = 10.0f;
    public float TiltMax { 
        get { return tiltMax; } 
        set { tiltMax = value; } 
    }

    [SerializeField] 
    [Range(0.0f, 1.0f)] 
    private float oblatenessMax;
    public float OblatenessMax { 
        get { return oblatenessMax; } 
        set { oblatenessMax = value; } 
    }
    
    [SerializeField] 
    private ShipStateMachine _shipStateMachine;

    private void Start()
    {
        if (_shipStateMachine == null) _shipStateMachine = FindObjectOfType<ShipStateMachine>();
    }

    public override void SpawnAll()
    {
        if (satellites.Count == 0)
        {
            return;
        }

        StartCoroutine(Cor_SpawnSatellites());
    }
    
    public override void DespawnAll()
    {
        if (instances != null)
        {
            for (var i = instances.Count - 1; i >= 0; i--)
            {
                var instance = instances[i];

                if (instance != null)
                {
                    SgtHelper.Destroy(instance.gameObject);
                }
            }

            instances.Clear();
        }
    }

    private IEnumerator Cor_SpawnSatellites()
    {
        yield return new WaitUntil(() => _shipStateMachine.CurrentState is not ShipWarpState);
        
        var parentPoint = CachedObject;

        BuildSpawnList();

        SgtHelper.BeginRandomSeed(CachedObject.Seed);
        {
            for (int i = 0; i < satellites.Count; i++)
            {
                var radius = Constants.ConvertKmTo(Constants.DistanceUnits.Meters, satellites[i].orbitRadius);
                var angle = Random.Range(0.0f, 360.0f);
                var tilt = new Vector3(Random.Range(-TiltMax, TiltMax), 0.0f, Random.Range(-TiltMax, TiltMax));
                var oblateness = Random.Range(0.0f, OblatenessMax);
                var orbitSpeed = 360f / Constants.ConvertYearsTo(Constants.TimeUnits.Seconds, satellites[i].orbitPeriod);
                var position =
                    SgtFloatingOrbit.CalculatePosition(parentPoint, radius, angle, tilt, Vector3.zero, oblateness);
                var instance = SpawnAt(position, i);
                var orbit = instance.GetComponent<SgtFloatingOrbit>();

                if (orbit == null)
                {
                    orbit = instance.gameObject.AddComponent<SgtFloatingOrbit>();
                }

                float scale = Constants.ConvertKmTo(Constants.DistanceUnits.Meters, satellites[i].diameter);
                orbit.transform.localScale = new Vector3(scale, scale, scale);
                orbit.ParentPoint = parentPoint;
                orbit.Radius = radius;
                orbit.Angle = angle;
                orbit.Oblateness = oblateness;
                orbit.DegreesPerSecond = orbitSpeed;
                orbit.Tilt = tilt;
            }
        }
        SgtHelper.EndRandomSeed();
    }
}
