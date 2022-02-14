using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace MacKay.EditorShared
{
    public class ExposedFieldsEditorWindow : EditorWindow
    {
        private List<ExposedFieldInfo> exposedMembers = new List<ExposedFieldInfo>();

        public struct ExposedFieldInfo
        {
            public MemberInfo memberInfo;
            public ExposeFieldAttribute exposeFieldAttribute;

            public ExposedFieldInfo (MemberInfo info, ExposeFieldAttribute attribute)
            {
                memberInfo = info;
                exposeFieldAttribute = attribute;
            }
        }

        #region Unity Methods
        [MenuItem("Tools/Exposed Fields")]
        public static void Open()
        {
            ExposedFieldsEditorWindow window = CreateWindow<ExposedFieldsEditorWindow>("Exposed Fields");
        }

        private void OnEnable()
        {

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    MemberInfo[] members = type.GetMembers(flags);

                    foreach (MemberInfo member in members)
                    {
                        if (member.CustomAttributes.ToArray().Length > 0)
                        {
                            ExposeFieldAttribute attribute = member.GetCustomAttribute<ExposeFieldAttribute>();

                            if (attribute != null)
                            {
                                exposedMembers.Add(new ExposedFieldInfo(member, attribute));
                            }
                        }
                    }
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Exposed Fields", EditorStyles.boldLabel);
            GUIStyle labelStyle;
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.richText = true;

            foreach (ExposedFieldInfo member in exposedMembers)
            {
                EditorGUILayout.LabelField($"{member.exposeFieldAttribute.displayName} | <color=green>null</color>", labelStyle);
            }
        }

        #endregion
    }
}