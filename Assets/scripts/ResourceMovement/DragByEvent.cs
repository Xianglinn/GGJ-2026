using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragByEvent : MonoBehaviour
{
    private Vector3 startPos;

    private void Start(){

    }

    public void DragMethod(){
        transform.position = Input.mousePosition;
    }

    public void EndDragMethod(){

    }
}
