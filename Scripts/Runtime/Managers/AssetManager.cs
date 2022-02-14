using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.Managers
{
    public class AssetManager : MonoBehaviour
    {
        #region Fields
        public AudioClip warp;

        private static AssetManager _instance;
        #endregion

        #region Property
        public static AssetManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (Instantiate(Resources.Load("[ASSET_MANAGER]")) as GameObject).GetComponent<AssetManager>();
                }
                return _instance;
            }
        }
        #endregion

    }
}
