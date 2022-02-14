using System.Collections;
using System.Collections.Generic;
using MacKay.Audio;
using UnityEngine;
using UnityEngine.InputSystem;
using AudioType = MacKay.Audio.AudioType;

public class AudioTesting : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.tKey.wasReleasedThisFrame)
        {
            AudioController.Instance.PlayAudio(AudioType.ST_COURAGE_WITHIN, 0f, 4f);
        }

        if (Keyboard.current.gKey.wasReleasedThisFrame)
        {
            AudioController.Instance.PlayAudio(AudioType.ST_DISTANT_DREAMS, 0f, 4f);
        }
    }
}
