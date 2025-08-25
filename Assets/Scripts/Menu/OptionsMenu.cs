using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [Header("Paneles de las opciones")]
    public GameObject[] optionPanels;

    /*
        Metodo para activar el panel indicado
    */
    public void OpenOptions(int index)
    {
        for (int i = 0; i < optionPanels.Length; i++)
        {
            optionPanels[i].SetActive(i == index);
        }
    }
}
