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
	
	public UILabel taskView;
	public UITexture taskImg;

	public Texture[] lvlTextures;

	public Transform missionsObj;

	public MusicSfx soundOBJ;

	public UILabel forCash;

	GameObject itemsWrapper;
	GameData data;

	string[] missionDescription = 
	{
		"Bank robbery reported. Robbers got away but dumped cash bags around the city. It's your job to go find them.",
		"Reports of a stolen vehicle in the area. Drive around and locate the stolen vehicle.",
		"Robbers are hiding the stashes around the city. We have reports of their known locations, find them.",
		"We’re getting reports of a stolen truck full of gold bars. Locate the stolen truck.",
		"Well done for finding the truck, however the gold bars seemed to have fallen out of the back while driving. You know what's coming next, Go find the gold Bars before they come back for them.",
		"Street Racers are in town, they surely plan to hold a race somewhere tonight, we don't know where and when. But lets go find all the cars and clamp them before night fall"
	};

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
	}

	void Start()
	{
		taskView.text = missionDescription [data.currentLvl - 1];
		taskImg.mainTexture = lvlTextures [data.currentLvl - 1];
		setMissionItem ();
		circleRemaining = itemsWrapper.transform.childCount;
		showScore ();
	}

	void setMissionItem ()
	{
		string name = "Mission " + data.currentLvl.ToString ();
		itemsWrapper = missionsObj.FindChild (name).gameObject;
		for(int i = 0; i < missionsObj.childCount; i++)
		{
			if(missionsObj.GetChild(i).name != name)
				missionsObj.GetChild(i).gameObject.SetActive(false);
		}
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
					//GameObject.Find ("BikeManager").GetComponent<BikeManager> ().SetAdditionalBike();
					textErn = "Congratulation you found all items!";
					if(data.currentLvl == data.allowLvls)
					{
						data.allowLvls ++;
						data.save();
					}
					StartCoroutine(goToLvlChoose());
				}

				earningView.GetComponent<UILabel>().text =textErn;
				earningView.GetComponent<Animator>().Play("earning",0,0f);
			}
			if(forCash == null)
				forCash = GameObject.Find("forCash").GetComponent<UILabel>();
			forCash.text = (itemsWrapper.transform.childCount - circleRemaining).ToString() + " / "+itemsWrapper.transform.childCount.ToString();
		}
	}

	IEnumerator goToLvlChoose()
	{
		yield return new WaitForSeconds (3.5f);
		GameObject.Find ("BikeManager").GetComponent<BikeManager> ().Reset ();
		GoTo.LoadEnvironmentChoose ();
		yield return null;
	}

	IEnumerator refreshCircles ()
	{
		yield return new WaitForSeconds(1f);
		circleRemaining = itemsWrapper.transform.childCount;
		for(int i = 0; i< itemsWrapper.transform.childCount;i++)
		{
			itemsWrapper.transform.GetChild(i).GetComponent<Circle>().refresh();
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
