using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Display Combat Popup", story: "Display Popup [Text] for [Seconds] Seconds", category: "Action", id: "5d6c0bdd7f098412788ac515e9a3f85d")]
public partial class DisplayCombatPopupAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Text;
    [SerializeReference] public BlackboardVariable<float> Seconds;

    protected override Status OnStart()
    {
        CombatCanvas canvas = CombatCanvas.Instance;

        if (canvas == null)
            return Status.Failure;

        canvas.SetPopup(Text, Seconds);

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

