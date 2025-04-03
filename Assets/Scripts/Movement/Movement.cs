using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    [Serializable]
    public enum CharacterStates
    {
        Idle = 0,
        Walk,
        Jump,
        Shoot
    }
    [SerializeField] private CharacterStates m_currentState;

    [Header("Main Components")]
    [SerializeField] private PlayerControls m_controls;
    private CharacterController m_controller;

    [Header("Values")]
    [SerializeField] private float m_speed = 5.0f;
    [SerializeField] private float m_YVelocity = 0.0f;
    private float m_moveInput = 0.0f;

    private void Awake()
    {
        m_controls = new PlayerControls();
        m_controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        m_controls.Enable();
        m_controls.Player.Move.performed += ctx => m_moveInput = ctx.ReadValue<float>();
        m_controls.Player.Move.canceled += ctx => m_moveInput = 0;
    }

    private void OnDisable()
    {
        m_controls.Disable();
    }

    private void Update()
    {
        MovementUpdate();
    }

    private void MovementUpdate()
    {
        Vector3 playerPos = new Vector3(m_moveInput * m_speed * Time.deltaTime, m_YVelocity, 0);
        m_controller.Move(playerPos);
    }

}
