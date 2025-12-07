using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class rotatePuzzelManager : MonoBehaviour
{
    [SerializeField] GameObject[] model;//按从下到上的顺序放入拼图的模型块预制体
    [SerializeField] float rotationsentity = 1.0f;
    [SerializeField] InteractionLayerMask newMask;
    [SerializeField] RotatePuzzle[] puzzleParts;//一组拼图的所有拼图块
    private bool isRight = false;//用于判断整个旋转拼图组正不正确
    private bool isReady = false;
    void Start()
    {
        intPuzzleParts();
    }
    void FixedUpdate()
    {
        if(!isReady){return;}
        for(int i = 0; i < puzzleParts.Length-1; i++)
        {
            if(Mathf.Abs((puzzleParts[i].angle - puzzleParts[i + 1].angle).magnitude) >= 5.0f)//判断每两个拼图块的旋转角度是否一样，如果不一样，则返回，并将isRight设置为false，如果都一样，则完成循环并将isRight为true
            {
                isRight = false;
                return;
            }
        }
        isRight = true;
    }
    public void intPuzzleParts()
    {
        Vector3 lastboundssize = Vector3.zero;
        Vector3 lastposition = Vector3.zero;
        var _puzzleParts = new List<RotatePuzzle>();
        foreach (var item in model)
        {
            var bounds = item.GetComponent<Renderer>().bounds;
            Vector3 position = transform.position;
            if(lastboundssize != Vector3.zero)
            {
                position = lastposition;
                float y = lastboundssize.y/2.0f + bounds.size.y/2.0f;
                position = new Vector3(position.x,position.y+y,position.z);
            }
           var t01 = Instantiate(item,position,Quaternion.identity);
           var t02 = Instantiate(item,t01.transform.position,Quaternion.identity);
           t01.transform.SetParent(this.transform);
           t02.transform.SetParent(t01.transform);
           t02.AddComponent<RotatePuzzle>();
           var box = t01.AddComponent<BoxCollider>();
           t01.AddComponent<XRSimpleInteractable>();
           t01.GetComponent<XRSimpleInteractable>().interactionLayers = newMask;
           t02.GetComponent<RotatePuzzle>().simpleInteractable = t01.GetComponent<XRSimpleInteractable>();
           t02.GetComponent<RotatePuzzle>().rotatescale = rotationsentity;
           t01.GetComponent<MeshRenderer>().enabled = false;
           box.center = Vector3.zero;
           box.size = t01.transform.InverseTransformVector(bounds.size);
           _puzzleParts.Add(t02.GetComponent<RotatePuzzle>());
           lastposition = t01.transform.position;
           lastboundssize = bounds.size;
           t02.GetComponent<RotatePuzzle>()._int();
        }
        puzzleParts = _puzzleParts.ToArray();
        isReady = true;
    }
}
