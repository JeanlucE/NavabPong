using UnityEngine;
using System.Collections;

public class KillParticleSystem : MonoBehaviour {

	// Update is called once per frame
	void Update () 
    {
        if (!particleSystem.IsAlive()) Destroy(this.gameObject);
	}
}
