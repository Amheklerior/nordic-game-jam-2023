using UnityEngine;

public interface IAttackable
{
    public void OnAttacked();

    public Transform GetTransform();
}
