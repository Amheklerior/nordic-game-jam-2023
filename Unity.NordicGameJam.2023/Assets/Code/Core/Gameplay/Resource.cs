using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Resource : MonoBehaviour
{
    public Action onConsume;
    [NonSerialized] public bool IsTaken = false;
    public float FoodAmount = 0.5f;

    private List<SpriteRenderer> renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    private void Start()
    {
        foreach (var rend in renderers)
        {
            ModifyVerticees(rend);
        }
    }

    void ModifyVerticees(SpriteRenderer rend)
    {
        Vector2[] newVerticees = new Vector2[64];
        ushort[]  indicees     = new ushort[62 * 3];


        ushort lastBig   = 63;
        ushort lastSmall = 3;

        ushort l1 = 0;
        ushort l2 = 1;
        ushort l3 = 2;

        for (int i = 0; i < 64; i++)
        {
            newVerticees[i] = new Vector2(Mathf.Sin(i / 64.0f * 2.0f * Mathf.PI) + 1.0f, Mathf.Cos(i / 64.0f * 2.0f * Mathf.PI) + 1.0f) * 128.0f;
        }

        for (int i = 0; i < 62; i++)
        {
            indicees[i * 3] = l1;
            indicees[i * 3 + 1] = l2;
            indicees[i * 3 + 2] = l3;

            if (i % 2 == 0)
            {
                l2 = l1;
                l1 = lastBig;
                lastBig--;
            }
            else
            {
                l2 = l3;
                l3 = lastSmall;
                lastSmall++;
            }
        }

        rend.sprite.OverrideGeometry(newVerticees, indicees);
    }
}