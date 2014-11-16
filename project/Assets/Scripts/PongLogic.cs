using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomInputScript))]
public class PongLogic : MonoBehaviour
{

    public GameObject player1, player2, ballPrefab;
    public int maxAmountOfBalls;
    public GameObject[] balls;
    private List<GameObject> ballsIntern = new List<GameObject>(5);
    public GameObject upper, lower, left, right; // Walls
    public TextMesh winText;
    public ParticleSystem ballHitP1, ballHitP2;

    private Game game = Game.getInstance();

    //Ball variablen
    private float maxBatPos = 3.2f;
    private float maxAddedYForce = 220f;
    private float velocityBoost = 1.07f;
    private float startSpeed = 400f;
    private float maxSpeed = 20f;
    private float velocityXtoYRatio = 1.3f;
    private float minVelocity = 6f;

    // Ball Splitting
    private float timeToBallIdleSplit = 10f;
    private float durationToBallSplit = 1.3f;
    private float velocityPercentageAfterSplit = 0.5f;

    //Wie stark der motor rumbled bei einem auftreffen vom ball
    public int rumblePower = 1000;

    //Drall vom Ball
    public float velocityToSpinRatio = 1.5f;
    private List<float> ballSpin = new List<float>(5);
    private float spinFalloff = 0.99f;

    // Input
    private CustomInputScript customInput;

    void Start()
    {
        customInput = GetComponent<CustomInputScript>();

        foreach (GameObject g in balls)
        {
            if (g)
            {
                ballsIntern.Add(g);
                g.rigidbody.AddForce(Vector3.right * -startSpeed);
                startSpeed *= -1;
                ballSpin.Add(0);
            }
        }

        Screen.showCursor = false;
    }

    // Update is called once per frame
    void Update()
    {

        float leftSliderPercentage = customInput.getLeftSlider();
        float rightSliderPercentage = customInput.getRightSlider();

        float leftPlayerSpeed = customInput.getLeftSliderSpeed();
        float rightPlayerSpeed = customInput.getRightSliderSpeed();


        player1.transform.position = new Vector3(player1.transform.position.x, leftSliderPercentage * maxBatPos, player1.transform.position.z);
        player2.transform.position = new Vector3(player2.transform.position.x, rightSliderPercentage * maxBatPos, player2.transform.position.z);


        for (int i = ballsIntern.Count - 1; i >= 0; i--)
        {
            GameObject ball = ballsIntern[i];

            Vector3 newVel = Vector3.Normalize(ball.rigidbody.velocity + new Vector3(0, ballSpin[i], 0));
            float ballSpeed = ball.rigidbody.velocity.magnitude;
            ball.rigidbody.velocity = newVel * ballSpeed;
            ballSpin[i] *= spinFalloff;


            if (hasHitLeftPlayer(ball))
            {

                float angleForce = ((player1.renderer.bounds.max.y - ball.transform.position.y - player1.renderer.bounds.size.y / 2f) / (player1.renderer.bounds.size.y / 2f)) * -maxAddedYForce;

                ball.rigidbody.AddForce(Vector3.up * angleForce);
                ball.rigidbody.velocity = new Vector3(Mathf.Min(Mathf.Abs(ball.rigidbody.velocity.x) * velocityBoost, maxSpeed), Mathf.Sign(ball.rigidbody.velocity.y) * Mathf.Min(Mathf.Abs(ball.rigidbody.velocity.y), Mathf.Abs(velocityXtoYRatio * ball.rigidbody.velocity.x)), 0);

                Instantiate(ballHitP1, ball.transform.position, ballHitP1.transform.rotation);
                StartCoroutine("Rumble");

                ballSpin[i] = leftPlayerSpeed * velocityToSpinRatio;
            }

            if (hasHitRightPlayer(ball))
            {

                float angleForce = ((player2.renderer.bounds.max.y - ball.transform.position.y - player2.renderer.bounds.size.y / 2f) / (player2.renderer.bounds.size.y / 2f)) * -maxAddedYForce;

                ball.rigidbody.AddForce(Vector3.up * angleForce);
                ball.rigidbody.velocity = new Vector3(Mathf.Max(Mathf.Abs(ball.rigidbody.velocity.x) * -1 * velocityBoost, -maxSpeed), Mathf.Sign(ball.rigidbody.velocity.y) * Mathf.Min(Mathf.Abs(ball.rigidbody.velocity.y), Mathf.Abs(velocityXtoYRatio * ball.rigidbody.velocity.x)), 0);

                Instantiate(ballHitP2, ball.transform.position, ballHitP2.transform.rotation);
                StartCoroutine("Rumble");

                ballSpin[i] = rightPlayerSpeed * velocityToSpinRatio;
            }

            if (ball.renderer.bounds.max.y >= upper.renderer.bounds.min.y)
            {
                ball.rigidbody.velocity = new Vector3(ball.rigidbody.velocity.x, Mathf.Abs(ball.rigidbody.velocity.y) * -1, 0);
            }

            if (ball.renderer.bounds.min.y <= lower.renderer.bounds.max.y)
            {
                ball.rigidbody.velocity = new Vector3(ball.rigidbody.velocity.x, Mathf.Abs(ball.rigidbody.velocity.y), 0);
            }

            if (ball.renderer.bounds.min.x <= left.renderer.bounds.min.x)
            { // eigtl bound max, aber buggt

                ballsIntern.Remove(ball);
                ballSpin.RemoveAt(i);
                Destroy(ball);
                Game.getInstance().player2GameScore++;
            }
            else if (ball.renderer.bounds.max.x >= right.renderer.bounds.max.x)
            { // eigtl bound min, aber buggt

                ballsIntern.Remove(ball);
                ballSpin.RemoveAt(i);
                Destroy(ball);
                Game.getInstance().player1GameScore++;
            }

            if (ball.rigidbody.velocity.magnitude < minVelocity)
            {
                ball.rigidbody.velocity *= (minVelocity / (ball.rigidbody.velocity.magnitude != 0f ? ball.rigidbody.velocity.magnitude : 1));
            }
        }

        if (ballsIntern.Count == 0)
        {
            if (game.player1GameScore > game.player2GameScore)
                winText.text = "Player 1 wins";
            else if (game.player2GameScore > game.player1GameScore)
                winText.text = "Player 2 wins";
            else
                winText.text = "Navab wins";

            StartCoroutine("GameOver");
        }
        else if (ballsIntern.Count == 1 && !idleBalloonRunning)
        {
            StartCoroutine("IdleBalloon");
        }

    }

    private bool hasHitLeftPlayer(GameObject ball)
    {
        return ball.renderer.bounds.min.x <= player1.renderer.bounds.max.x &&
                ball.transform.position.y <= player1.renderer.bounds.max.y &&
                ball.transform.position.y >= player1.renderer.bounds.min.y;
    }

    private bool hasHitRightPlayer(GameObject ball)
    {
        return ball.renderer.bounds.max.x >= player2.renderer.bounds.min.x &&
                ball.transform.position.y <= player2.renderer.bounds.max.y &&
                ball.transform.position.y >= player2.renderer.bounds.min.y;
    }

    public AnimationCurve slowCurve;
    private bool idleBalloonRunning = false;
    private IEnumerator IdleBalloon()
    {
        idleBalloonRunning = true;
        yield return new WaitForSeconds(timeToBallIdleSplit);

        while (ballsIntern.Count == 1 && !isMovingTowardCenter(ballsIntern[0]))
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (ballsIntern.Count == 1)
        {
            GameObject ball = ballsIntern[0];
            ballSpin[0] = 0;
            ball.GetComponent<TrailRenderer>().enabled = false;
            float startVel = ball.rigidbody.velocity.magnitude;
            float startX = ball.transform.position.x;

            while (Mathf.Abs(ball.transform.position.x) >= 0.075f)
            {
                ball.rigidbody.velocity = ball.rigidbody.velocity.normalized * slowCurve.Evaluate(ball.transform.position.x / -startX + 1) * startVel;
                yield return new WaitForFixedUpdate();
            }

            ball.transform.position = new Vector3(0, ball.transform.position.y, 0);
            ball.rigidbody.velocity = new Vector3(0, 0, 0);

            yield return new WaitForSeconds(durationToBallSplit);

            AddBall(new Vector3(0.5f, ball.transform.position.y, 0f), new Vector3(startVel * velocityPercentageAfterSplit, 0f, 0f));

            ball.transform.position = new Vector3(-0.5f, ball.transform.position.y, 0);
            ball.rigidbody.velocity = new Vector3(-startVel * velocityPercentageAfterSplit, 0f, 0f);
            ball.GetComponent<TrailRenderer>().enabled = true;

            idleBalloonRunning = false;
        }
    }

    private bool isMovingTowardCenter(GameObject ball)
    {
        return (ball.transform.position.x > 0 && ball.rigidbody.velocity.x < 0) || (ball.transform.position.x < 0 && ball.rigidbody.velocity.x > 0);
        // is the ball on the right side and moving to the left OR is the ball on the left side and movign to the right
    }

    private bool gameOver = false;

    private IEnumerator GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;

            for (int i = 0; i < 3; i++)
            {

                customInput.setLight(0, 1);

                yield return new WaitForSeconds(0.1f);

                customInput.setLight(0, 0);

                customInput.setLight(1, 1);

                yield return new WaitForSeconds(0.1f);

                customInput.setLight(1, 0);

                customInput.setLight(2, 1);

                yield return new WaitForSeconds(0.1f);

                customInput.setLight(2, 0);

                customInput.setLight(3, 1);

                yield return new WaitForSeconds(0.1f);

                customInput.setLight(3, 0);
            }

            yield return new WaitForSeconds(1);
            game.player1GameScore = 0;
            game.player2GameScore = 0;

            Application.LoadLevel("pong");
        }
    }

    private IEnumerator Rumble()
    {
        customInput.setMotor(rumblePower);

        yield return new WaitForSeconds(0.2f);

        customInput.setMotor(0);
    }

    public GameObject AddBall(Vector3 pos)
    {
        if (ballsIntern.Count < maxAmountOfBalls)
        {
            ballSpin.Add(0);
            GameObject ball = (GameObject)Instantiate(ballPrefab, pos, ballPrefab.transform.rotation);
            ballsIntern.Add(ball);
            ball.rigidbody.AddForce(Vector3.right * -startSpeed);
            startSpeed *= -1;
            return ball;
        }
        return null;
    }

    public GameObject AddBall(Vector3 pos, Vector3 vel)
    {
        if (ballsIntern.Count < maxAmountOfBalls)
        {
            ballSpin.Add(0);
            GameObject ball = (GameObject)Instantiate(ballPrefab, pos, ballPrefab.transform.rotation);
            ball.rigidbody.velocity = vel;

            ballsIntern.Add(ball);
            return ball;
        }
        return null;
    }

    void OnApplicationQuit()
    {
        customInput.setMotor(0);

        customInput.setLight(0, 0);
        customInput.setLight(1, 0);
        customInput.setLight(2, 0);
        customInput.setLight(3, 0);
    }

    public List<GameObject> getBalls()
    {
        return ballsIntern;
    }

    public float getMaxBatPos()
    {
        return maxBatPos;
    }
}