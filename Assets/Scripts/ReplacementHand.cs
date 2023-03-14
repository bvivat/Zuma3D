using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ReplacementHand : MonoBehaviour
{
	[EnumFlags]
	[Tooltip("The flags used to attach this object to the hand.")]
	public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic | Hand.AttachmentFlags.DetachOthers;
	public SteamVR_Action_Boolean grabAction;

	public Hand hand;
	public SphereManager sphereManager;
	public int currentColor = 0;

	public Laser laser;


	private void OnEnable()
	{
		// Attach the newBall action
		if (hand == null)
			hand = this.GetComponent<Hand>();
		grabAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "NewBall");

		if (grabAction == null)
		{
			Debug.LogError("<b>[SteamVR Interaction]</b> No grab action assigned", this);
			return;
		}

		grabAction.AddOnChangeListener(OnGrabActionChange, hand.handType);
	}

	private void OnDisable()
	{
		if (grabAction != null)
			grabAction.RemoveOnChangeListener(OnGrabActionChange, hand.handType);
	}

	private void OnGrabActionChange(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
	{
		if (newValue)
		{
			Grab();
		}
	}

	public void Grab()
	{
		// Spawn a new ball
		var projectile = sphereManager.instantiateNextBall(transform.position, Quaternion.identity, currentColor);
		projectile.tag = "New";
		projectile.GetComponent<Follower>().enabled = false;
		// Scaled to the hand size
		projectile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		projectile.GetComponent<Rigidbody>().useGravity = true;

		// Trying to replicate some of the steamVR "Throwable" script behavior, for the new ball to be grabbed as soon as it spawns
		GrabTypes startingGrabType = hand.GetGrabStarting();
		if (startingGrabType != GrabTypes.None)
		{
			hand.AttachObject(projectile.gameObject, startingGrabType, attachmentFlags); //projectile.GetComponent<Throwable>() ?
			hand.HideGrabHint();
		}

	}

	public void SetCurrentColor(int color)
	{
		if (color < 0)
		{
			color = sphereManager.nextRandomColor();
		}
		currentColor = color;
	}
}

