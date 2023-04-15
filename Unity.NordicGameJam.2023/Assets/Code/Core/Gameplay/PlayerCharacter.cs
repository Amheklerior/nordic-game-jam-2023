using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour, IFeedable
{
    public Transform ResourceHolder;
    public List<Resource> CollectedResources;

    [Header("Movement")]    
    public float MaxVelocity;

    [Space] public float AccelerationTime;
    public AnimationCurve ForceAcceleration;

    [Space]
    public float DashForce = 15f;

    public float DashCooldown = 1f;
    
    private Vector2 movementInput;
    private Rigidbody2D _rigidbody;

    private float DashTimer;
    private float AccelerationTimer;
    private float _currentVelocity => 
        MaxVelocity * ForceAcceleration.Evaluate(AccelerationTimer / AccelerationTime);
    
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
        _rigidbody.AddForce(movementInput * _currentVelocity, ForceMode2D.Force);
    }

    private void Update()
    {
        ResourceHolder.rotation = Quaternion.Euler(0.0f, 0.0f, Time.time * 360.0f);

        AccelerationTiming();
        DashTiming();
    }

    private void DashTiming()
    {
        if (DashTimer != 0)
            DashTimer = Mathf.Clamp(DashTimer - Time.deltaTime, 0, DashCooldown);
    }

    private void AccelerationTiming()
    {
        if (movementInput != Vector2.zero && AccelerationTimer <= AccelerationTime)
            AccelerationTimer = Mathf.Clamp(AccelerationTimer + Time.deltaTime, (float)0, (float)AccelerationTime);
        else
            AccelerationTimer = 0;
    }

    public void ProvideFeed(float amount) { }

    public void OnMovement(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void OnDash(InputValue value)
    {
        if (DashTimer != 0) return;
        
        _rigidbody.AddForce(movementInput * DashForce, ForceMode2D.Impulse);
        DashTimer = DashCooldown;
    }

    public void OnConsume(InputValue value) { }

}