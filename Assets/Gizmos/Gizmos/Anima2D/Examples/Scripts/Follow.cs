using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Follow : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;
    public bool Enable;
    public float dis;

	void LateUpdate()
	{
        Debug.Log(gameObject.name + "  " + (transform.position.y - target.position.y));
		if(Enable && target)
		{
            if(transform.position.x - target.position.x <  dis)
			    transform.position = target.position + offset;
		}
	}

    public void Enableded()
    {
        Enable = true;
    }
}
