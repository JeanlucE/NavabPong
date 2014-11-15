using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.IO;

public class CustomInputScript : MonoBehaviour {

	// Input
	SerialPort stream;
	string receivedData = "EMPTY";
	private bool[] buttonPressed = new bool[6];

	private CircularBuffer leftSliderBuffer = new CircularBuffer(8);
	private CircularBuffer rightSliderBuffer = new CircularBuffer(8);

	private float leftPaddlePosLastFrame = 0f, rightPaddlePosLastFrame = 0f;
	private float leftPaddleSpeed = 0f, rightPaddleSpeed = 0f;

    //Keyboard input
    private const string LEFT_PLAYER_AXIS = "Vertical1";
    private const string RIGHT_PLAYER_AXIS = "Vertical2";
    private float leftValue = 0f, rightValue = 0f;

    void Awake() {
        try
        {
            stream = new SerialPort("COM3", 115200);
            stream.Open();
        }
        catch (IOException e) {
            Debug.Log(e.Message);   
        }
    }

	// Use this for initialization
	void Start () {

		if (stream.IsOpen) {
			Debug.Log ("Stream opened");
            stream.Write("4");
            receivedData = stream.ReadLine();

            leftPaddlePosLastFrame = (System.Convert.ToInt32(receivedData.Substring(18, 4), 16) - 2048f) / 2048f;
            rightPaddlePosLastFrame = (System.Convert.ToInt32(receivedData.Substring(13, 4), 16) - 2048) / 2048f;
		} else {
			Debug.Log ("Stream not open");
		}
	}
	
	// Update is called once per frame
	void Update () {
        if (!stream.IsOpen)
        {
            leftValue += Input.GetAxisRaw(LEFT_PLAYER_AXIS) * Time.deltaTime * 4.5f;
            rightValue = (Input.mousePosition.y / (float)Screen.height) * 2 - 1;
            

            leftValue = Mathf.Clamp(leftValue, -1f, 1f);
            rightValue = Mathf.Clamp(rightValue, -1, 1f);

            leftPaddleSpeed = leftValue - leftPaddlePosLastFrame;
            rightPaddleSpeed = rightValue - rightPaddlePosLastFrame;

            leftPaddlePosLastFrame = leftValue;
            rightPaddlePosLastFrame = rightValue;

            return;

        }
		//get Input
		stream.Write ("4");
		receivedData = stream.ReadLine ();
		
		float leftSlider = (System.Convert.ToInt32(receivedData.Substring (18, 4), 16) - 2048f) / 2048f;
		float rightSlider = (System.Convert.ToInt32(receivedData.Substring (13, 4), 16) - 2048) / 2048f;
		leftSliderBuffer.Add (leftSlider);
		rightSliderBuffer.Add (rightSlider);


		//Calculate Paddle speed
		float leftSliderPercentage = getLeftSlider ();
		float rightSliderPercentage = getRightSlider ();
		
		leftPaddleSpeed = leftSliderPercentage - leftPaddlePosLastFrame;
		rightPaddleSpeed = rightSliderPercentage - rightPaddlePosLastFrame;
		
		leftPaddlePosLastFrame = leftSliderPercentage;
		rightPaddlePosLastFrame = rightSliderPercentage;
	}

	public float getLeftSlider(){
        if (!stream.IsOpen)
            return leftValue;

		return leftSliderBuffer.Average ();
	}

	public float getRightSlider(){
        if (!stream.IsOpen)
            return rightValue;

		return rightSliderBuffer.Average ();
	}

	public void setMotor(int value){
        if (!stream.IsOpen)
            return;

		int val = Mathf.Clamp (value, 0, 1000);

		stream.Write ("m " + value + "\r\n");
		stream.ReadLine ();
	}

	public void setLight(int index, int status){
        if (!stream.IsOpen)
            return;

		stream.Write ("l " + index + " " + status + "\r\n");
		stream.ReadLine ();
	} 	

	public float getLeftSliderSpeed(){
		return leftPaddleSpeed;		
	}

	public float getRightSliderSpeed(){
		return rightPaddleSpeed;		
	}
}
