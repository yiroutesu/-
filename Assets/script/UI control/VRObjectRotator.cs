using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class VRObjectRotator : MonoBehaviour
{
    [Header("Input Settings")]
    public bool useNewInputSystem = true;
    
    [Header("New Input System")]
    public InputActionReference triggerAction;
    public InputActionReference touchpadTouchAction;
    public InputActionReference touchpadPositionAction;
    
    [Header("Legacy VR Input")]
    public XRNode controllerNode = XRNode.RightHand;
    
    [Header("Raycast Settings")]
    public float raycastDistance = 10f;
    public LayerMask interactableLayer = -1;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 2f;
    public bool useLocalRotation = false;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private Transform currentTarget;
    private bool isRotating = false;
    private Vector2 lastTouchPosition;
    private RaycastHit hitInfo;
    
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    
    void OnEnable()
    {
        // 启用新的 Input System 动作
        if (useNewInputSystem)
        {
            triggerAction?.action.Enable();
            touchpadTouchAction?.action.Enable();
            touchpadPositionAction?.action.Enable();
        }
    }
    
    void OnDisable()
    {
        // 禁用新的 Input System 动作
        if (useNewInputSystem)
        {
            triggerAction?.action.Disable();
            touchpadTouchAction?.action.Disable();
            touchpadPositionAction?.action.Disable();
        }
    }
    
    void Update()
    {
        HandleInput();
        HandleRotation();
        UpdateRayVisualization();
    }
    
    void HandleInput()
    {
        bool triggerPressed = false;
        bool touchpadTouched = false;
        Vector2 touchPosition = Vector2.zero;



        if (useNewInputSystem)
        {
            // 使用新的 Input System
            triggerPressed = GetTriggerInputNew();
            touchpadTouched = GetTouchpadTouchNew();
            touchPosition = GetTouchpadPositionNew();
        }
        else
        {
            // 使用传统的 XR 输入
            triggerPressed = GetTriggerInputLegacy();
            touchpadTouched = GetTouchpadTouchLegacy();
            touchPosition = GetTouchpadPositionLegacy();
        }
        
        // 射线检测
        Ray ray = new Ray(transform.position, transform.forward);
        bool hasHit = Physics.Raycast(ray, out hitInfo, raycastDistance, interactableLayer);

        if (triggerPressed && showDebugLogs)
        {

        }

        // 开始旋转
        if (triggerPressed && hasHit && !isRotating)
        {
            StartRotation(hitInfo.transform);
        }
        // 结束旋转
        else if (!triggerPressed && isRotating)
        {
            StopRotation();
        }
        
        // 更新触摸位置
        if (touchpadTouched && isRotating)
        {
                        Debug.Log("HAHAHAH");
            if (lastTouchPosition != Vector2.zero)
            {
                Vector2 delta = touchPosition - lastTouchPosition;
                RotateTarget(delta);

            }
            lastTouchPosition = touchPosition;
        }
        else
        {
            lastTouchPosition = Vector2.zero;
        }
    }
    
    #region Input Methods
    
    // 新的 Input System 输入方法
    private bool GetTriggerInputNew()
    {
        if (triggerAction != null)
            return triggerAction.action.ReadValue<float>() > 0.5f;
        return false;
    }
    
    private bool GetTouchpadTouchNew()
    {
        if (touchpadTouchAction != null)
            return touchpadTouchAction.action.ReadValue<float>() > 0.5f;
        return false;
    }
    
    private Vector2 GetTouchpadPositionNew()
    {
        if (touchpadPositionAction != null)
        {
            Vector2 rawPosition = touchpadPositionAction.action.ReadValue<Vector2>();
            // 将鼠标位置转换为 -1 到 1 的范围
            return new Vector2(
                (rawPosition.x / Screen.width) * 2 - 1,
                (rawPosition.y / Screen.height) * 2 - 1
            );
        }
        return Vector2.zero;
    }
    
    // 传统的 XR 输入方法
    private bool GetTriggerInputLegacy()
    {
        UnityEngine.XR.InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (device.isValid)
        {
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool triggerPressed);
            return triggerPressed;
        }
        return false;
    }
    
    private bool GetTouchpadTouchLegacy()
    {
        UnityEngine.XR.InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (device.isValid)
        {
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisTouch, out bool touchpadTouched);
            return touchpadTouched;
        }
        return false;
    }
    
    private Vector2 GetTouchpadPositionLegacy()
    {
        UnityEngine.XR.InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (device.isValid)
        {
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 touchPosition);
            return touchPosition;
        }
        return Vector2.zero;
    }
    
    #endregion
    
    #region Rotation Methods
    
    void StartRotation(Transform target)
    {
        currentTarget = target;
        isRotating = true;
        
        // 保存初始状态
        initialRotation = useLocalRotation ? currentTarget.localRotation : currentTarget.rotation;
        initialPosition = currentTarget.position;
        
        if (showDebugLogs)
            Debug.Log($"开始旋转: {currentTarget.name}");
    }
    
    void StopRotation()
    {
        isRotating = false;
        currentTarget = null;
        lastTouchPosition = Vector2.zero;
        
        if (showDebugLogs)
            Debug.Log("停止旋转");
    }
    
    void HandleRotation()
    {
        if (!isRotating || currentTarget == null) return;
        
        // 确保位置不变
        currentTarget.position = initialPosition;
    }

private Quaternion totalRotation = Quaternion.identity; // 使用四元数累积

void RotateTarget(Vector2 delta)
{
    if (currentTarget == null) return;
    
    // 计算本次旋转增量（四元数）
    Quaternion deltaRotation = Quaternion.Euler(
        delta.y * rotationSpeed, 
        -delta.x * rotationSpeed, 
        0
    );
    
    // 累积旋转（四元数乘法）
    totalRotation = deltaRotation * totalRotation;
    
    if (useLocalRotation)
    {
        currentTarget.localRotation = initialRotation * totalRotation;
    }
    else
    {
        currentTarget.rotation = initialRotation * totalRotation;
    }
    
    currentTarget.position = initialPosition;
    
}
    
    #endregion
    
    void UpdateRayVisualization()
    {
        // 射线可视化
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, 
                     isRotating ? Color.green : Color.white);
    }
    
    // 在 Inspector 中显示当前状态（仅编辑器中）
    void OnGUI()
    {
        if (showDebugLogs)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"输入系统: {(useNewInputSystem ? "New Input System" : "Legacy XR")}");
            GUI.Label(new Rect(10, 30, 300, 20), $"状态: {(isRotating ? "旋转中 - " + currentTarget?.name : "等待输入")}");
            GUI.Label(new Rect(10, 50, 300, 20), $"使用本地旋转: {useLocalRotation}");
        }
    }
    
    public bool IsRotating()
    {
        return isRotating;
    }
    
    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }
    
    // 公共方法用于外部控制
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = Mathf.Max(0.1f, speed);
    }
    
    public void SetUseLocalRotation(bool useLocal)
    {
        useLocalRotation = useLocal;
    }
    
    public void ForceStopRotation()
    {
        StopRotation();
    }
}