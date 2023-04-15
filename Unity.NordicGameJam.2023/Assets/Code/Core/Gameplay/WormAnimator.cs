using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WormAnimator : MonoBehaviour
{
    public HingeJoint2D PrefabBodyNode;
    public Rigidbody2D RootRigidbody;

    public int NodesToCreate;

    private void Awake()
    {
        var offset        = Vector3.left;
        var instPos       = RootRigidbody.transform.position;
        var prevRigidbody = RootRigidbody;
        for (int i = 0; i < NodesToCreate; i++)
        {
            instPos += Quaternion.Euler(0.0f, 0.0f, Random.Range(-30.0f, 30.0f)) * offset;
            var node = Instantiate(PrefabBodyNode, instPos, Quaternion.identity);
            node.connectedBody = prevRigidbody;
            prevRigidbody = node.GetComponent<Rigidbody2D>();
        }
    }
}