using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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

    private float startYPos;
    private const float PATH_AMPLITUDE = 2.0f;
    private const float PATH_WAVE_LENGTH = 4.0f;
    private const float FORWARD_DISTANCE = 1.0f;

    public float TailRotationSpeed;
    public float TailMovementSpeed;


    [FormerlySerializedAs("PrefabBodyNode")]
    public Rigidbody2D TailNodePrefab;

    private List<Rigidbody2D> tail = new List<Rigidbody2D>();
    public int NodesToCreate;
    private const float TAIL_SPACING = 0.9f;

    private void Awake()
    {
        startYPos = transform.position.y;
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody2D>();
        GenerateBody();

        GetComponent<SpriteRenderer>().color = WormTeam.SecondaryColor;
        for (int i = 0; i < tail.Count; i++)
        {
            tail[i].GetComponent<SpriteRenderer>().color = WormTeam.PrimaryColor;
        }
    }

    private void GenerateBody()
    {
        var prevNode      = transform;
        var rotationAngle = 0.0f;
        for (int i = 0; i < NodesToCreate; i++)
        {
            rotationAngle += Random.Range(-10.0f, 10.0f);
            var rotation = Quaternion.Euler(0.0f, 0.0f, rotationAngle);
            var spawnPos = prevNode.transform.position + prevNode.rotation * Vector3.left * TAIL_SPACING + rotation * Vector3.left * TAIL_SPACING;
            var node     = Instantiate(TailNodePrefab, spawnPos, rotation);

            prevNode = node.transform;
            tail.Add(node);
        }
    }

    private Vector3 XDistToPathPos(float x)
    {
        var targetY = Mathf.Sin(x * Mathf.PI / PATH_WAVE_LENGTH) * PATH_AMPLITUDE + startYPos;
        return new Vector3(x, targetY, transform.position.z);
    }

    public void FixedUpdate()
    {
        Vector3 targetVector = XDistToPathPos(transform.position.x + FORWARD_DISTANCE) - transform.position;
        _rigidbody.velocity = targetVector.normalized * (BaseVelocity * FeedToVelocityCurve.Evaluate(currentFeed / MaxFeed));
        _rigidbody.SetRotation(Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg));
        currentFeed -= FeedConsumptionRate * Time.fixedDeltaTime;
        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
        UpdateTail();
    }

    private void UpdateTail()
    {
        var prevNode      = transform;

        for (int i = 0; i < tail.Count; i++)
        {
            var node         = tail[i];
            var targetPos    = prevNode.transform.position + prevNode.rotation * Vector3.left * 1.1f + node.transform.rotation * Vector3.left * 1.1f;
            var lookAtVector = prevNode.transform.position + prevNode.rotation * Vector3.left * 1.1f - node.transform.position;
            var targetRot    = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(lookAtVector.y, lookAtVector.x) * Mathf.Rad2Deg);
            var currentRot   = Quaternion.Euler(0.0f, 0.0f, node.rotation);

            node.velocity = Vector3.zero;
            node.angularVelocity = 0.0f;

            node.SetRotation(Quaternion.RotateTowards(currentRot, targetRot, Time.fixedDeltaTime * TailRotationSpeed));
            node.MovePosition(Vector3.MoveTowards(node.position, targetPos, Time.fixedDeltaTime * TailMovementSpeed));

            prevNode = node.transform;
        }
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
        Gizmos.DrawSphere(XDistToPathPos(transform.position.x + FORWARD_DISTANCE), 0.5f);
    }

    [Button]
    public void TestFeed()
    {
        currentFeed += 1.0f;
        currentFeed = Mathf.Clamp(currentFeed, 0.0f, MaxFeed);
    }
}