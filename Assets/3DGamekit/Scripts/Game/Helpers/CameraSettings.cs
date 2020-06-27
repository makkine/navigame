﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;


public interface CameraMessageTarget : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void FreezeInput();
    void UnfreezeInput();
}


public class CameraSettings : MonoBehaviour, CameraMessageTarget
{
    public enum InputChoice
    {
        KeyboardAndMouse, Controller,
    }
    
    [Serializable]
    public struct InvertSettings
    {
        public bool invertX;
        public bool invertY;
    }


    public Transform follow;
    public Transform lookAt;
    public CinemachineFreeLook keyboardAndMouseCamera;
    public CinemachineFreeLook controllerCamera;
    public InputChoice inputChoice;
    public InvertSettings keyboardAndMouseInvertSettings;
    public InvertSettings controllerInvertSettings;
    public bool allowRuntimeCameraSettingsChanges;

    public CinemachineFreeLook Current
    {
        get { return inputChoice == InputChoice.KeyboardAndMouse ? keyboardAndMouseCamera : controllerCamera; }
    }
    
    void Reset()
    {
        Transform keyboardAndMouseCameraTransform = transform.Find("KeyboardAndMouseFreeLookRig");
        if (keyboardAndMouseCameraTransform != null)
            keyboardAndMouseCamera = keyboardAndMouseCameraTransform.GetComponent<CinemachineFreeLook>();

        Transform controllerCameraTransform = transform.Find("ControllerFreeLookRig");
        if (controllerCameraTransform != null)
            controllerCamera = controllerCameraTransform.GetComponent<CinemachineFreeLook>();
        
        PController playerController = FindObjectOfType<PController>();
        if (playerController != null && playerController.name == "Ellen")
        {
            follow = playerController.transform;

            lookAt = follow.Find("HeadTarget");

            if (playerController.cameraSettings == null)
                playerController.cameraSettings = this;
        }
    }

    void Awake()
    {
        UpdateCameraSettings();
    }

    void Update()
    {
        if (allowRuntimeCameraSettingsChanges)
        {
            UpdateCameraSettings();
        }
    }

    void UpdateCameraSettings()
    {
        keyboardAndMouseCamera.Follow = follow;
        keyboardAndMouseCamera.LookAt = lookAt;
        keyboardAndMouseCamera.m_XAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertX;
        keyboardAndMouseCamera.m_YAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertY;

        controllerCamera.m_XAxis.m_InvertInput = controllerInvertSettings.invertX;
        controllerCamera.m_YAxis.m_InvertInput = controllerInvertSettings.invertY;
        controllerCamera.Follow = follow;
        controllerCamera.LookAt = lookAt;

        keyboardAndMouseCamera.Priority = inputChoice == InputChoice.KeyboardAndMouse ? 1 : 0;
        controllerCamera.Priority = inputChoice == InputChoice.Controller ? 1 : 0;
    }

    public void FreezeInput()
    {
        keyboardAndMouseCamera.m_XAxis.m_InputAxisName = "";
        keyboardAndMouseCamera.m_YAxis.m_InputAxisName = "";
        controllerCamera.m_XAxis.m_InputAxisName = "";
        controllerCamera.m_YAxis.m_InputAxisName = "";
    }

    public void UnfreezeInput() //DO NOT CHANGE THIS OR THE CAMERA WILL NEVER BE USEABLE! 
    {
        keyboardAndMouseCamera.m_XAxis.m_InputAxisName = "CameraX";
        keyboardAndMouseCamera.m_YAxis.m_InputAxisName = "CameraY";
        controllerCamera.m_XAxis.m_InputAxisName = "CameraX";
        controllerCamera.m_YAxis.m_InputAxisName = "CameraY";
    }
} 

