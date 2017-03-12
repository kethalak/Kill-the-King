using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {
	const string PLATFORM_KEY = "Platform";

	public static void SetPlatform(string platform)
	{
		if(platform == "PC" || platform == "VR")
			PlayerPrefs.SetString(PLATFORM_KEY, platform);
		else
			Debug.LogError("cannot change Platform to " + platform);
	}
	public static string GetPlatform()
	{
		return PlayerPrefs.GetString(PLATFORM_KEY);
	}
}
