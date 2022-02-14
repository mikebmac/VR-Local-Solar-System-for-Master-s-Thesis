using UnityEditor;
using UnityEngine;
using MacKay.PlayerController.Ship;
using MacKay.UI;
using SpaceGraphicsToolkit;
using UnityEngine.UI;

namespace MacKay.Editor
{
    public class GroundControlManagerWindow : EditorWindow
    {
        private static GroundControlManagerWindow _window;
        private SgtFloatingTarget _targetWarp;
        [SerializeField] private Vector2 scrollPosition;

        [MenuItem("Tools/Ground Control")]
        public static void Open()
        {
            if (!_window) _window = GetWindow<GroundControlManagerWindow>("Ground Control");
            _window.Show();
        }

        #region UnityMethods

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += EditorApplicationPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= EditorApplicationPlayModeStateChanged;
        }

        #endregion

        #region Editor Methods
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginScrollView(scrollPosition);
            DrawWarpUI();
            DrawShipUI();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Private Methods
        private void DrawWarpUI()
        {
            EditorGUILayout.LabelField("Warping", EditorStyles.boldLabel);
            if (Application.isPlaying)
            {
                _targetWarp =
                    (SgtFloatingTarget) EditorGUILayout.ObjectField(_targetWarp, typeof(SgtFloatingTarget), true);

                if (_targetWarp == null)
                {
                    EditorGUILayout.HelpBox("Please select a warp target.", MessageType.Warning);
                }
                else
                {
                    if (GUILayout.Button("Warp"))
                    {
                        ShipActionHandler.Instance.WarpTo(_targetWarp);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Warping is a runtime event only.", MessageType.Warning);
            }
        }

        private void DrawShipUI()
        {
            EditorGUILayout.LabelField("Ship Orbit", EditorStyles.boldLabel);
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Enter Orbit"))
                {
                    UITouchButton_ActionHandler.Instance.UISplashOrbitEnterOrbit();
                }
                
                if (GUILayout.Button("Leave Orbit"))
                {
                    UITouchButton_ActionHandler.Instance.UISplashOrbitLeaveOrbit();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Ship orbit information can be viewed in runtime only.", MessageType.Warning);
            }
            
            EditorGUILayout.LabelField("Ship Music", EditorStyles.boldLabel);
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Play/Pause"))
                {
                    UITouchScreenController.Instance.TogglePlayButton();
                }
                
                if (GUILayout.Button("Next Track"))
                {
                    UITouchScreenController.Instance.NextTrack();
                }
                if (GUILayout.Button("Previous Track"))
                {
                    UITouchScreenController.Instance.PreviousTrack();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Ship music information can be viewed in runtime only.", MessageType.Warning);
            }
        }

        private void EditorApplicationPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.EnteredPlayMode ||
                playModeStateChange == PlayModeStateChange.ExitingPlayMode)
            {
                _targetWarp = null;
            }
        }
        #endregion
    }
}
