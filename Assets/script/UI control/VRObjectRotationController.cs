using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(VRRayInteractor))]
public class VRObjectRotationController : MonoBehaviour
{
    [Header("旋转设置")]
    public float rotationSpeed = 2f;
    public bool useWorldSpace = true;
    
    private VRRayInteractor rayInteractor;
    private InputDevice controllerDevice;
    private Vector3 lastControllerPosition;
    private Quaternion lastControllerRotation;
    private Transform rotatingObject;
    
    void Start()
    {
        rayInteractor = GetComponent<VRRayInteractor>();
        controllerDevice = InputDevices.GetDeviceAtXRNode(rayInteractor.controllerNode);
    }
    
    void Update()
    {
        if (rayInteractor.IsSelecting) // 需要在VRRayInteractor中暴露这个属性
        {
            UpdateObjectRotation();
        }
    }
    
    public void StartObjectRotation(Transform targetObject)
    {
        rotatingObject = targetObject;
        
        // 记录初始控制器状态
        if (controllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
        {
            lastControllerPosition = position;
        }
        if (controllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
        {
            lastControllerRotation = rotation;
        }
    }
    
    public void StopObjectRotation()
    {
        rotatingObject = null;
    }
    
    void UpdateObjectRotation()
    {
        if (rotatingObject == null) return;
        
        // 获取当前控制器状态
        if (controllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 currentPosition) &&
            controllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion currentRotation))
        {
            // 计算控制器的位移和旋转变化
            Vector3 positionDelta = currentPosition - lastControllerPosition;
            Quaternion rotationDelta = currentRotation * Quaternion.Inverse(lastControllerRotation);
            
            // 应用旋转到物体
            ApplyRotation(positionDelta, rotationDelta);
            
            // 更新记录的状态
            lastControllerPosition = currentPosition;
            lastControllerRotation = currentRotation;
        }
    }
    
    void ApplyRotation(Vector3 positionDelta, Quaternion rotationDelta)
    {
        if (useWorldSpace)
        {
            // 世界空间旋转
            rotatingObject.rotation = rotationDelta * rotatingObject.rotation;
        }
        else
        {
            // 本地空间旋转
            rotatingObject.localRotation = rotationDelta * rotatingObject.localRotation;
        }
        
        // 或者使用位置delta来控制旋转（类似划动手势）
        // float rotationX = positionDelta.y * rotationSpeed * 100f;
        // float rotationY = -positionDelta.x * rotationSpeed * 100f;
        // rotatingObject.Rotate(rotationX, rotationY, 0, useWorldSpace ? Space.World : Space.Self);
    }
}