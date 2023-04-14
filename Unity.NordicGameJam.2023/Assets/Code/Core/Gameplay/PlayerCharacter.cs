using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour, IFeedable
{
    private Rigidbody2D _rigidbody;
    public Transform ResourceHolder;

    private Vector2 movementInput;
    public float MaxVelocity;

    public List<Resource> CollectedResources;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == GameConstants.RESOURCE_LAYER)
        {
            var re = col.gameObject.GetComponent<Resource>();
            if (!re.IsTaken)
            {
                TakeResource(re);
            }
        }
        
        if (col.gameObject.layer == GameConstants.WORM_LAYER)
        {
            var worm = col.gameObject.GetComponent<Worm>();
            foreach (var res in CollectedResources)
            {
                worm.ConsumeResource(res);
            }
            
            CollectedResources.Clear();
        }
    }

    
    private void TakeResource(Resource resource)
    {
        CollectedResources.Add(resource);
        resource.IsTaken = true;
        resource.transform.SetParent(ResourceHolder);
        resource.transform.localPosition = Random.insideUnitCircle;
    }

    public void ConsumeResource(Resource res)
    {
        
    }
    
    private void FixedUpdate()
    {
        _rigidbody.velocity = movementInput * MaxVelocity;
    }

    private void Update()
    {
        ResourceHolder.rotation = Quaternion.Euler(0.0f, 0.0f, Time.time * 360.0f);
    }

    public void ProvideFeed(float amount) { }

    public void OnMovement(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void OnConsume(InputValue value) { }

}