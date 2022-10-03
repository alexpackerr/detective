using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CustomText : DialogueViewBase
{
    [SerializeField] private float appearanceTime = 0.5f;
    [SerializeField] private float disappearanceTime = 0.5f;
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] RectTransform container;
    [SerializeField] private bool waitForInput;
    Coroutine currentAnimation;
    Action advanceHandler = null;

    private float Scale
    {
        set => container.localScale = new Vector3(value, value, value);
    }

    // Start is called before the first frame update
    public void Start()
    {
        Scale = 0;
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (gameObject.activeInHierarchy == false)
        {
            onDialogueLineFinished();
            return;
        }
        Debug.Log($"{this.name} running line {dialogueLine.TextID}");

        Scale = 0;
        text.text = dialogueLine.Text.Text;

        advanceHandler = requestInterrupt;

        currentAnimation = this.Tween(
            0f, 1f,
            appearanceTime,
            (from, to, t) => Scale = Mathf.Lerp(from, to, t),
            () =>
            {
                Debug.Log($"{this.name} finished presenting {dialogueLine.TextID}");
                currentAnimation = null;

                if (waitForInput)
                {
                    advanceHandler = requestInterrupt;
                }
                else
                {
                    advanceHandler = null;
                    onDialogueLineFinished();
                }
            }
            );

    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (gameObject.activeInHierarchy == false)
        {
            onDialogueLineFinished();
            return;
        }
        advanceHandler = null;
        Debug.Log($"{this.name} was interrupted while presenting {dialogueLine.TextID}");

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        Scale = 1f;

        onDialogueLineFinished();
    }

    public override void DismissLine(Action onDismissalComplete)
    {
        if (gameObject.activeInHierarchy == false)
        {
            // This line view isn't active; it should immediately report that
            // it's finished dismissing.
            onDismissalComplete();
            return;
        }

        Debug.Log($"{this.name} dismissing line");

        // If we have an animation running, stop it.
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        advanceHandler = () =>
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
            advanceHandler = null;
            onDismissalComplete();
            Scale = 0f;
        };

        currentAnimation = this.Tween(
            1f, 0f,
            disappearanceTime,
            (from, to, t) => Scale = Mathf.Lerp(from, to, t),
            () =>
            {
                // We're done animating! Signal that we're done.
                advanceHandler = null;
                Debug.Log($"{this.name} finished dismissing line");
                currentAnimation = null;
                onDismissalComplete();
            });
    }

    public override void UserRequestedViewAdvancement()
    {
        // We have received a signal that the user wants to proceed to the next
        // line.

        // Invoke our 'advance line' handler, which (depending on what we're
        // currently doing) will be a signal to interrupt the line, stop the
        // current animation, or do nothing.
        advanceHandler?.Invoke();
    }
}

public static class TweenExtensions
{

    public delegate void TweenFunction<T>(T from, T to, float factor);
    public static Coroutine Tween<T>(this MonoBehaviour behaviour, T from, T to, float time, TweenFunction<T> handler, System.Action onComplete = null)
    {
        return behaviour.StartCoroutine(HandleTween(from, to, time, handler, onComplete));
    }

    private static IEnumerator HandleTween<T>(T from, T to, float time, TweenFunction<T> handler, System.Action onComplete)
    {
        // How much time has elapsed since we started this change.
        var timeElapsed = 0f;

        // Loop until 'time' seconds have elapsed.
        while (timeElapsed < time)
        {

            // Our handler function expects the 'factor' parameter to be between
            // zero and one, where zero is the start of the animation and one is
            // the end of the animation.
            //
            // We'll get this value by dividing timeElapsed by total time, and
            // then clamping the result to between 0 and 1.
            float factor = Mathf.Clamp01(timeElapsed / time);

            // We have everything we need - call the handler function to perform
            // whatever specific work we need to do.
            handler(from, to, factor);

            // Update our elapsed time.
            timeElapsed += Time.deltaTime;

            // Yield the coroutine so that other game work can be done. (This is
            // very important! If you forget this, Unity will freeze up.)
            yield return null;
        }

        // We've reached the end of our animation. Tidy up by calling the
        // handler one last time, with a value of 1. (It's very unlikely that
        // the 'factor' variable we calcuate above would reach an even value of
        // 1.0, and certain animations would look strange if they end early,
        // like an object's transparency.) To deal with this, we'll call it one
        // last time with the final value.

        handler(from, to, 1f);

        // Finally, if we had an on-complete method to call, call it now.
        onComplete?.Invoke();
    }
}
