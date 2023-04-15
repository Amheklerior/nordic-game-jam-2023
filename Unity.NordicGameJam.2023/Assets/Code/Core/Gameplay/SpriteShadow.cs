using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteShadow : MonoBehaviour
{
    [SerializeField] private Vector2 _offSet;
    [SerializeField] private Material _shadowMaterial;

    private SpriteRenderer shadowRenderer;
    [SerializeField] private SpriteRenderer casterRenderer;

    private Transform shadowTransform;
    private Transform casterTransform;


    #region Unity Methods

    private void Awake()
    {
        casterTransform = transform;
    }

    private void Start()
    {
        shadowTransform = new GameObject($"{casterTransform.name}'s Shadow").transform;
        shadowTransform.parent = casterTransform;
        shadowRenderer = shadowTransform.AddComponent<SpriteRenderer>();
        if (casterRenderer == null)
            casterRenderer = GetComponent<SpriteRenderer>();

        shadowTransform.localScale = Vector3.one;
        shadowTransform.localPosition = _offSet;
        shadowRenderer.sprite = casterRenderer.sprite;
        shadowRenderer.sortingOrder = casterRenderer.sortingOrder - 1;
        shadowRenderer.material = _shadowMaterial;
    }

    void LateUpdate()
    {
        shadowTransform.position = casterTransform.position + (Vector3)_offSet;
        shadowTransform.localRotation = quaternion.Euler(0, 0, 0);
    }
    
    #endregion
}
