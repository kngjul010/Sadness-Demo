using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsList : MonoBehaviour
{
	
	public GameObject[] faceType = new GameObject[5];
	public GameObject[] faceMorphObject = new GameObject[5];
	public Material[] skin_Materials = new Material[6];
	public GameObject eyes_Object;
	public Material[] eyeColors = new Material[6];


	public Material[] TopMaterials = new Material[4];
	public GameObject TopObject;
	public Material[] BottomMaterials = new Material[4];
	public GameObject BottomObject;
	//HairA Materials and Objects

	public GameObject[] hairMainObjects = new GameObject[5];
	//public GameObject[] haiFadeObjects = new GameObject[5];
	public Material[] hairA_Materials = new Material[5];
	public Material[] hairA_FadeMaterials = new Material[5];
	//public GameObject HairA_Object;
	public GameObject HairA_FadeObject;
	//public GameObject HairA_MainObject;

	//HairB Materials and Objects
	public Material[] hairB_Materials = new Material[5];
	public Material[] hairB_FadeMaterials = new Material[5];
	//public GameObject HairB_Object;
	public GameObject HairB_FadeObject;
	//public GameObject HairB_MainObject;

	//HairC Materials and Objects
	public Material[] hairC_Materials = new Material[5];
	public Material[] hairC_FadeMaterials = new Material[5];
	//public GameObject HairC_Object;
	public GameObject HairC_FadeObject;
	//public GameObject HairC_MainObject;

	//HairD Materials and Objects
	public Material[] hairD_Materials = new Material[5];
	public Material[] hairD_FadeMaterials = new Material[5];
	//public GameObject HairD_Object;
	public GameObject HairD_FadeObject;
	//public GameObject HairD_MainObject;
	//HairE Materials and Objects
	public Material[] hairE_Materials = new Material[5];
	public Material[] hairE_FadeMaterials = new Material[5];
	//public GameObject HairE_Object;
	public GameObject HairE_FadeObject;
	//public GameObject HairE_MainObject;
	//public string stringText = "Test";

	void Start ()
	{
		//Material[] mats = Resources.LoadAll("Materials", typeof(Material)).Cast<Material>().ToArray();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
