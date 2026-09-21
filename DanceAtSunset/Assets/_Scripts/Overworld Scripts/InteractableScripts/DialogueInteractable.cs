using System.Collections;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    public bool RotateToPlayerOnInteract;
    [ShowIf(nameof(RotateToPlayerOnInteract))] public AnimationCurve RotationCurve;
    [ShowIf(nameof(RotateToPlayerOnInteract))] public float RotationDuration;
    private Quaternion defaultRotation;

    public string InteractionPrompt => "Interact to talk to them";
    [SerializeField] private DialogueData dialogueData;

    private void Awake()
    {
        defaultRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        EndDialogueAction.OnDialogueEnded += OnInteractEnd;
    }

    private void OnDisable()
    {
        EndDialogueAction.OnDialogueEnded -= OnInteractEnd;
    }

    public void Interact()
    {
        DialogueManager.Instance?.SetDialogue(dialogueData.dialogue);

        if(RotateToPlayerOnInteract)
            StartCoroutine(RotateOnInteract(true));
    }

    private IEnumerator RotateOnInteract(bool rotateTowardsPlayer)
    {
        float progress = 0f;
        Vector3 direction = OverworldManager.Instance.GetPlayer().transform.position - transform.position;
        Quaternion startRotation = rotateTowardsPlayer ? defaultRotation : Quaternion.LookRotation(direction, transform.up);
        Quaternion endRotation = rotateTowardsPlayer ? Quaternion.LookRotation(direction, transform.up) : defaultRotation;

        while (progress < 1f)
        {
            yield return null;

            progress += Time.deltaTime / RotationDuration;

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, RotationCurve.Evaluate(progress));
        }
    }

    private void OnInteractEnd()
    {
        if (RotateToPlayerOnInteract)
            StartCoroutine(RotateOnInteract(false));
    }

    public void InteractRangeEnter()
    {

    }

    public void InteractRangeExit()
    {

    }
}
