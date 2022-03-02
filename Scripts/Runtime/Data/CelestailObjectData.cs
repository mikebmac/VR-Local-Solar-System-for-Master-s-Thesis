using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.Data
{
    [System.Serializable]
    public class CelestailObjectData
    {
        public GameObject celestailObject;
        public string objectName;
        [Tooltip("In KM.")]
        public float diameter;
        public float mass;
        public float density;
        [Tooltip("In Earth days.")]
        public float rotationPeriod;
        [Tooltip("In Earth years.")]
        public float orbitPeriod;
        public float equatorialInclination;
        public CelestailWarpPointData warpPoint;
        public CelestialSatelliteData[] satellites;

        public CelestailObjectData (string objectName, float diameter, float mass, float density, float rotationPeriod, float orbitPeriod, CelestailWarpPointData warpPoint = null, CelestialSatelliteData[] satellites = null)
        {
            this.objectName = objectName;
            this.diameter = diameter;
            this.mass = mass;
            this.density = density;
            this.rotationPeriod = rotationPeriod;
            this.orbitPeriod = orbitPeriod;
            this.warpPoint = warpPoint;
            this.satellites = satellites;

            if (warpPoint == null)
            {
                this.warpPoint = new CelestailWarpPointData(objectName, new Vector3());
            }
        }
    }
}