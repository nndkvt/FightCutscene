using UnityEngine;
using UnityEngine.Events;

public class ButtonSwitchUnityEvent : MonoBehaviour
{
    [SerializeField] private KeyCode _key;

    [SerializeField] private UnityEvent _onClickOn;
    [SerializeField] private UnityEvent _onClickOf;

    private bool _IsOn = true;

    private void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            if (_IsOn)
            {
                _onClickOn.Invoke();
                _IsOn = false;
            }
            else
            {
                _onClickOf.Invoke();
                _IsOn = true;
            }           
        }
    }
}



