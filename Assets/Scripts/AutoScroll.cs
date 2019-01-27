using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScroll : MonoBehaviour {

    public float vel;
    public Transform[] maxMin;
    public GameObject triangle;
    private Vector3[] positions;
    public int divisions;
    // Update is called once per frame
    private void Start()
    {
        positions = new Vector3[divisions];
        Vector3 dif = maxMin[1].position - maxMin[0].position;
        
        for(int i =0; i<divisions; i++)
        {
            positions[i] = maxMin[0].position + dif * (i)/divisions;
        }
        StartCoroutine(GenerateTriangles(triangle, positions, dif));
    }
    void Update ()
    {
        transform.Translate(transform.right * vel);
    }

    private IEnumerator GenerateTriangles(GameObject triangle, Vector3[] positions, Vector3 dif)
    {
        while (true)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = maxMin[0].position + dif * (i) / positions.Length;
            }
            foreach (Vector3 p in positions)
            {
                GameObject a = Instantiate(triangle, p, Quaternion.Euler(0,0,0));
                a.transform.localScale = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            
            }
            yield return null;
        }
    }

}
