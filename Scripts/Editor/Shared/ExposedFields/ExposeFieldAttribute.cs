using System;
using UnityEngine;

namespace MacKay.EditorShared
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExposeFieldAttribute : Attribute
    {
        public string displayName;

        public ExposeFieldAttribute(string displayName)
        {
            this.displayName = displayName;
        }
    }
}