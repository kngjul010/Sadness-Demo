using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkController : MonoBehaviour
{
	//int blendShapeCount;


	public GameObject faceType;
	public SkinnedMeshRenderer skinnedMeshRenderer;

	public float timeBetweenBlinks = 3f;
	float initialScoredTime;

	float blendshapeWeight = 0f;
	bool blinked = false;
	public bool isDead = false;
	float blinkAcceleration = 700f;
	float maxBlendshapeWeight = 100f;
	private CharacterCustomize customizeScript;

	// Use this for initialization
	void Start ()
	{
		
		skinnedMeshRenderer = faceType.GetComponent<SkinnedMeshRenderer> ();

		initialScoredTime = timeBetweenBlinks;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isDead) {
			skinnedMeshRenderer = faceType.GetComponent<SkinnedMeshRenderer> ();
			skinnedMeshRenderer.SetBlendShapeWeight (12, 100f);

		} else {
			timeBetweenBlinks -= Time.deltaTime;
		}
		if (timeBetweenBlinks <= 0f) {

			if (blinked != true)
				Blink ();
			
		}
		if (timeBetweenBlinks > 0f) {

			blinked = false;
		}
	}

	IEnumerator ResetBlink ()
	{
		yield return new WaitForSeconds (.95f);

		if (skinnedMeshRenderer != null)
			skinnedMeshRenderer.SetBlendShapeWeight (12, 0f);
		timeBetweenBlinks = initialScoredTime - (Random.Range (-1f, 1f));



	}

	void Blink ()
	{
		if (blendshapeWeight < maxBlendshapeWeight) {
			blendshapeWeight += blinkAcceleration * Time.deltaTime;
		} else {
			blendshapeWeight = 0f;
			blinked = true;
			StartCoroutine ("ResetBlink");
		}
		skinnedMeshRenderer = faceType.GetComponent<SkinnedMeshRenderer> ();
		skinnedMeshRenderer.SetBlendShapeWeight (12, blendshapeWeight);


	}
}
