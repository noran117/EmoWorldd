using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using UnityEngine;


public class Dissolve : MonoBehaviour
{
   [SerializeField] private float _dissolveTime = 0.75f;

   private SpriteRenderer [] _spriteRenderers;
   private Material [] _materials;

   private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");

   private int _virticalDissolve = Shader.PropertyToID("_VerticalDissolve");

   private void Start()
   {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _materials = new Material[_spriteRenderers.Length];
        for( int i=0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
        }
   }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            StartCoroutine(Vanish(true, false));
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            StartCoroutine(Appear(true, false));
        }
        //Vertical
        /*
        if (Input.GetKeyUp(KeyCode.E))
        {
            StartCoroutine(Vanish(false, true));
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            StartCoroutine(Appear(false, true));
        }
        */
    }

    private IEnumerator Vanish(bool useDissolve,bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime){

            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / _dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / _dissolveTime));

            for(int i=0;i< _materials.Length; i++)
            {
                if(useDissolve)
                    _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
                if(useVertical)
                    _materials[i].SetFloat(_virticalDissolve, lerpedVerticalDissolve);
            }

            yield return null;
        }

    }
    private IEnumerator Appear(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {

            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(1.1f, 0, (elapsedTime / _dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(1.1f, 0, (elapsedTime / _dissolveTime));

            for (int i = 0; i < _materials.Length; i++)
            {
                if (useDissolve)
                    _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
                if (useVertical)
                    _materials[i].SetFloat(_virticalDissolve, lerpedVerticalDissolve);
            }

            yield return null;
        }

    }

}
