using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonWaveScript : MonoBehaviour
{
    public int num;

    public int cuadrante;

    public correctWaveScript correctWaveScript;

    // M�todo intermedio
    public void CallMyFunction()
    {
        correctWaveScript.CheckWin(num, cuadrante); 
    }
}
