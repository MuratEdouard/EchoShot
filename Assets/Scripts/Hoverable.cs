using UnityEngine;

public class Hoverable : MonoBehaviour
{
    public Animator animator;
    public Renderer renderer;

    void Start()
    {
        OnHoverEnter();
        OnHoverExit();
    }

    public void OnHoverEnter()
    {
        animator.Play("Idle");
        animator.enabled = true;
        renderer.material.EnableKeyword("_EMISSION");
    }

    public void OnHoverExit()
    {
        animator.enabled = false;
        renderer.material.DisableKeyword("_EMISSION");
    }
}
