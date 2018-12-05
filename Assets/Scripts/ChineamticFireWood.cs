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

    public AudioSource[] song;
    public Transform[] both;
    public float distanceMin;
    public PhoneController Phone;

    private void Start()
    {
        
    }

    private void Update()
    {
        float d = Vector2.Distance(TransformToVec2(both[0].position), TransformToVec2(both[1].position));

        if (d < distanceMin)
        {
            foreach (AudioSource AS in song)
            {
                AS.volume = 1 - (d / distanceMin);
            }
        }
        else
            foreach (AudioSource AS in song)
                AS.volume = 0;


    }

    private Vector2 TransformToVec2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

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
                InvokeRepeating("ZoomIn", 0, 0.01f);
            Phone.gameObject.transform.localScale = new Vector3(0.009177777f, 0.009177777f, 0.02002002f);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (currentlyzoom == Zoom.In)
                InvokeRepeating("ZoomOut", 0, 0.01f);

            Phone.gameObject.transform.localScale = new Vector3 (0.01825265f, 0.01825265f, 0.02002002f);
        }
    }
}
