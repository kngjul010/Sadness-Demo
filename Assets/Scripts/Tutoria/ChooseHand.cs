using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;

public class ChooseHand : UIElement
{
    public HandModels handModels;
    public bool isMaleOption;

    protected SkeletonUIOptions ui;

    protected override void Awake()
    {
        base.Awake();

        ui = this.GetComponentInParent<SkeletonUIOptions>();
        handModels = GameObject.FindGameObjectWithTag("Player").GetComponent<HandModels>();
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();

        if (ui != null)
        {
            if (isMaleOption)
            {
                handModels.isMale = true;
                handModels.maleLeft.SetActive(true);
                handModels.maleRight.SetActive(true);
                handModels.femaleLeft.SetActive(false);
                handModels.femaleRight.SetActive(false);
            }
            else
            {
                handModels.isMale = false;
                handModels.maleLeft.SetActive(false);
                handModels.maleRight.SetActive(false);
                handModels.femaleLeft.SetActive(true);
                handModels.femaleRight.SetActive(true);
            }
        }
    }
}
