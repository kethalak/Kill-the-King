using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public VR_Player player;
	public List<VR_Player> players;
	private static GameManager instance;

	public static GameManager Instance
	{
		get
		{
			return instance;
		}
	}

	// IEnumerator AnimateArenaChange(Arena arena)
	// {
	// 	t=0;
	// 	while(t < 1)
	// 	{
	// 		t += Time.deltaTime;
	// 		currentArena.transform.localScale = Vector3.Slerp(currentArena.transform.localScale, Vector3.zero, 3 * Time.deltaTime);
	// 		yield return null;
	// 	}
	// 	currentArena.gameObject.SetActive(false);
	// 	arena.gameObject.transform.localScale = currentArena.transform.localScale;
	// 	arena.gameObject.SetActive(true);
	// 	while(t < 4)
	// 	{
	// 		t += Time.deltaTime;
	// 		arena.transform.localScale  = Vector3.Lerp(arena.transform.localScale, new Vector3(1, 1, 1), 3 * Time.deltaTime);
	// 		yield return null;
	// 	}
	// 	arena.transform.localScale = new Vector3(1, 1, 1);
	// 	currentArena.transform.localScale = new Vector3(1, 1, 1);
	// 	Arena temp = currentArena;
	// 	currentArena = arena;
	// 	arenas[1] = temp;
	// }

    void Awake() 
	{
		if(instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}
		instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }
}
