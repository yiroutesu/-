using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRRayInteractor : MonoBehaviour
{
    [Header("射线设置")]
    public XRNode controllerNode = XRNode.RightHand;
    public float rayDistance = 10f;
    public LayerMask interactableLayer = -1;
    
    [Header("视觉反馈")]
    public LineRenderer lineRenderer;
    public GameObject reticle;
    
    private InputDevice controllerDevice;
    private Transform currentTarget;
    private bool isSelecting = false;
    
    void Update()
    {
        UpdateRaycast();
        HandleInput();
        UpdateVisuals();
    }
    
    void InitializeVisuals()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = true;
        }
        if (reticle != null)
        {
            reticle.SetActive(false);
        }
    }
    
    void UpdateRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        
        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            currentTarget = hit.transform;
            if (reticle != null)
            {
                reticle.SetActive(true);
                reticle.transform.position = hit.point;
                reticle.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
        }
        else
        {
            currentTarget = null;
            if (reticle != null)
            {
                reticle.SetActive(false);
            }
        }
    }
    
    void HandleInput()
    {
        // 检测触发按钮按下
        if (controllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed))
        {
            if (triggerPressed && currentTarget != null && !isSelecting)
            {
                StartRotation();
            }
            else if (!triggerPressed && isSelecting)
            {
                StopRotation();
            }
        }
        
        // 在旋转过程中更新旋转
        if (isSelecting)
        {
            UpdateRotation();
        }
    }
    
    void UpdateVisuals()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            Vector3 endPoint = transform.position + transform.forward * rayDistance;
            
            if (currentTarget != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, interactableLayer))
                {
                    endPoint = hit.point;
                }
            }
            
            lineRenderer.SetPosition(1, endPoint);
        }
    }

    void UpdateRotation()
    {
        // 旋转逻辑将在下面的脚本中实现
    }
    
        public bool IsSelecting => isSelecting;
    public Transform CurrentTarget => currentTarget;
    
    private VRObjectRotationController rotationController;
    
    void Start()
    {
        rotationController = GetComponent<VRObjectRotationController>();
        controllerDevice = InputDevices.GetDeviceAtXRNode(controllerNode);
        InitializeVisuals();
        // ... 其他初始化 ...
    }
    
    void StartRotation()
    {
        isSelecting = true;
        rotationController.StartObjectRotation(currentTarget);
        Debug.Log("开始旋转物体: " + currentTarget.name);

    }
    
    void StopRotation()
    {
        isSelecting = false;
        rotationController.StopObjectRotation();
        Debug.Log("停止旋转物体");
    }
}