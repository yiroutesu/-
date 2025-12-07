using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
[AddComponentMenu("XR/Poke Setup")]
public class pokeset : MonoBehaviour
{
    [SerializeField] private PokeThresholdDatum Setting;
    private XRPokeFilter poke;
    private XRSimpleInteractable simpleIX;
    [SerializeField]private bool Is;
    [Header("音效")]
    [SerializeField]public AudioClip  se;          // 音效
    [Header("音效大小")]
    [SerializeField][Range(0,1)]public float volume = 1f;
    [Header("粒子")]
    [SerializeField]public ParticleSystem ps;      // 粒子

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            Addcom(child.gameObject);
        }
    }
    private void Addcom(GameObject a)
    {
        if(!a.GetComponent<Collider>())return;
        a.GetComponent<MeshRenderer>().enabled=false;
        simpleIX = a.GetComponent<XRSimpleInteractable>();
        if (simpleIX == null)
        {
            simpleIX = a.AddComponent<XRSimpleInteractable>();
            simpleIX.selectEntered.AddListener(isPoke);
            simpleIX.selectExited.AddListener(unPoke);
            simpleIX.hoverEntered.AddListener(hoverIn);
            simpleIX.hoverExited.AddListener(hoverout);
        }

        poke = a.GetComponent<XRPokeFilter>();
        if (poke == null)
        {
            poke = a.AddComponent<XRPokeFilter>();
        }
        if (Setting != null)
        {
            poke.pokeConfiguration = new PokeThresholdDatumProperty(Setting);
        }
    }

    private void hoverout(HoverExitEventArgs arg0)
    {

    }

    private void hoverIn(HoverEnterEventArgs arg0)
    {

    }

    private void unPoke(SelectExitEventArgs arg0)
    {

    }

    private void isPoke(SelectEnterEventArgs arg0)
    {
        Debug.Log(transform.name+"被戳中");
        findEndToPlay();
    }

    public void findEndToPlay()
    {
        if (se)
        {
            AudioSource.PlayClipAtPoint(se, transform.position, volume);
        }

        if (ps)
        {
            var particles = Instantiate(ps);
            particles.transform.SetParent(transform.GetComponentInParent<Transform>());
            particles.transform.position = transform.position+new Vector3(0,0.5f,0);
            Destroy(particles,20.0f);
        }
    }
}
