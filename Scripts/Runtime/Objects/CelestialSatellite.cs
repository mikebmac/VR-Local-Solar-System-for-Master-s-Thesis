using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.Game
{
    public class CelestialSatellite : CelestialObjects
    {
        [SerializeField]
        private bool isTidalLocked;
        [SerializeField]
        private GameObject parentCelestailObject;
    }
}