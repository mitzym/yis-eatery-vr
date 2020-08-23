using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class VR_ObjectHeld : XRBaseInteractor
{
    private GameObject _objectBeingHeld = null;
    public bool ObjectBeingHeld { get { return _objectBeingHeld; } }

    public override void GetValidTargets(List<XRBaseInteractable> validTargets)
    {
        throw new System.NotImplementedException();
    }

    public void SetObjectBeingHeld(GameObject obj)
    {
        _objectBeingHeld = obj;
    }
}
