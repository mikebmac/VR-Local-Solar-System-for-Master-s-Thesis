using UnityEngine;
using DG.Tweening;

namespace MacKay.Animations
{
    public class ShipRumble : MonoBehaviour
    {
        [Header("Ship Rumble")]
        [SerializeField]
        private Transform spacecraft;

        [SerializeField]
        private bool isRumbling;
        public bool IsRumbling
        {
            get { return isRumbling; }
        }

        [SerializeField]
        private float rumbleIntesity = 1f;
        public float RumbleIntesity
        {
            get { return rumbleIntesity; }
        }

        #region Unity Methods
        private void Awake()
        {
            if (!spacecraft)
            {
                spacecraft = GameObject.FindGameObjectWithTag("PlayerShip").transform;
            }
        }
        #endregion

        #region Public Methods
        public void StartRumbling(float duration, float strength)
        {
            if (spacecraft != null)
                spacecraft.DOShakePosition(duration, strength, 30, 90, false, true);
        }
        #endregion
    }
}