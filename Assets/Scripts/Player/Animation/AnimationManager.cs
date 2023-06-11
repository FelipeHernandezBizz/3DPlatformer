using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator anim;
    int horizontal;
    int vertical;

    private void Awake()
    {
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void PlayerTargetAnimation(string targetAnim)
    {

    }

    public void UpdateAnimValue(float horizontalMovement, float verticalMovement)
    {
        anim.SetFloat(horizontal, horizontalMovement, .1f, Time.deltaTime);
        anim.SetFloat(vertical, verticalMovement, .1f, Time.deltaTime);
    }
}
