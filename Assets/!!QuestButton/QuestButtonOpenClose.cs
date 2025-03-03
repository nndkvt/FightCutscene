using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButtonOpenClose : MonoBehaviour
{
    [SerializeField] GameObject _openCloseObject;

    public void OpenClose()
    {
        _openCloseObject.SetActive(!_openCloseObject.activeSelf);
    }
}
