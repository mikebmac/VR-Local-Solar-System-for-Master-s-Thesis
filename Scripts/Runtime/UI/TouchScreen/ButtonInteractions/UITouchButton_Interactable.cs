using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.UI
{
    public abstract class UITouchButton_Interactable : MonoBehaviour
    {

        protected abstract void OnTriggerEnter(Collider other);
    }
}