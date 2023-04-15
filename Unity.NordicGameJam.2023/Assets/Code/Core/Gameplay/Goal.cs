using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public Action<TeamDefinitions> GoalHit;
    private bool _goalReached = false;
    
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer != GameConstants.WORM_LAYER || _goalReached) return;
        var worm = col.gameObject.GetComponent<Worm>();
        GoalHit?.Invoke(worm.WormTeam);
        _goalReached = true;
        Debug.LogWarning("GOAL!");
    }
}
