using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class chessPuzzle : XRSocketInteractor
{
    [SerializeField] public GameObject correctPiece; // 正确的棋子
    [SerializeField] private Material normalMaterial; // 正常材质
    [SerializeField] private Material hoverMaterial; // 悬停材质
    [SerializeField] private Material errorMaterial; // 错误材质
    
    [Header("悬停控制")]
    public float maxHoverDistance = 0.1f;
    
    private MeshRenderer socketRenderer;
    private bool hasCorrectPieceHover = false;
    public bool isRight = false;
    private bool hasselect = false;
    
    protected override void Start()
    {
        base.Start();
        socketRenderer = GetComponent<MeshRenderer>();
        showInteractableHoverMeshes = false;
        // 确保渲染器初始状态正确
        if (socketRenderer != null)
        {
            socketRenderer.material = normalMaterial;
            socketRenderer.enabled = true; // 确保渲染器启用
        }
    }
    
    // 重写悬停检查方法
    public override bool CanHover(IXRHoverInteractable interactable)
    {
        // 检查距离
        if(hasselect){return false;}
        if(interactable.transform.GetComponent<puzzleParts>().isselected){return false;}
        float distance = Vector3.Distance(interactable.transform.position, transform.position);
        if (distance >= maxHoverDistance&&interactable.transform.name != correctPiece.transform.name)
            return false;
            
        return base.CanHover(interactable);
    }
    
    // 重写交互检查方法
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        // 只有正确的棋子才能放置
        if(hasselect){return false;}
        float distance = Vector3.Distance(interactable.transform.position, transform.position);
        if (interactable.transform.name != correctPiece.transform.name&&distance <= maxHoverDistance)
            return false;
            
        return base.CanSelect(interactable);
    }
    
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        
        print($"Hover Entered: {args.interactableObject.transform.name}");
        if(hasselect){return;}
        if(args.interactableObject.transform.GetComponent<puzzleParts>().isselected){return;}
        if (socketRenderer != null&&!hasselect&&args.interactableObject.transform.name == correctPiece.transform.name)
        {
            // 确保渲染器启用
            socketRenderer.enabled = true;
            
            // 检查是否是正确棋子
            if (args.interactableObject.transform.name == correctPiece.transform.name)
            {
                socketRenderer.material = hoverMaterial;
                hasCorrectPieceHover = true;
                Debug.Log("Correct piece hovering");
            }
            else
            {
                socketRenderer.material = errorMaterial;
                hasCorrectPieceHover = false;
                Debug.Log("Wrong piece hovering");
            }
        }
        else
        {
            socketRenderer.material = errorMaterial;
            socketRenderer.enabled = true;
        }
    }
    
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        
        print("Hover Exited");
        
        if (socketRenderer != null)
        {
            // 只有在没有放置物体时才恢复正常材质
            if (!hasselect)
            {
                socketRenderer.material = normalMaterial;
                socketRenderer.enabled = true;
            }
            hasCorrectPieceHover = false;
        }
    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        
        XRGrabInteractable selectedObject = args.interactableObject as XRGrabInteractable;
        if(hasselect){return;}
        if (selectedObject != null && selectedObject.transform.name == correctPiece.transform.name)
        {
            // 正确棋子放置
            isRight = true;
            hasselect = true;
            print("Correct piece placed");
        }
        else
        {
            // 错误棋子尝试放置
            isRight = false;
            hasselect = false;
            print("Wrong piece placed");
        }
        
        // 放置后隐藏渲染器
        if (socketRenderer != null)
        {
            socketRenderer.enabled = false;
        }
    }
    
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        isRight = false;
        hasselect = false;
        print("Piece removed");
        
        // 物体被拿走时，显示渲染器并恢复正常材质
        if (socketRenderer != null)
        {
            socketRenderer.enabled = true;
            socketRenderer.material = normalMaterial;
        }
    }
    
    // 辅助方法：更新正确棋子引用
    public void SetCorrectPiece(GameObject piece)
    {
        correctPiece = piece;
    }
    
    // 添加这个方法以确保悬停检测正常工作
    public override bool isSelectActive
    {
        get
        {
            return base.isSelectActive;
        }
    }
}