using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField]
    private Transform left;
    [SerializeField]
    private Transform right;
    [SerializeField]
    private Transform top;
    [SerializeField]
    private Transform bottom;

    public Transform Left
    {
        get { return left; }
    }

    public Transform Right
    {
        get { return right; }
    }

    public Transform Top
    {
        get { return top; }
    }

    public Transform Bottom
    {
        get { return bottom; }
    }
}
