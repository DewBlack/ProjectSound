using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour {

    public Vector3 Start;
    public Vector3 End;

    public Vector3 diference()
    {
        return End - Start;
    }
}
