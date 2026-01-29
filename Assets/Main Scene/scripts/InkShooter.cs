using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.Windows;

//[SelectionBase]
public class InkShooter : MonoBehaviour
{
    [SerializeField] ParticleSystem inkParticle;
    [SerializeField] Transform parentController;
    [SerializeField] Transform splatGunNozzle;

    void VisualPolish()
    {
        if (!DOTween.IsTweening(parentController))
        {
            parentController.DOComplete();
            Vector3 forward = -parentController.forward;
            Vector3 localPos = parentController.localPosition;
            parentController.DOLocalMove(localPos - new Vector3(0, 0, .2f), .03f)
                .OnComplete(() => parentController.DOLocalMove(localPos, .1f).SetEase(Ease.OutSine));
        }

        if (!DOTween.IsTweening(splatGunNozzle))
        {
            splatGunNozzle.DOComplete();
            splatGunNozzle.DOPunchScale(new Vector3(0, 1, 1) / 1.5f, .15f, 10, 1);
        }
    }
    public void StartShooting()
    {
        if(inkParticle!= null && !inkParticle.isPlaying)
        { 
            inkParticle.Play();
            VisualPolish();
        }
    }
    public void StopShooting()
    {
        if(inkParticle != null && inkParticle.isPlaying) 
            inkParticle.Stop();
    }
}
