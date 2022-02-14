using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.Data
{
    [System.Serializable]
    public class Ship 
    {
        public State currentState;
        public float speedMin;
        public float speedMax;
        public float speedRange;

        public Ship(float speedMin, float speedMax, float speedRange)
        {
            this.currentState = State.NONE;
            this.speedMin = speedMin;
            this.speedMax = speedMax;
            this.speedRange = speedRange;
        }
    }

    public enum State
    {
        NONE,
        CONTROLLABLE,
        AUTOPILOT
    }
}