using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DisableInputMaps : MonoBehaviour
{
    [Header("需要禁用的映射")]
    public InputActionReference disableMaps;

    [Header("需要禁用的Object")]
    public GameObject disableObject;

    [Header("需要测试的映射")]
    public InputActionReference DebugMaps;

    [Header("Trigger映射")]
    public InputActionReference TriggerAction;

    [Header("射线交互设置")]
    public float raycastDistance = 10f;
    public LayerMask interactableLayer = -1;

    [Header("旋转速度")]
    public float rotationSpeed = 4f;
    public bool useLocalRotation = false;

    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private Transform currentTarget;
    private bool isRotating = false;
    private Vector2 lastTouchPosition;
    private RaycastHit hitInfo;
    
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    // 添加可旋转物体的Layer
    private int rotatableLayer;
    
    void Start()
    {
        // 获取可旋转物体的Layer
        rotatableLayer = LayerMask.NameToLayer("ObjectRotatable");
        
        if (rotatableLayer == -1)
        {
            Debug.LogError("未找到 'ObjectRotatable' 层，请检查层设置");
        }
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        bool hasHit = Physics.Raycast(ray, out hitInfo, raycastDistance, interactableLayer);
        
        // 简化射线颜色判断逻辑
        if (hasHit && hitInfo.collider.gameObject.layer == rotatableLayer)
        {
            Debug.DrawRay(transform.position, transform.forward * hitInfo.distance, Color.green);
            if (showDebugLogs)
                Debug.Log("绿色射线！击中: " + hitInfo.collider.gameObject.name);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.white);
        }

        DealingRotating();
    }

    private void DealingRotating()
    {
        bool triggerPressed = GetTriggerInput();

        // 射线检测
        Ray ray = new Ray(transform.position, transform.forward);
        bool hasHit = Physics.Raycast(ray, out hitInfo, raycastDistance, interactableLayer);
        
        if (!hasHit)
        {
            // 如果没有击中任何物体且正在旋转，结束旋转
            if (isRotating)
            {
                disableMaps.action.Enable();
                EndRotating();
            }
            return;
        }

        // 使用Layer比较而不是字符串比较
        if (hitInfo.collider.gameObject.layer != rotatableLayer)
        {
            // 如果击中的不是可旋转物体且正在旋转，结束旋转
            if (isRotating)
            {
                disableMaps.action.Enable();
                EndRotating();
            }
            return;
        }

        if (showDebugLogs)
            Debug.Log("检测到可旋转物体: " + hitInfo.collider.gameObject.name);

        if (triggerPressed && !isRotating) // 开始旋转
        {
            disableObject.GameObject().SetActive(false);
            disableMaps.action.Disable();
            StartRotating(hitInfo.transform);
        }
        else if (!triggerPressed && isRotating) // 结束旋转
        {
            disableObject.GameObject().SetActive(true);
            disableMaps.action.Enable();
            EndRotating();
        }
        
        Vector2 stickInput = DebugMaps.action.ReadValue<Vector2>();

        Vector3 objectCenter = hitInfo.collider.gameObject.GetComponent<Renderer>().bounds.center;

        // 更新触摸位置
        if ((stickInput.x != 0 || stickInput.y != 0) && isRotating)
        {
            if (stickInput.x != 0)
            {
                if (showDebugLogs)
                    Debug.Log("X轴旋转输入: " + stickInput.x);
                RotateTarget(objectCenter,Vector3.up,stickInput.x);
            }
            if (stickInput.y != 0)
            {
                if (showDebugLogs)
                    Debug.Log("Y轴旋转输入: " + stickInput.y);
                RotateTarget(objectCenter,Vector3.right,stickInput.y);
            }
        }
    }

    private void RotateTarget(Vector3 rotateCenter,Vector3 rotateAxis,float y)
    {
        if (currentTarget == null)
        {
            Debug.LogWarning("未检测到可旋转物体");
            return;
        }

        if (useLocalRotation)
        {
            currentTarget.RotateAround(rotateCenter,rotateAxis,y * rotationSpeed);
        }
        else
        {
            currentTarget.RotateAround(rotateCenter,rotateAxis,y * rotationSpeed);
        }
    }



    private void EndRotating()
    {
        isRotating = false;
        currentTarget = null;
        
        if (showDebugLogs)
            Debug.Log("停止旋转");
    }

    private void StartRotating(Transform targetTransform)
    {
        currentTarget = targetTransform;
        isRotating = true;

        if (showDebugLogs)
            Debug.Log($"开始旋转: {currentTarget.name}");
    }

    private bool GetTriggerInput()
    {
        if (TriggerAction != null)
            return TriggerAction.action.ReadValue<float>() > 0.5f;
        return false;
    }
}