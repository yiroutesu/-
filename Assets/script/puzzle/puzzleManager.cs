using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class puzzleManager : MonoBehaviour
{
    [SerializeField] chessPuzzle[] puzzleParts;//这里放入一组拼图的所有拼图块
    [SerializeField] ParticleSystem winingParticle;
    [SerializeField] Transform particleplaypoint;
    [SerializeField] RawImage introduceimage;
    public bool isRight = false;//用于判断整个拼图组是否正确
    private bool isWin = false;
    void FixedUpdate()
    {
        
        if (isRight)
        {
            if(isWin){return;}
            foreach (var item in puzzleParts){
            item.correctPiece.GetComponent<MeshCollider>().isTrigger = true;}
            var particles = Instantiate(winingParticle);
            particles.transform.SetParent(particleplaypoint);
            particles.transform.position = particleplaypoint.position;
            Destroy(particles,20.0f);
            isWin = true;
            introduceimage.GetComponent<RawImage>().enabled = false;
            return;
        }
        else
        {
            checkallParts();
        }
    }
    void checkallParts()
    {
        if(puzzleParts.Length <= 0){return;}
        foreach (var item in puzzleParts)
        {
            if(item is chessPuzzle _stock)
            {
                
                if (!item.isRight)
                {
                    isRight = false;
                    return;
                }
            }
        }
        isRight = true;
    }
}
