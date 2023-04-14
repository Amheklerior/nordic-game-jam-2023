using Sirenix.OdinInspector;
using UnityEngine;

public class Worm : MonoBehaviour, IFeedable
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

    public void ConsumeResource(Resource res)
    {
        currentFeed += res.FoodAmount;
        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
        Destroy(res.gameObject);
    }


    [Button]
    public void TestFeed()
    {
        currentFeed += 1.0f;
        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
    }
}