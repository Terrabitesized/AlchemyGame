using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PlayerWithinDistance", story: "Player within [Distance] units", category: "Conditions", id: "58d1c129584cb0834c536ba064f1f757")]
public partial class PlayerWithinDistanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> Distance;
    private Transform PlayerTransform;

    public override bool IsTrue()
    {
        if (PlayerTransform == null)
            return false;

        float distanceToPlayer = Vector3.Distance(
            GameObject.transform.position,
            PlayerTransform.position
        );

        return distanceToPlayer <= Distance.Value;
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
