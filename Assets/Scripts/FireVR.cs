using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class FireVR : MonoBehaviour
{

	public SteamVR_Action_Boolean firingAction;

	public Hand hand;

	public Laser laser;

	private void OnEnable()
	{
		if (hand == null)
			hand = this.GetComponent<Hand>();
		firingAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "Fire");
		if (firingAction == null)
		{
			Debug.LogError("<b>[SteamVR Interaction]</b> No fire action assigned", this);
			return;
		}

		firingAction.AddOnChangeListener(OnFireActionChange, hand.handType);
	}

	private void OnDisable()
	{
		if (firingAction != null)
			firingAction.RemoveOnChangeListener(OnFireActionChange, hand.handType);
	}

	private void OnFireActionChange(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
	{
		if (newValue)
		{
			Fire();
		}
	}

	public void Fire()
	{
		// Laser and fireVR are separated so that the laser can be used on non-vr player.
		laser.OnFire(null);
	}
}
