using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class ButtonHelper : MonoBehaviour
{
    protected void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonPressed);
    }

    public virtual void OnButtonPressed() { }
}
