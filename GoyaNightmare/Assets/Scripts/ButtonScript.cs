using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject _gameObject;

    // Update is called once per frame
    public void OnClick()
    {
        if (_gameObject.activeInHierarchy == true)
        {
            _gameObject.SetActive(false);
        }
        else
        {
            _gameObject.SetActive(true);
        }
    }
}
