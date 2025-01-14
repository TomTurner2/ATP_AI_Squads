﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct KeyEvent
{
    public KeyCode key;
    public UnityEvent on_press_event;
}


public class PlayerController : MonoBehaviour
{
    [SerializeField] SquadCommander current_commander;
    [SerializeField] Camera first_person_camera;

    [SerializeField] CustomEvents.Vector3Event update_look_target;
    [SerializeField] CustomEvents.Vector3Event on_move_event;
    [SerializeField] CustomEvents.BooleanEvent on_sprint_event;
    [SerializeField] CustomEvents.BooleanEvent on_command_event;
    [SerializeField] CustomEvents.BooleanEvent on_fire_held_event;
    [SerializeField] UnityEvent on_jump_event;
    [SerializeField] UnityEvent on_issue_command_event;

    [SerializeField] List<KeyEvent> key_events = new List<KeyEvent>();


    void Update ()
	{
	    FirstPersonControls();
	}


    private void FirstPersonControls()
    {
        AxisEvents();
        ButtonPressEvents();
        ButtonHeldEvents();
    }


    void LateUpdate()
    {
        CheckLookTarget();
    }


    private void CheckLookTarget()
    {
        RaycastHit hit;
        Ray ray = first_person_camera.ScreenPointToRay(new Vector3(Screen.width, Screen.height) * 0.5f);

        if (Physics.Raycast(ray, out hit))
            update_look_target.Invoke(hit.point);
    }


    private void AxisEvents()
    {
        Vector3 move_input = new Vector3(Input.GetAxisRaw("Right"), -Input.GetAxisRaw("Forward"));
        on_move_event.Invoke(move_input.normalized);
    }


    private void ButtonHeldEvents()
    {
        on_sprint_event.Invoke(Input.GetButton("Sprint"));
        on_command_event.Invoke(Input.GetButton("IndicateCommand"));
        on_fire_held_event.Invoke(Input.GetButton("Fire1"));
    }


    private void ButtonPressEvents()
    {
        if (Input.GetButtonDown("Jump"))
            on_jump_event.Invoke();

        if (Input.GetButton("IssueCommand"))
            on_issue_command_event.Invoke();

        foreach (KeyEvent key in key_events)
        {
            if (Input.GetKeyUp(key.key))
                key.on_press_event.Invoke();
        }
    }
}