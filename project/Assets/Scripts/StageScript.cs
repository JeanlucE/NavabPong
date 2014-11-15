using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageScript : MonoBehaviour {

    private List<GameObject> balls;
    public GameObject background;
    public Material navab, menze;

	// Use this for initialization
	void Start () {
        balls = Camera.main.GetComponent<PongLogic>().getBalls();
	}
	
	// Update is called once per frame
	void Update () {
        if (balls.Count <= 0)
            return;

        GameObject ball = balls[balls.Count - 1];

            if (ball.transform.position.x < -1.875433f)
            {
                background.renderer.material = navab;
                background.transform.localScale = new Vector3(15.94f, background.transform.localScale.y, background.transform.localScale.z);
            }
            else if (ball.transform.position.x > 1.875433f)
            {
                background.renderer.material = navab;
                background.transform.localScale = new Vector3(-15.94f, background.transform.localScale.y, background.transform.localScale.z);
            }
            else
            {
                background.renderer.material = menze;
                background.transform.localScale = new Vector3(15.94f, background.transform.localScale.y, background.transform.localScale.z);
            }
	}
}
