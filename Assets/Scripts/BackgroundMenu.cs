using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMenu : MonoBehaviour {

    public SpriteRenderer[] LevelChilds;

    public float vel;
    public float size = -65;
    public int currentlyLevel;
    public float difumineVel;

    // Use this for initialization
    void Start() {
        DifumineAll();
        InvokeRepeating("MoveLevels", 0, vel);
    }

    private void DifumineAll()
    {
        for(int i=1; i < LevelChilds.Length;i++)
        {
            Color c = LevelChilds[i].color;
            c.a = 0;
            LevelChilds[i].color = c;
        }
    }
	
    private void MoveLevels()
    {
        transform.Translate(transform.right * -vel);
        
        if (transform.position.x <= size)
        {
            var pos = transform.position;
            pos.x = 0;
            transform.position = pos;
            StartCoroutine(Difumine(LevelChilds[1]));
            StartCoroutine(Fumine(LevelChilds[0]));
        }
        if (transform.position.x <= (size /LevelChilds.Length))
        {           
            StartCoroutine(Difumine(LevelChilds[0]));
            StartCoroutine(Fumine(LevelChilds[1]));
        }
       
    }

    private IEnumerator Difumine(SpriteRenderer level)
    {
        Color c = level.color;
        while (c.a > 0)
        {            
            c.a -= difumineVel;
            level.color = c;
            yield return null;
        }
    }

    private IEnumerator Fumine(SpriteRenderer level)
    {
        Color c = level.color;
        while (c.a < 1)
        {
            c.a += difumineVel;
            level.color = c;
            yield return null;
        }
    }
}
