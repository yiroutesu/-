using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GotoGameLevel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string targetSceneName; // 目标场景名称
    [SerializeField] private float maxDistance = 10f; // 最大检测距离
    
    [Header("Detectable Object")]
    [SerializeField] private GameObject detectableObject; // 可被检测的物体
    [SerializeField] private LayerMask objectLayerMask = -1; // 物体所在的层

    [Header("References")]
    [SerializeField] private InputActionReference leftTriggerAction; // 左手柄Trigger输入
    [SerializeField] private Transform leftControllerTransform; // 左手柄变换组件

    [Header("Visual Feedback (Optional)")]
    [SerializeField] private GameObject reticle; // 可选的瞄准准星

    private bool isAimingAtObject = false;

    void Start()
    {
        // 如果没有指定左手柄变换，尝试使用主相机（VR中通常是头显）
        if (leftControllerTransform == null)
        {
            leftControllerTransform = Camera.main?.transform;
        }

        // 启用输入Action
        leftTriggerAction.action.Enable();
    }

    void Update()
    {
        CheckAimingAtObject();
        CheckTriggerInput();
        UpdateVisualFeedback();
    }

    /// <summary>
    /// 检查是否对准可检测物体
    /// </summary>
    private void CheckAimingAtObject()
    {
        if (leftControllerTransform == null || detectableObject == null) return;

        Ray ray = new Ray(leftControllerTransform.position, leftControllerTransform.forward);
        RaycastHit hit;

        bool wasAiming = isAimingAtObject;
        isAimingAtObject = false;

        if (Physics.Raycast(ray, out hit, maxDistance, objectLayerMask))
        {
            // 检查击中的物体是否是可检测物体或其子物体
            if (hit.collider.gameObject == detectableObject || 
                hit.collider.transform.IsChildOf(detectableObject.transform))
            {
                isAimingAtObject = true;
            }
        }

        // 只在状态改变时输出日志
        if (isAimingAtObject && !wasAiming)
        {
            Debug.Log("检测到目标物体");
        }
        else if (!isAimingAtObject && wasAiming)
        {
            Debug.Log("未检测到目标物体");
        }
    }

    /// <summary>
    /// 检查Trigger输入
    /// </summary>
    private void CheckTriggerInput()
    {
        if (isAimingAtObject && leftTriggerAction.action.ReadValue<float>() > 0.5f)
        {
            Debug.Log("触发目标物体，加载场景");
            LoadTargetScene();
        }
    }

    /// <summary>
    /// 更新视觉反馈
    /// </summary>
    private void UpdateVisualFeedback()
    {
        if (reticle != null)
        {
            reticle.SetActive(isAimingAtObject);
        }
    }

    /// <summary>
    /// 加载目标场景
    /// </summary>
    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("目标场景名称未设置！");
        }
    }

    void OnDisable()
    {
        // 禁用输入Action
        leftTriggerAction.action.Disable();
    }

    void OnDrawGizmosSelected()
    {
        // 在场景视图中绘制调试射线
        if (leftControllerTransform != null)
        {
            Gizmos.color = isAimingAtObject ? Color.green : Color.red;
            Gizmos.DrawRay(leftControllerTransform.position, leftControllerTransform.forward * maxDistance);
        }
    }
}