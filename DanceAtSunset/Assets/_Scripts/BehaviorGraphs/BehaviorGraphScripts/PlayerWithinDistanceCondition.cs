using System;
using Unity.Behavior;
using UnityEngine;

public enum DistanceComparison
{
    CloserThan,
    FurtherThan,
}


[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PlayerDistance", story: "Player distance [Comparison] [Distance] units", category: "Conditions", id: "58d1c129584cb0834c536ba064f1f757")]
public partial class PlayerWithinDistanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<DistanceComparison> Comparison;
    private Transform PlayerTransform;

    public override bool IsTrue()
    {
        if (PlayerTransform == null)
            return false;

        float distanceToPlayer = Vector3.Distance(
            GameObject.transform.position,
            PlayerTransform.position
        );

        switch (Comparison.Value)
        {
            case DistanceComparison.CloserThan:
                return distanceToPlayer < Distance.Value;

            case DistanceComparison.FurtherThan:
                return distanceToPlayer > Distance.Value;

            default:
                return false;
        }
    }

    public override void OnStart()
    {
        // Find the player when the condition starts.
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            PlayerTransform = player.transform;
    }

    public override void OnEnd()
    {
    }
}
