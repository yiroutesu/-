using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    [Header("触发设置")]
    public string playerTag = "Player";
    public bool showDebug = true;

    [Header("触发UI")]
    public GameObject TargetUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (showDebug) Debug.Log("玩家进入触发区域");
            OnPlayerEnter();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // 玩家在触发区域内持续调用
            OnPlayerStay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (showDebug) Debug.Log("玩家离开触发区域");
            OnPlayerExit();
        }
    }

    // 自定义事件方法
    private void OnPlayerEnter()
    {
        if (TargetUI != null)
        {
            TargetUI.SetActive(!TargetUI.activeSelf);
        }
        // 在这里添加进入时的事件
        // 例如：显示UI、播放声音、触发动画等
    }

    private void OnPlayerStay()
    {
        // 玩家在区域内持续触发
        // 例如：持续伤害、充能等
    }

    private void OnPlayerExit()
    {
        if (TargetUI != null)
        {
            TargetUI.SetActive(!TargetUI.activeSelf);
        }
        // 在这里添加离开时的事件
    }
}