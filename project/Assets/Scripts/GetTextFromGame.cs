using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class GetTextFromGame : MonoBehaviour {

    private TextMesh textMesh;
    public string key = "";

	// Use this for initialization
	void Start () {
        textMesh = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        textMesh.text = Game.getInstance().getText(key);
	}
}
