using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	float sliderValue = 5;

    void OnGUI() {
        GUI.Box(new Rect(Screen.width / 2f - Screen.width / 10f, Screen.height / 2f - Screen.height / 8f, Screen.width/5f, Screen.height/4f), "Loader Menu");

        if (GUI.Button(new Rect(Screen.width / 2f - 60, Screen.height / 2f - Screen.height / 10f  + 30, 120, 20), "AI vs AI"))
        {
            Game.getInstance().selectedInputMethod = CustomInputScript.InputMethod.AIvsAI;
            Application.LoadLevel("pong");
        }

        // Make the second button.
        if (GUI.Button(new Rect(Screen.width / 2f - 60, Screen.height / 2f - Screen.height / 10f + 60, 120, 20), "AI vs Mouse"))
        {
            Game.getInstance().selectedInputMethod = CustomInputScript.InputMethod.AIvsMouse;
            Application.LoadLevel("pong");
        }

        if (GUI.Button(new Rect(Screen.width / 2f - 60, Screen.height / 2f - Screen.height / 10f + 90, 120, 20), "IMGE Device"))
        {
            Game.getInstance().selectedInputMethod = CustomInputScript.InputMethod.BeagleBoard;
            Application.LoadLevel("pong");
        }

        if (GUI.Button(new Rect(Screen.width / 2f - 60, Screen.height / 2f - Screen.height / 10f + 120, 120, 20), "W/S vs Up/Down"))
        {
            Game.getInstance().selectedInputMethod = CustomInputScript.InputMethod.Keyboard;
            Application.LoadLevel("pong");
        }

        if (GUI.Button(new Rect(Screen.width / 2f - 60, Screen.height / 2f - Screen.height / 10f + 150, 120, 20), "W/S vs Mouse"))
        {
            Game.getInstance().selectedInputMethod = CustomInputScript.InputMethod.KeyboardMouse;
            Application.LoadLevel("pong");
        }

		sliderValue = GUI.HorizontalSlider (new Rect (Screen.width / 2f - 100, Screen.height / 2f - Screen.height / 10f + 180, 200, 20), sliderValue, 2, 100);
		Game.getInstance ().maxAmountOfBalls = (int) Mathf.Round (sliderValue);
		GUI.Label (new Rect (Screen.width / 2f - 40, Screen.height / 2f - Screen.height / 10f + 205, 80, 20), "Max Balls: " + Mathf.Round (sliderValue));
    }
}
