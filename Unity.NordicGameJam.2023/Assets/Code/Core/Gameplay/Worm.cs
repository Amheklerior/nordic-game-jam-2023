using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Worm : MonoBehaviour, IFeedable
{
    public TeamDefinitions WormTeam;
    [Space]
    public AnimationCurve FeedToVelocityCurve;
    public float BaseVelocity;

    private Transform _transform;
    private Rigidbody2D _rigidbody;

    public float MaxFeed;
    private float currentFeed;
    [ShowInInspector] public float CurrentFeed => currentFeed;

    public float FeedConsumptionRate;

    private Vector3 startPoint;
    private const float PATH_AMPLITUDE = 2.0f;
    private const float PATH_WAVE_LENGTH = 4.0f;
    private const float FORWARD_DISTANCE = 0.3f;

    private void Awake()
    {
        startPoint = transform.position;
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector3 TargetMovePoint()
    {
        float evaluateDistance = transform.position.x - startPoint.x + FORWARD_DISTANCE;
        var   targetY          = Mathf.Sin(evaluateDistance * Mathf.PI / PATH_WAVE_LENGTH) * PATH_AMPLITUDE + startPoint.y;
        return new Vector3(transform.position.x + FORWARD_DISTANCE, targetY, transform.position.z);
    }

    public void FixedUpdate()
    {
        Vector3 targetVector = TargetMovePoint() - transform.position;
        _rigidbody.velocity = targetVector.normalized * (BaseVelocity * FeedToVelocityCurve.Evaluate(currentFeed / MaxFeed));
        _rigidbody.SetRotation(Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg));
        currentFeed -= FeedConsumptionRate * Time.fixedDeltaTime;
        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
    }

    public void ConsumeResource(Resource res)
    {
        currentFeed += res.FoodAmount;
        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
        Destroy(res.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(TargetMovePoint(), 0.5f);
    }

    [Button]
    public void TestFeed()
    {
        currentFeed += 1.0f;
        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
    }
}