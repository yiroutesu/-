
using UnityEngine;
using UnityEngine.UI; // 重要：需要引入 UI 命名空间来访问 Slider

public class SliderRotationController : MonoBehaviour
{
    [Header("关联对象")]
    public Slider rotationSlider; // 拖拽你的 Slider 对象到这里
    public GameObject targetObject; // 拖拽你想要旋转的 GameObject 到这里

    [Header("旋转范围")]
    public float minRotationZ = 0f;
    public float maxRotationZ = 360f;

    void Start()
    {
        // 如果 Slider 被赋值了，在开始时为其添加监听事件
        if (rotationSlider != null)
        {
            // 设置 Slider 的最小最大值
            rotationSlider.minValue = minRotationZ;
            rotationSlider.maxValue = maxRotationZ;
            
            // 添加监听：当 Slider 值改变时，调用 ChangeRotation 方法
            rotationSlider.onValueChanged.AddListener(ChangeRotation);
            
            // 初始化一次旋转，确保对象起始状态正确
            ChangeRotation(rotationSlider.value);
        }
    }

    // 这个方法会在 Slider 值改变时自动被调用
    void ChangeRotation(float sliderValue)
    {
        // 检查目标对象是否存在
        if (targetObject != null)
        {
            // 获取对象当前的旋转角度
            Vector3 currentRotation = targetObject.transform.eulerAngles;
            // 只改变 Z 轴的角度，X 和 Y 轴保持不变
            currentRotation.y = sliderValue;
            // 将新的旋转角度应用回对象
            targetObject.transform.eulerAngles = currentRotation;
        }
    }

    // 可选：当脚本被禁用或对象被销毁时，移除监听以防止内存泄漏
    void OnDestroy()
    {
        if (rotationSlider != null)
            rotationSlider.onValueChanged.RemoveListener(ChangeRotation);
    }
}