using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PlayerFurtherThanDistance", story: "Player further than [distance] units", category: "Conditions", id: "d01861dc23760f581ed0c53f961df39b")]
public partial class PlayerFurtherThanDistanceCondition : Condition
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

        return distanceToPlayer >= Distance.Value;
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
