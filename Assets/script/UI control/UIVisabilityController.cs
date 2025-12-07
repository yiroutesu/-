using UnityEngine;
using UnityEngine.UI;

public class UIVisabilityController : MonoBehaviour
{
    [Header("控制对象")]
    public GameObject targetObject; // 拖拽你要控制的对象到这里
    public Button controlButton;    // 拖拽你的按钮到这里

    void Start()
    {
        // 添加按钮点击监听
        controlButton.onClick.AddListener(ToggleVisibility);
    }

    void ToggleVisibility()
    {
        // 切换对象的激活状态（显示/隐藏）
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }

    // 可选：在销毁时移除监听
    void OnDestroy()
    {
        if (controlButton != null)
            controlButton.onClick.RemoveListener(ToggleVisibility);
    }
}