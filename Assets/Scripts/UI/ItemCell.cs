using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemCell : MonoBehaviour {

    private ScrollRect parentScroll;

    void Start () 
    {
        parentScroll = GetComponentInParent<ScrollRect>();
    }
 
}
