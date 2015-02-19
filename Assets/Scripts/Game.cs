using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public BikeCamera cam;
	public GameType type;
	//public UIConfig conf;

	public GameObject firstBike;
	public GameObject secondBike;

	public Material[] bikeMaterials;

	public GameObject popup;
	public GameObject homePopup;
	public GameObject shopPopup;
	public GameObject preStartMenu;
	public GameObject buttons;
	public GameObject arrowControls;
	public GameObject tiltControls;
	public GameObject nitroBtn;
	public GameObject earningView;
	public GameObject bikeAvailable;
	public GameObject infoShow;
	public GameObject buttonsList;

	public GameObject circleWrapper;

	public MusicSfx soundOBJ;

	public UILabel forCash;

	GameData data;

	public static bool isRunning;

	private int[] powers = {90,95,100,102,105,107};
	private int[] shiftPowers = {50,55,60,65,70,75};

	private bool isHomeShow = false;
	private float scale = 0f;

	private int circleRemaining;

	public enum GameType
	{
		collectForCount,
		collectForPoints
	}

	void Awake()
	{
		data = GameData.Get ();
		//setBikeProperties ();
	}

	void Start()
	{
		circleRemaining = circleWrapper.transform.childCount;
		showScore ();
	}

	void setBikeProperties ()
	{
		Transform targetBike;
		if(data.currentBike <=2)
		{
			firstBike.SetActive(true);
			secondBike.SetActive(false);
			targetBike = firstBike.transform;
		}
		else
		{
			firstBike.SetActive(false);
			secondBike.SetActive(true);
			targetBike = secondBike.transform;
		}
		cam.target = targetBike;
		targetBike.FindChild ("Body").GetComponent<MeshRenderer> ().material = bikeMaterials [data.currentBike];
		targetBike.FindChild ("Steer2").GetComponent<MeshRenderer> ().material = bikeMaterials [data.currentBike];
		BikeControl b = targetBike.GetComponent<BikeControl> ();
		b.bikeSetting.bikePower = powers [data.currentBike];
		b.bikeSetting.shiftPower = shiftPowers [data.currentBike];

		Transform[] positionView = {targetBike.FindChild("Components").FindChild("ForestView").FindChild("View-2").transform/*,
			targetBike.FindChild("Components").FindChild("ForestView").FindChild("View-3").transform*/};
		cam.cameraSwitchView = positionView;
	}
	
	public void StartGame(bool toHideLRButtons)
	{
		preStartMenu.SetActive (false);
		buttons.SetActive (true);
		isRunning = true;
		ShowLeftRightButtons(!toHideLRButtons);
	}
	
	public void ShowLeftRightButtons(bool toShow) 
	{
		arrowControls.SetActive (toShow);
		tiltControls.SetActive (!toShow);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(!isHomeShow)
			{
				isRunning = false;
				PreClosePopup.showPopup = true;
				soundOBJ.muteTMP();
				popup.SetActive(true);
				scale = Time.timeScale;
				Time.timeScale = 0f;
			}
			else
			{
				hideHomePopup();
			}
		}
	}


	public void showScore(int points = 0)
	{
		if(type == GameType.collectForPoints)
		{
			if(points != 0)
				circleRemaining -=1;
			addPoints (points);
			ShowEarning (points);
			if(forCash == null)
				forCash = GameObject.Find("forCash").GetComponent<UILabel>();
			forCash.text = "Points: " + data.cash.ToString ();
			if(circleRemaining == 0)
				StartCoroutine(refreshCircles());
		}
		else if (type == GameType.collectForCount)
		{
			if(points != 0)
			{
				circleRemaining -=1;
				string textErn = "";
				if(circleRemaining != 0)
					textErn = circleRemaining.ToString() + " items remain!";
				else
				{
					GameObject.Find ("BikeManager").GetComponent<BikeManager> ().SetAdditionalBike();
					textErn = "You have got additional Bike on each map!";
				}

				earningView.GetComponent<UILabel>().text =textErn;
				earningView.GetComponent<Animator>().Play("earning",0,0f);
			}
			if(forCash == null)
				forCash = GameObject.Find("forCash").GetComponent<UILabel>();
			forCash.text = (circleWrapper.transform.childCount - circleRemaining).ToString() + " / "+circleWrapper.transform.childCount.ToString();
		}
	}

	IEnumerator refreshCircles ()
	{
		yield return new WaitForSeconds(1f);
		circleRemaining = circleWrapper.transform.childCount;
		for(int i = 0; i< circleWrapper.transform.childCount;i++)
		{
			circleWrapper.transform.GetChild(i).GetComponent<Circle>().refresh();
			yield return new WaitForSeconds(0.03f);
		}
		yield return null;
	}

	void addPoints (int points)
	{
		if (points <= 0)
			return;

		List<bool> pre = availableBike (data.cash);
		List<bool> after = availableBike (data.cash + points);

		bool newBike = false;
		for(int i = 0; i< pre.Count; i++)
		{
			if(pre[i] != after[i])
			{
				newBike = true;
				break;
			}
		}

		if(newBike)
		{
			StartCoroutine(showAvailableBike());
		}

		data.cash += points;
		data.save ();
	}

	List<bool> availableBike (int points)
	{
		bool result = false;
		List<bool> bikesAllow = new List<bool> ();
		for(int i = 0; i < Shop.prices.Length; i++)
		{
			bool equal = false;
			for(int j = 0;j < data.allowBikes.Count; j++ )
			{
				if(i == data.allowBikes[j])
				{
					equal = true;
					break;
				}
			}

			if(!equal && Shop.prices[i]<= points)
			{
				result = true;
				bikesAllow.Add(true);
				//break;
			}
			else if(!equal)
				bikesAllow.Add(false);
		}
		return bikesAllow;
	}

	IEnumerator showAvailableBike()
	{
		yield return new WaitForEndOfFrame ();
		bikeAvailable.SetActive (true);
		yield return new WaitForSeconds (5.0f);
		bikeAvailable.SetActive (false);
		yield return null;
	}
	void ShowEarning (int points)
	{
		if (points <= 0)
			return;
		if(earningView == null)
			earningView = GameObject.Find("earningView");
		earningView.GetComponent<UILabel>().text = "You got "+points+" coins!";
		earningView.GetComponent<Animator>().Play("earning",0,0f);
	}

	public void showHomePopup()
	{
		isHomeShow = true;
		homePopup.SetActive (true);
	}
	public void hideHomePopup()
	{
		isHomeShow = false;
		homePopup.SetActive(false);
	}
	public void mainMenu()
	{
		GameObject.Find ("BikeManager").GetComponent<BikeManager> ().Reset ();
		Time.timeScale = 1f;
		isRunning = false;
		GoTo.LoadMenu ();
	}

	public void showShopPopup()
	{
		shopPopup.SetActive (true);
	}
	public void hideShopPopup()
	{
		shopPopup.SetActive (false);
	}

	public void goShop()
	{
		Time.timeScale = 1f;
		isRunning = false;
		GoTo.LoadShop ();
	}

	public void onInfoClick()
	{
		infoShow.SetActive (false);
		GameObject.Find ("AdmobAdAgent").GetComponent<AdMob_Manager> ().showBanner();
	}

	public void onListClick()
	{
		float val = 150f;
		//nitroBtn.SetActive (buttonsList.activeSelf);
		Vector3 pos = nitroBtn.transform.localPosition;
		if(buttonsList.activeSelf)
		{
			pos.x -= val;
			buttonsList.SetActive(false);
		}
		else
		{
			pos.x += val;
			buttonsList.SetActive(true);
		}
		nitroBtn.transform.localPosition = pos;
	}
}
