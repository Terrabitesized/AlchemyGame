using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Limited Combat Popup", story: "Display Popup [Text] for [Seconds] Seconds a Max of [Count] Times", category: "Action", id: "467021b29f7c0fc4be1c8f10274edd72")]
public partial class LimitedCombatPopupAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Text;
    [SerializeReference] public BlackboardVariable<float> Seconds;
    [SerializeReference] public BlackboardVariable<int> Count;

    private int currentUses = 0;

    protected override Status OnStart()
    {
        // Check if we have enough uses left
        if (currentUses >= Count.Value)
            return Status.Success;

        CombatCanvas canvas = CombatCanvas.Instance;

        if (canvas == null)
            return Status.Failure;

        canvas.SetPopup(Text, Seconds);
        currentUses++;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

