using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mic_UI : MonoBehaviour
{
    public Image UI;

    public void Show(bool v)
    {
        UI.enabled = v;
    }
}
