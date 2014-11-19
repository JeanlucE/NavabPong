using UnityEngine;
using System.Collections;

public class Game {
    
    private static Game instance;

    public static Game getInstance(){
        if(instance == null)
            instance = new Game();

        return instance;
    }

    public int GamesPlayed { get; set; }

    public int player1TotalScore { get; set; }

    public int player2TotalScore { get; set; }

    public int player1GameScore { get; set; }

    public int player2GameScore { get; set; }

	public int navabTotalScore { get; set;}

    public CustomInputScript.InputMethod selectedInputMethod;

	public int maxAmountOfBalls = 2;

    private Game(){
        
    }

    public string getText(string key) {
        if (key == "Player 1 Game")
            return "" + player1GameScore;
        else if (key == "Player 2 Game")
            return "" + player2GameScore;
        else if (key == "Player 1 Total")
            return "" + player1TotalScore;
        else if (key == "Player 2 Total")
            return "" + player2TotalScore;
        else
            return null;
    }
}
