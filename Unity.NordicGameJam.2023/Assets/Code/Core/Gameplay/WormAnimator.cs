using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class WormAnimator : MonoBehaviour
{
    public HingeJoint2D PrefabBodyNode;
    public Rigidbody2D RootRigidbody;

    public int NodesToCreate;

    private void Awake()
    {
        GenerateBody();
    }

    [Button]
    private void GenerateBody()
    {
        var offset        = Vector3.left * 2.0f;
        var instPos       = RootRigidbody.transform.position;
        var prevRigidbody = RootRigidbody;
        for (int i = 0; i < NodesToCreate; i++)
        {
            instPos += Quaternion.Euler(0.0f, 0.0f, Random.Range(-30.0f, 30.0f)) * offset;
            var node = Instantiate(PrefabBodyNode, instPos, Quaternion.identity);
            node.anchor = Vector2.right;

            var posOffset = prevRigidbody.transform.position + prevRigidbody.transform.rotation * Vector3.left - node.transform.position;
            node.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(posOffset.y, posOffset.x) * Mathf.Rad2Deg);
            node.connectedBody = prevRigidbody;

            node.GetComponent<DistanceJoint2D>().autoConfigureDistance = false;
            node.GetComponent<DistanceJoint2D>().connectedBody = prevRigidbody;
            prevRigidbody = node.GetComponent<Rigidbody2D>();
        }
    }
}