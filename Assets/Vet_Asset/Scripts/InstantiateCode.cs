using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCode : MonoBehaviour
{
	private int hairCol;
	private int hairTyp;
	private int faceTyp;
	private int eyeCol;
	private int topCol;
	private int btmCol;
	private int skinTyp;

	//	private MaterialsList materialsList;
	//	private BlinkController blinkScript;
	//	public SkinnedMeshRenderer skinnedMeshRenderer;

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

	public Transform prefabObject;
	public FaceType faceType;
	public SkinType skinType;
	public EyeColor eyeColor;
	public TopColors topColors;
	public BottomColors btmColors;
	public HairType hairType;
	public HairColor hairColor;



	// Use this for initialization
	void Start ()
	{
		Transform pref = Instantiate (prefabObject, gameObject.transform.position, gameObject.transform.rotation);

		//Usage:
		//charCustomize (int faceType 0-4, int eyeColor 0-5, int topColors 0-3, int bottomColors 0-3, int hairType 0-4, int hairColor 0-4, int skynType)
		/*
//faceType: 0 FaceA
//          1 FaceB
//          2 FaceC
//          3 FaceD
//          4 FaceE
//
//eyeColor: 0 Brown
//          1 Blue
//          2 Green
//          3 Black
//          4 Dark Blue
//          5 Light Brown
//          
//topColor: 0 White with blue shirt
//          1 Blue
//          2 Grey
//          3 White with purple shirt
//
//btmColors:0 White
//          1 Gray
//          2 Blue
//          3 Purple
//
//hairType: 0 Medium
//          1 Pony Tail
//          2 French Roll
//          3 Short
//          4 Bun
//
//hairColor:0 Blond
//          1 White
//          2 Dark
//          3 Red
//          4 Brown
//          
//skinType: 0 Pale,
//		  1 Old,
//		  2 Tanned,
//		  3 White,
//		  4 Black,
//		  5 DarkBrown,
//		  6 DarkerOld
//
//
//*/

		hairCol = (int)hairColor;
		eyeCol = (int)eyeColor;
		hairTyp = (int)hairType;
		faceTyp = (int)faceType;
		btmCol = (int)btmColors;
		topCol = (int)topColors;
		skinTyp = (int)skinType;
		pref.gameObject.GetComponent<CharacterCustomize> ().charCustomize (faceTyp, eyeCol, topCol, btmCol, hairTyp, hairCol, skinTyp);
		//pref.gameObject.GetComponent<CharacterCustomize> ().charCustomize (2, 3, 3, 3, 3, 3, 2);

	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
