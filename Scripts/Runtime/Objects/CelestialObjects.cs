using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.Game
{
    public class CelestialObjects : MonoBehaviour
    {
        [SerializeField]
        private GameObject visualObject;
        [SerializeField]
        private bool enableRotation = false;
        [SerializeField]
        private Vector3 equatorialInclination;
        [SerializeField]
        private float rotationDegreesPerSecond;
        [SerializeField]
        private bool enableTidalLocked = false;
        [SerializeField]
        private Transform tidalTarget;

        private void Update()
        {
            if (enableRotation)
            {
                RotateAtAxis();
            }
            else if(enableTidalLocked)
            {
                RotateTidalLocked();
            }
        }

        public void SetRotationAndAxis(float rotationPeriod, float equatorialInclination)
        {
            rotationDegreesPerSecond = 360 / Constants.ConvertDaysTo(Constants.TimeUnits.Seconds, rotationPeriod);
            this.equatorialInclination = new Vector3(0f, 0f, equatorialInclination);

            if (visualObject)
            {
                enableRotation = true;
            }
        }

        public void SetTidalLockedTarget(Transform target)
        {
            tidalTarget = target;

            if (visualObject)
            {
                enableTidalLocked = true;
            }
        }

        private void RotateAtAxis()
        {
            visualObject.transform.Rotate(equatorialInclination, rotationDegreesPerSecond * Time.deltaTime);
        }

        private void RotateTidalLocked()
        {
            visualObject.transform.LookAt(tidalTarget);
        }
    }
}
