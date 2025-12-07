using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGravity : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 玩家移动速度
    public float gravity = -9.81f; // 重力加速度
    public float groundDistance = 0.1f; // 检测地面的距离
    public LayerMask groundMask; // 地面层
    public Transform groundCheck; // 地面检测点

    private float yVelocity; // 玩家的垂直速度
    private Vector3 velocity; // 玩家的总速度
    private bool isGrounded; // 是否站在地面上

    private void Start()
    {
        groundCheck = transform;
    }

    private void Update()
    {
        // 检测是否站在地面上
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 如果站在地面上，重置垂直速度
        if (isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }

        // 玩家的垂直速度更新
        yVelocity += gravity * Time.deltaTime;
        velocity.y = yVelocity;

        // 玩家的水平移动
        float x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        velocity.x = x;
        velocity.z = z;

        // 应用速度
        transform.position += velocity;
    }
}