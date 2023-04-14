using Sirenix.OdinInspector;
using UnityEngine;

public class Worm : MonoBehaviour
{
    public AnimationCurve FeedToVelocityCurve;
    public float BaseVelocity;

    private Transform _transform;
    private Rigidbody2D _rigidbody;

    public float MaxFeed;
    private float currentFeed;
    [ShowInInspector] public float CurrentFeed => currentFeed;

    public float FeedConsumptionRate;

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        _rigidbody.velocity = Vector3.right * (BaseVelocity * FeedToVelocityCurve.Evaluate(currentFeed / MaxFeed));

        currentFeed -= FeedConsumptionRate * Time.fixedDeltaTime;

        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
    }

    public void ProvideFeed(float amount)
    {
        currentFeed += amount;
        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
    }

    [Button]
    public void TestFeed()
    {
        ProvideFeed(1.0f);
    }
}