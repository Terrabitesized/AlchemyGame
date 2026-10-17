using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "Player Distance",
    story: "Player distance [Comparison] [Distance] units",
    category: "Conditions",
    id: "58d1c129584cb0834c536ba064f1f757")]
public partial class PlayerDistanceCondition : Condition
{
    [SerializeReference]
    public BlackboardVariable<float> Distance;

    [SerializeReference]
    public BlackboardVariable<DistanceComparison> Comparison;

    private Transform PlayerTransform;

    public override void OnStart()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        PlayerTransform = player != null ? player.transform : null;
    }

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

    public override void OnEnd()
    {
    }
}