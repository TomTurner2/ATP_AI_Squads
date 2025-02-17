﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] Character character_controller;
    [SerializeField] Animator character_animator;


    void Update()
    {
        if (character_controller == null || character_animator == null)
            return;

        SetAnimatorParameters();
    }


    void SetAnimatorParameters()
    {
        character_animator.SetBool("dead", character_controller.dead);
        character_animator.SetFloat("speed", character_controller.current_speed);
        character_animator.SetBool("crouching", character_controller.crouching);
    }

}
