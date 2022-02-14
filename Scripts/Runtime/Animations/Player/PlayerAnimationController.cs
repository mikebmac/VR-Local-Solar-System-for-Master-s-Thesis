using UnityEngine;
using MacKay.PlayerController.Ship;

namespace MacKay.Animations
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private static PlayerAnimationController _instance;
        public static PlayerAnimationController Instance { get { return _instance; } }

        [SerializeField]
        private ShipRumble anim_ShipRumble;
        public ShipRumble ShipRumble
        {
            get
            {
                if (!anim_ShipRumble)
                {
                    anim_ShipRumble = GetComponent<ShipRumble>();
                    if (!anim_ShipRumble)
                    {
                        anim_ShipRumble = gameObject.AddComponent<ShipRumble>();
                    }
                }
                return anim_ShipRumble;
            }
        }


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
            }
        }
        
        #endregion
    }
}