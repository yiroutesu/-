using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

enum rotateCenter
{
    X,
    Y,
    Z
}
enum interactorDirection
{
    X,
    Y,
    Z
}
public class RotatePuzzle : MonoBehaviour
{
    [SerializeField] public XRSimpleInteractable simpleInteractable;
    [SerializeField] private rotateCenter rotatePole = rotateCenter.Y;//绕哪个轴旋转
    [SerializeField] public float rotatescale = 1.0f;//控制旋转敏感度的
    
    private bool isGrabbed = false;
    private Vector3 grabStartPosition;
    private Quaternion grabStartRotation;
    private XRBaseInteractor currentInteractor;
    public Vector3 angle;
    
    public void _int(){
        simpleInteractable.selectEntered.AddListener(OnGrab);
        simpleInteractable.selectExited.AddListener(OnRelease);
    }
    
    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        currentInteractor = args.interactorObject as XRBaseInteractor;
        
        if (currentInteractor != null)
        {
            grabStartPosition = currentInteractor.transform.localPosition;
            grabStartRotation = transform.rotation;
        }
    }
    
    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        currentInteractor = null;
    }
    
    void FixedUpdate()
    {
        if (isGrabbed && currentInteractor != null)
        {
            // 只更新旋转，位置保持不变
            Quaternion newRotation = CalculateRotationFromControllerMovement();
            transform.rotation = newRotation;
        }
        checkRotation();

    }
    private void checkRotation()
    {
        angle = transform.eulerAngles;
    }
    
    private Quaternion CalculateRotationFromControllerMovement()
    {
        // 获取控制器当前位置
        Vector3 currentControllerPos = currentInteractor.transform.localPosition;
        // 计算控制器在抓取开始后相对于物体的移动角度
        Vector3 currentVector = currentControllerPos - transform.position;
        Vector3 perVector = grabStartPosition - transform.position;

        float rotationAngle;

        // 应用旋转（只围绕Y轴）
        Quaternion targetRotation;
        switch (rotatePole)
        {
            case(rotateCenter.X):
                rotationAngle = Vector3.SignedAngle(perVector,currentVector,Vector3.left)*rotatescale;
                targetRotation = grabStartRotation * Quaternion.Euler(rotationAngle, 0, 0);
                break;
            case(rotateCenter.Y):
                rotationAngle = Vector3.SignedAngle(perVector,currentVector,Vector3.up)*rotatescale;
                targetRotation = grabStartRotation * Quaternion.Euler(0, rotationAngle, 0);
                break;
            case(rotateCenter.Z):
                rotationAngle = Vector3.SignedAngle(perVector,currentVector,Vector3.forward)*rotatescale;
                targetRotation = grabStartRotation * Quaternion.Euler(0, 0, rotationAngle);
                break;
            default:
                rotationAngle = Vector3.SignedAngle(perVector,currentVector,Vector3.up)*rotatescale;
                targetRotation = grabStartRotation * Quaternion.Euler(0, rotationAngle, 0);
                break;
        }
        
        return targetRotation;
    }
}