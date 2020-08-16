using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class HandAccept : UIElement
{
    private bool confirm;


    protected override void Awake()
    {
        base.Awake();

        confirm = false;
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        confirm = true;
        
    }

    public bool getConfirm()
    {
        return confirm;
    }


}
