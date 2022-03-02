using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SpaceGraphicsToolkit;
using MacKay.Data;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    #endregion

    [Header("Debug")]
    public bool enableDebug = false;

    [Header("Ship")]
    [SerializeField]
    private Ship shipData;
    public Ship ShipData
    {
        get => shipData;
        set => shipData = value;
    }

    [Header("Planets")]
    [SerializeField]
    private CelestailObjectData[] planets;
    public CelestailObjectData[] Planets => planets;

    [Header("Warp Points")]
    [SerializeField]
    private List<SgtFloatingTarget> warpPoints = new List<SgtFloatingTarget>();
    public List<SgtFloatingTarget> WarpPoints => WarpPoints;

    #region Unity Methods
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else
        {
            _instance = this;
            DOTween.Init(true, true, LogBehaviour.Default);
        }
    }

    private void Start()
    {
        LoadData();
    }
    #endregion

    #region Public Methods
    #endregion

    #region Data Loading Methods
    private void LoadData()
    {
        if (planets != null)
        {
            foreach (CelestailObjectData planet in planets)
            {
                if (planet.celestailObject)
                {
                    LoadPlanet(planet);
                }
            }
        }
    }

    private void LoadPlanet(CelestailObjectData planet)
    {
        float scale = Constants.ConvertKmTo(Constants.DistanceUnits.Meters, planet.diameter);
        planet.celestailObject.transform.localScale = new Vector3(scale, scale, scale);

        planet.celestailObject.GetComponent<MacKay.Game.CelestialPlanet>().SetRotationAndAxis(planet.rotationPeriod, planet.equatorialInclination);

        if (planet.warpPoint.target) LoadWarpPoint(planet.warpPoint);
        if (planet.satellites.Length > 0) LoadSatellites(planet);
    }

    private void LoadWarpPoint (CelestailWarpPointData point)
    {
        warpPoints.Add(point.target);
    }

    private void LoadSatellites(CelestailObjectData parentPlanet)
    {
        CelestialSpawner spawner = parentPlanet.celestailObject.GetComponent<CelestialSpawner>();

        if (!spawner)
        {
            LogWarning($"Attempted to load satellites without a CelestailSpawner on <b>{parentPlanet.objectName}</b>, creating a <i>CelestailSpawner</i>.");
            spawner = parentPlanet.celestailObject.AddComponent<CelestialSpawner>();
        }

        SgtSpawnList spawnList = parentPlanet.celestailObject.GetComponent<SgtSpawnList>();

        if (!spawnList)
        {
            LogWarning($"Attempted to load satellites without a SGTLSpawnList on <b>{parentPlanet.objectName}</b>, creating a <i>SgtSpawnList</i>.");
            spawnList = parentPlanet.celestailObject.AddComponent<SgtSpawnList>();
        }

        spawnList.Category = parentPlanet.objectName;
        spawner.Category = parentPlanet.objectName;

        foreach (CelestialSatelliteData satellite in parentPlanet.satellites)
        {
            if (!satellite.satellitePrefab)
            {
                LogWarning($"Satellite named <b>{satellite.objectName}</b> attached to <b>{parentPlanet.objectName}</b> does not contain a prefab. <i>The satellite will be skipped</i>.");
                continue;
            }

            SgtFloatingObject prefab = satellite.satellitePrefab.GetComponent<SgtFloatingObject>();
            if (prefab)
            {
                spawnList.Prefabs.Add(prefab);
                spawner.Satellites.Add(satellite);
            } 
            else
            {
                LogWarning($"Satellite named <b>{satellite.objectName}</b> attached to <b>{parentPlanet.objectName}</b> prefab does not contain a <i>SgtFloatingObject</i>. <i>The satellite will be skipped</i>.");
                continue;
            }

            MacKay.Game.CelestialSatellite sat = prefab.gameObject.GetComponent<MacKay.Game.CelestialSatellite>();
            if (sat == null)
            {
                LogWarning($"Satellite prefab <b>{satellite.objectName}</b> attached to <b>{parentPlanet.objectName}</b> does not contain a <i>CelestialSatellite</i>. <i>The satellite will be skipped</i>.");
                continue;
            }

            if (satellite.tidallyLocked)
            {
                sat.SetTidalLockedTarget(parentPlanet.celestailObject.transform);
            }
        }
    }

    private void Log(string msg)
    {
        if (!enableDebug) return;

        Debug.Log("<color=green>[GameManger]</color>: " + msg, this);
    }

    private void LogWarning(string msg)
    {
        if (!enableDebug) return;
        Debug.LogWarning("<color=red>[GameManger]</color>: " + msg, this);
    }
    #endregion
}
