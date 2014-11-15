using UnityEngine;
using System.Collections;

public class BallLogic : MonoBehaviour {

	public static float ballSpawnValue = 0;

	// Use this for initialization
	void OnCollisionEnter(Collision c)
	{
		ballSpawnValue += 0.5f;

		if (ballSpawnValue >= 1.0f) {
			Camera.main.GetComponent<PongLogic> ().AddBall (c.transform.position + new Vector3(0f,1f,0f));
			ballSpawnValue = 0;
		}
	}
}
