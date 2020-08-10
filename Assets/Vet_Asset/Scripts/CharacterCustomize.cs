using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomize : MonoBehaviour
{
	private int hairCol;
	private int hairTyp;
	private int faceTyp;
	private int eyeCol;
	private int topCol;
	private int btmCol;
	private int skinTyp;

	private MaterialsList materialsList;
	private BlinkController blinkScript;
	private SkinnedMeshRenderer skinnedMeshRenderer;

	public enum HairType
	{
		Medium,
		PonyTail,
		FrenchRoll,
		Short,
		Bun

	}

	public enum HairColor
	{
		Blond,
		White,
		Dark,
		Red,
		Brown

	}

	public enum EyeColor
	{
		Brown,
		Blue,
		Green,
		Black,
		DarkBlue,
		LightBrown

	}


	public enum FaceType
	{
		FaceA,
		FaceB,
		FaceC,
		FaceD,
		FaceE

	}

	public enum SkinType
	{
		Pale,
		Old,
		Tanned,
		White,
		Black,
		DarkBrown,
		DarkerOld

	}


	public enum TopColors
	{
		WhiteBlue,
		Blue,
		Grey,
		WhitePurple

	}

	public enum BottomColors
	{
		White,
		Gray,
		Blue,
		Purple

	}

	public FaceType faceType;
	public SkinType skinType;
	public EyeColor eyeColor;
	public TopColors topColors;
	public BottomColors btmColors;
	public HairType hairType;
	public HairColor hairColor;
	//public GameObject currentFace;

	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void charCustomize (int face, int eye, int top, int bottom, int hairT, int hairC, int skinT)
	{
		materialsList = gameObject.GetComponent<MaterialsList> ();
		blinkScript = gameObject.GetComponent<BlinkController> ();

		//Set Faces visibility and material
		for (int i = 0; i < materialsList.faceType.Length; i++) {
			materialsList.faceType [i].SetActive (false);
		}
		materialsList.faceType [face].SetActive (true);
		blinkScript.faceType = materialsList.faceMorphObject [face];
		//Set Head skin
		Transform[] allFaceChilds = materialsList.faceType [face].transform.GetComponentsInChildren<Transform> (true);
		foreach (Transform child in materialsList.faceType [face].transform) {
			//print ("Foreach loop: " + child);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.skin_Materials [skinT];
		}

		//HairColor

		//HairA==========================
		foreach (Transform child in materialsList.hairMainObjects [0].transform) {
			//print ("Foreach loop: " + child);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.hairA_Materials [hairC];
		}
		Renderer hairARendFade = materialsList.HairA_FadeObject.GetComponent<Renderer> ();
		hairARendFade.material = materialsList.hairA_FadeMaterials [hairC];
		//===========================================
		//HairB==========================
		if (materialsList.hairMainObjects [1] != null) {
			foreach (Transform child in materialsList.hairMainObjects [1].transform) {
				//print ("Foreach loop: " + child);
				Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
				skinRend.material = materialsList.hairB_Materials [hairC];
			}
			Renderer hairBRendFade = materialsList.HairB_FadeObject.GetComponent<Renderer> ();
			hairBRendFade.material = materialsList.hairB_FadeMaterials [hairC];

		}

		//===========================================

		//HairC==========================
		foreach (Transform child in materialsList.hairMainObjects [2].transform) {
			//print ("Foreach loop: " + child);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.hairC_Materials [hairC];
		}
		Renderer hairCRendFade = materialsList.HairC_FadeObject.GetComponent<Renderer> ();
		hairCRendFade.material = materialsList.hairC_FadeMaterials [hairC];
		//===========================================
		//HairD==========================
		foreach (Transform child in materialsList.hairMainObjects [3].transform) {
			//print ("Foreach loop: " + child);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.hairD_Materials [hairC];
		}
		Renderer hairDRendFade = materialsList.HairD_FadeObject.GetComponent<Renderer> ();
		hairDRendFade.material = materialsList.hairD_FadeMaterials [hairC];
		//===========================================
		//HairE==========================
		foreach (Transform child in materialsList.hairMainObjects [4].transform) {
			//print ("Foreach loop: " + child);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.hairE_Materials [hairC];
		}
		Renderer hairERendFade = materialsList.HairE_FadeObject.GetComponent<Renderer> ();
		hairERendFade.material = materialsList.hairE_FadeMaterials [hairC];
		//===========================================
		//Hair Type



		for (int i = 0; i < materialsList.hairMainObjects.Length; i++) {
			materialsList.hairMainObjects [i].SetActive (false);
		}
		materialsList.hairMainObjects [hairT].SetActive (true);


		foreach (Transform child in materialsList.TopObject.transform) {
			//print ("Foreach loop: " + child);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.TopMaterials [top];
		}

		//Se Bottom Color

		foreach (Transform child in materialsList.BottomObject.transform) {
			//print ("Foreach loop: " + child);
			//child.gameObject.SetActive (false);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.BottomMaterials [bottom];
		}


		//
		foreach (Transform child in materialsList.eyes_Object.transform) {
			//print ("Foreach loop: " + child);
			//child.gameObject.SetActive (false);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.eyeColors [eye];
		}

	}

	void OnValidate ()
	{
		//code for In Editor customize
		hairCol = (int)hairColor;
		eyeCol = (int)eyeColor;
		hairTyp = (int)hairType;
		faceTyp = (int)faceType;
		btmCol = (int)btmColors;
		topCol = (int)topColors;
		skinTyp = (int)skinType;
		charCustomize (faceTyp, eyeCol, topCol, btmCol, hairTyp, hairCol, skinTyp);
	
	}
}
