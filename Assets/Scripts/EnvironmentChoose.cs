using UnityEngine;
using System.Collections;
using System;

public class EnvironmentChoose : MonoBehaviour {

	public GameObject loadScreen;
	public GameObject backMenu;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			PreClosePopup.showPopup = true;
			backMenu.SetActive(true);
		}
	}

	public void  LoadCityOne()
	{
		playGame(GoTo.LoadGameTownOne);
	}

	public void  LoadCityTwo()
	{
		playGame(GoTo.LoadGameTownTwo);
	}

	public void LoadDrity()
	{
		playGame(GoTo.LoadGameDirty);
	}

	public void LoadSnow()
	{
		playGame(GoTo.LoadGameSnow);
	}

	public void LoadTrack()
	{
		playGame(GoTo.LoadGameTrack);
	}

	void playGame(Action func)
	{
		GameObject.Find ("AdmobAdAgent").GetComponent<AdMob_Manager> ().hideBanner ();
		loadScreen.SetActive (true);
		func ();
	}
}
