using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {

	public GameObject loadScreen;
	public GameObject backMenu;
	public GameObject abs;
	public GameObject prclose;

	public void moreGames()
	{
		Application.OpenURL ("https://play.google.com/store/apps/developer?id=i6+Games");
	}
	public void onGamePress()
	{
		AppSoundManager.Get ().PlaySfx (Sfx.Type.sfx_click);
		loadScreen.SetActive (true);
		GoTo.LoadEnvironmentChoose();
	}

	public void onShopPress()
	{
		loadScreen.SetActive (true);
		GoTo.LoadShop();
	}

	void Awake()
	{
		GameData.Get ();
		DontDestroyOnLoad (abs);
	}
	void Start()
	{
		IAS_Manager.Instance.ResetMainBanners(); 
	}
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			PreClosePopup.showPopup = true;
			backMenu.SetActive(true);
		}
	}
}
