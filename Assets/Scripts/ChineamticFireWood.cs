using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Zoom
{
    In,
    Out
}
public class ChineamticFireWood : MonoBehaviour {

    public Cinemachine.CinemachineVirtualCamera cam;
    public Zoom currentlyzoom;
    public float[] size;
    public float change;
    
    private void ZoomIn()
    {
        if (cam.m_Lens.OrthographicSize > size[0])
            cam.m_Lens.OrthographicSize -= change;
        else
        {
            currentlyzoom = Zoom.In;
            CancelInvoke("ZoomIn");
        }
    }
    private void ZoomOut()
    {
        if (cam.m_Lens.OrthographicSize < size[1])
            cam.m_Lens.OrthographicSize += change;
        else
        {
            currentlyzoom = Zoom.Out;
            CancelInvoke("ZoomOut");
        }  
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (currentlyzoom == Zoom.Out)
                InvokeRepeating("ZoomIn", 0, 0.03f);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (currentlyzoom == Zoom.In)
                InvokeRepeating("ZoomOut", 0, 0.03f);
        }
    }
}
