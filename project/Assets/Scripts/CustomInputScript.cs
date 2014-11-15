using System.IO;
using System.IO.Ports;
using UnityEngine;

public class CustomInputScript : MonoBehaviour
{

    // Input Method
    public enum InputMethod { AIvsMouse = 1, BeagleBoard = 2, Keyboard = 3, KeyboardMouse = 4 };
    public InputMethod inputMethod;

    // Input
    SerialPort stream;
    string receivedData = "EMPTY";
    //private bool[] buttonPressed = new bool[6];

    private CircularBuffer leftSliderBuffer = new CircularBuffer(8);
    private CircularBuffer rightSliderBuffer = new CircularBuffer(8);

    private float leftPaddlePosLastFrame = 0f, rightPaddlePosLastFrame = 0f;
    private float leftPaddleSpeed = 0f, rightPaddleSpeed = 0f;

    //Keyboard input
    private const string LEFT_PLAYER_AXIS = "Vertical1";
    private const string RIGHT_PLAYER_AXIS = "Vertical2";
    private float leftValue = 0f, rightValue = 0f;

    // PongLogic COntact for AI
    private PongLogic pongLogic;

    void Awake()
    {
        if (inputMethod == InputMethod.BeagleBoard)
        {
            try
            {
                stream = new SerialPort("COM3", 115200);
                stream.Open();
            }
            catch (IOException e)
            {
                Debug.Log(e.Message + "\nCould not access Cactus Controller. Switching to Keyboard/Mouse setup");
                inputMethod = InputMethod.KeyboardMouse;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        if (stream != null)
        {
            Debug.Log("Stream opened");
            stream.Write("4");
            receivedData = stream.ReadLine();

            leftPaddlePosLastFrame = (System.Convert.ToInt32(receivedData.Substring(18, 4), 16) - 2048f) / 2048f;
            rightPaddlePosLastFrame = (System.Convert.ToInt32(receivedData.Substring(13, 4), 16) - 2048) / 2048f;
        }

        pongLogic = Camera.main.GetComponent<PongLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputMethod >= InputMethod.Keyboard)
        {
            leftValue += Input.GetAxisRaw(LEFT_PLAYER_AXIS) * Time.deltaTime * 4.5f;

            if (inputMethod == InputMethod.Keyboard)
                rightValue += Input.GetAxisRaw(RIGHT_PLAYER_AXIS) * Time.deltaTime * 4.5f;
            else
                rightValue = (Input.mousePosition.y / (float)Screen.height) * 2 - 1;


            leftValue = Mathf.Clamp(leftValue, -1f, 1f);
            rightValue = Mathf.Clamp(rightValue, -1, 1f);

            leftPaddleSpeed = leftValue - leftPaddlePosLastFrame;
            rightPaddleSpeed = rightValue - rightPaddlePosLastFrame;

            leftPaddlePosLastFrame = leftValue;
            rightPaddlePosLastFrame = rightValue;
        }
        else if (inputMethod == InputMethod.BeagleBoard)
        {
            stream.Write("4");
            receivedData = stream.ReadLine();

            float leftSlider = (System.Convert.ToInt32(receivedData.Substring(18, 4), 16) - 2048f) / 2048f;
            float rightSlider = (System.Convert.ToInt32(receivedData.Substring(13, 4), 16) - 2048) / 2048f;
            leftSliderBuffer.Add(leftSlider);
            rightSliderBuffer.Add(rightSlider);


            //Calculate Paddle speed
            float leftSliderPercentage = getLeftSlider();
            float rightSliderPercentage = getRightSlider();

            leftPaddleSpeed = leftSliderPercentage - leftPaddlePosLastFrame;
            rightPaddleSpeed = rightSliderPercentage - rightPaddlePosLastFrame;

            leftPaddlePosLastFrame = leftSliderPercentage;
            rightPaddlePosLastFrame = rightSliderPercentage;
        }
        else
        {
            GameObject closestBall = null;

            foreach (GameObject ball in pongLogic.getBalls())
            {
                if (closestBall == null || closestBall.transform.position.x > ball.transform.position.x)
                    closestBall = ball;
            }

            if (closestBall == null)
                return;

            leftValue = (float)closestBall.transform.position.y / pongLogic.getMaxBatPos();
            rightValue = (Input.mousePosition.y / (float)Screen.height) * 2 - 1;


            leftValue = Mathf.Clamp(leftValue, -1f, 1f);
            rightValue = Mathf.Clamp(rightValue, -1, 1f);

            leftPaddleSpeed = leftValue - leftPaddlePosLastFrame;
            rightPaddleSpeed = rightValue - rightPaddlePosLastFrame;

            leftPaddlePosLastFrame = leftValue;
            rightPaddlePosLastFrame = rightValue;
        }
    }

    public float getLeftSlider()
    {
        if (stream == null)
            return leftValue;

        return leftSliderBuffer.Average();
    }

    public float getRightSlider()
    {
        if (stream == null)
            return rightValue;

        return rightSliderBuffer.Average();
    }

    public void setMotor(int value)
    {
        if (stream == null)
            return;

        int val = Mathf.Clamp(value, 0, 1000);

        stream.Write("m " + value + "\r\n");
        stream.ReadLine();
    }

    public void setLight(int index, int status)
    {
        if (stream == null)
            return;

        stream.Write("l " + index + " " + status + "\r\n");
        stream.ReadLine();
    }

    public float getLeftSliderSpeed()
    {
        return leftPaddleSpeed;
    }

    public float getRightSliderSpeed()
    {
        return rightPaddleSpeed;
    }
}
