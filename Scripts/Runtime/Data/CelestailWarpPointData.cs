using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;

namespace MacKay.Data
{
    [System.Serializable]
    public class CelestailWarpPointData
    {
        public SgtFloatingTarget target;
        public string targetName;
        public Vector3 offset;

        public CelestailWarpPointData (string targetName, Vector3 offset)
        {
            this.targetName = targetName;
            this.offset = offset;
        }

        
    }
}