using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [NonSerialized] public bool IsTaken = false;
    public float FoodAmount = 0.5f;
}