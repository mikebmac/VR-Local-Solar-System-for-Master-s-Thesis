using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.Data
{
    [System.Serializable]
    public class CelestailSatelliteData
    {
        public string objectName;
        [Tooltip("In KM.")]
        public float diameter;
        public float mass;
        public float density;
        [Tooltip("In Earth days.")]
        public float rotationPeriod;
        [Tooltip("In Earth years.")]
        public float orbitPeriod;
        public GameObject satellitePrefab;
        [Tooltip("In KM.")]
        public float orbitRadius;
        public bool tidallyLocked;

        public CelestailSatelliteData(string objectName, float diameter = 0f, float mass = 0f, float density = 0f, float rotationPeriod = 0f, float orbitPeriod = 0f, GameObject satellitePrefab= null, float orbitRadius = 0f, bool tidallyLocked = false)
        {
            this.objectName = objectName;
            this.diameter = diameter;
            this.mass = mass;
            this.density = density;
            this.rotationPeriod = rotationPeriod;
            this.orbitPeriod = orbitPeriod;
            this.satellitePrefab = satellitePrefab;
            this.orbitRadius = orbitRadius;
            this.tidallyLocked = tidallyLocked;
        }
    }
}