using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using LitJson;

public class GameData{

	private static GameData instance;
	public bool sfx;
	public bool music;
	public bool extraBike;
	public int cash;
	public int currentBike;
	public List<int> allowBikes;

	private string version = "save_00092";
	public static GameData Get()
	{
		if (instance == null)
		{
			instance = new GameData();
			instance = instance.Load();
			AppSoundManager.MuteSfx = !instance.sfx;
			AppSoundManager.MuteMusic = !instance.music;
		}
		return instance;
	}


	GameData Load ()
	{		
		string data = PlayerPrefs.GetString(version, null);
		Debug.Log("Load game data:" + data);
		if (data == null || data.Trim() == "")
		{
			reset();
			return this;
		}
		GameData gdata;
		try
		{
			gdata = JsonMapper.ToObject<GameData>(data);
		}
		catch (System.Exception e)
		{
			reset();
			return this;
		}
		return gdata;
	}
	
	void reset ()
	{
		sfx = true;
		music = true;
		extraBike = false;
		cash = 0;
		currentBike = 0;
		allowBikes = new List<int> ();
		allowBikes.Add (0);
		save();
	}

	public void save ()
	{
		string data = JsonMapper.ToJson(this);
		Debug.Log("Save gamedata as:" + data);
		PlayerPrefs.SetString(version, data);
	}

	public bool bikeIsUnlock(int num)
	{
		for(int i = 0; i < allowBikes.Count;i++)
		{
			if (num == allowBikes[i])
				return true;
		}
		return false;
	}
	
}
