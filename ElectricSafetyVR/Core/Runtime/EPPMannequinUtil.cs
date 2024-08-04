using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class EPPMannequinUtil : MonoBehaviour
{
    [OnValueChanged("ActiveChanged")]
    public bool active = true;
    public GameObject[] activeGOs;

    private void Awake()
    {
        Set(true);
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var c = transform.GetChild(i);
            if (Array.IndexOf(activeGOs, c.gameObject) == -1)
            {
                Destroy(c.gameObject);
            }
        }
    }

    private void ActiveChanged()
    {
        Set(active);
    }

    private void Set(bool value)
    {
        active = value;
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            if (Array.IndexOf(activeGOs, c.gameObject) != -1)
            {
                c.gameObject.SetActive(active);
            }
            else
            {
                c.gameObject.SetActive(!active);
            }
        }
    }
}
