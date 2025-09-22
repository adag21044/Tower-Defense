using UnityEngine;

public class TimeScale : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) Time.timeScale *= 2f;
        else
        if (Input.GetKeyDown(KeyCode.KeypadMinus)) Time.timeScale *= 0.5f;
        else
        if (Input.GetKeyDown(KeyCode.Alpha0)) Time.timeScale = 1;
    }
}