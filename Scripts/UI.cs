using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    //Misc
    public Image currentBirdImg_Pref, currentBirdImg_OtherBird;
    public TMP_Text currentPrefTxt, prefDescriptionTxt, errorCount_Manual, errorCount_Brute, timeTxt, currentSolutionTxt;
    //

    public List<GameObject> menus;
    private GameManager gameManager;

    public List<Color> colors;
    public Color neutralColor, disabledColor;

    public List<Image> boardPositionImgs_Manual, boardPositionImgs_Brute, otherBirds, birdPawns;

    private bool hardSettings = false;
    private int currentBird = 0, manualCurrentSpot = 0, solutionViewing = 0;
    private int[] manualSolution = new int[8];

    // Start is called before the first frame update
    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        SwitchMenu(0); //start at the first menu

        //assign colors to bird select items
        for (int i = 0; i < otherBirds.Count(); i++)
        {
            otherBirds[i].color = colors[i];
        }

        for (int i = 0; i < birdPawns.Count(); i++)
        {
            birdPawns[i].color = colors[i];
        }
    }

    public void SwitchMenu(int menu)
    {
        //Sets the chosen menu as the only active one
        for (int i = 0; i < menus.Count; i++)
        {
            if (i == menu) menus[i].SetActive(true);
            else menus[i].SetActive(false);
        }
    }

    private void ChangeText(TMP_Text text, string newText)
    {
        //change both the shadow and the main text of a text set
        foreach (TMP_Text txt in text.gameObject.GetComponentsInChildren<TMP_Text>())
        {
            txt.text = newText;
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void NextBird()
    {
        if (hardSettings) //if currently managing hard settings
        {
            //move on to the next bird
            currentBird++;
            hardSettings = false;
            ChangeText(currentPrefTxt, "Preference 1");
        }
        else
        {
            if (gameManager.hardMode)
            {
                hardSettings = true; //switch to hard settings if hard mode was selected
                ChangeText(currentPrefTxt, "Preference 2");
            }
            else
            {
                currentBird++;
            }
        }

        if (currentBird >= 8) //no more birds to go
        {
            SwitchMenu(3);
        }
    }

    public void SwitchSolution(int dir)
    {
        solutionViewing += dir;
        solutionViewing = solutionViewing >= gameManager.bestSolutions.Count ? 0 : solutionViewing;
        solutionViewing = solutionViewing < 0 ? gameManager.bestSolutions.Count - 1 : solutionViewing;

        ChangeText(currentSolutionTxt, "solution " + (solutionViewing + 1) + " of " + gameManager.bestSolutions.Count());

        //Display new solution
        for (int i = 0; i < boardPositionImgs_Brute.Count; i++)
        {
            boardPositionImgs_Brute[i].color = colors[gameManager.bestSolutions[solutionViewing][i]];
        }
    }

    public void SwitchToBruteForce()
    {
        SwitchMenu(4);
        BruteForce();
    }

    public void EvaluateNew()
    {
        ChangeText(errorCount_Manual, "");
        foreach (Image birdPawnImg in birdPawns)
        {
            birdPawnImg.gameObject.SetActive(true);
        }
        manualCurrentSpot = 0;
        manualSolution = new int[8];

        foreach (Image spotImg in boardPositionImgs_Manual)
        {
            spotImg.color = disabledColor;
            spotImg.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        boardPositionImgs_Manual[0].color = neutralColor;
    }

    //************************************** Game Progression **************************************
    public void GetDifficulty(bool hard)
    {
        //save difficulty in gamemanager
        gameManager.hardMode = hard;

        //Continue to the next page (preference select)
        currentBirdImg_Pref.color = colors[currentBird];
        SwitchMenu(1);
    }

    public void GetPreference(int pref)
    {
        if (currentBird >= 8) return;

        PreferenceType preferenceType = (PreferenceType)pref;

        if (hardSettings)
        {
            gameManager.birds[currentBird].hardBirdPreference = preferenceType;
        }
        else
        {
            gameManager.birds[currentBird].easyBirdPreference = preferenceType;
        }

        //Does this preference refer to another bird?
        switch (preferenceType)
        {
            //no
            case PreferenceType.none:
            case PreferenceType.longSide:
            case PreferenceType.shortSide:
            case PreferenceType.boomerang:
            case PreferenceType.notBoomerang:
                NextBird();
                if (currentBird >= 8) return;
                currentBirdImg_Pref.color = colors[currentBird];
                return;

            //yes
            case PreferenceType.shareCornerWith:
                ChangeText(prefDescriptionTxt, "want to share a corner with");
                break;

            case PreferenceType.sameAs:
                ChangeText(prefDescriptionTxt, "wants the same as");
                break;

            case PreferenceType.oppositeAs:
                ChangeText(prefDescriptionTxt, "wants the oppsite of");
                break;

            case PreferenceType.nextTo:
                ChangeText(prefDescriptionTxt, "want to sit next to");
                break;

            case PreferenceType.acrossFrom:
                ChangeText(prefDescriptionTxt, "want to sit across from");
                break;

            case PreferenceType.twoSpacesFrom:
                ChangeText(prefDescriptionTxt, "want to be at least two spaces from");
                break;

            case PreferenceType.neitherNextToNorAcrossFrom:
                ChangeText(prefDescriptionTxt, "does not want to sit across from nor next to");
                break;
        }

        currentBirdImg_OtherBird.color = colors[currentBird];
        SwitchMenu(2); //bird select
    }

    public void GetOtherBird(int typ)
    {
        BirdTypes type = (BirdTypes)typ;
        Bird otherBird = gameManager.birds.FirstOrDefault(b => b.type == type); //always find the right bird

        //assign other bird
        if (hardSettings)
        {
            gameManager.birds[currentBird].hardOtherBird = otherBird;
        }
        else
        {
            gameManager.birds[currentBird].easyOtherBird = otherBird;
        }

        NextBird();
        if (currentBird >= 8) return;
        currentBirdImg_Pref.color = colors[currentBird];
        SwitchMenu(1);
    }

    public void GetSolveMode(bool bruteFoce)
    {
        if (bruteFoce)
        {
            SwitchMenu(4);
            BruteForce();
        }
        else
        {
            boardPositionImgs_Manual[0].color = neutralColor;
            SwitchMenu(5);
        }
    }

    public void PlaceBird(int bird)
    {
        EventSystem.current.currentSelectedGameObject.SetActive(false); //Disable the placed bird
        boardPositionImgs_Manual[manualCurrentSpot].color = colors[bird]; //set the color of the current position
        manualSolution[manualCurrentSpot] = bird;
        manualCurrentSpot++;

        if (manualCurrentSpot >= 8)
        {
            Evaluate();
            return;
        }

        boardPositionImgs_Manual[manualCurrentSpot].color = neutralColor;
    }

    private void Evaluate()
    {
        //assign index to all birds
        for (int i = 0; i < manualSolution.Length; i++)
        {
            gameManager.birds[manualSolution[i]].position = i;
        }

        //Evaluate and count errors
        int errors = 0;
        foreach (Bird bird in gameManager.birds)
        {
            if (!Preferences.EvaluateRequirement(bird.easyBirdPreference, bird, bird.easyOtherBird))
            {
                errors++;
                boardPositionImgs_Manual[bird.position].rectTransform.GetChild(0).gameObject.SetActive(true);
            }
            if (gameManager.hardMode)
            {
                if (!Preferences.EvaluateRequirement(bird.hardBirdPreference, bird, bird.hardOtherBird))
                {
                    errors++;
                    boardPositionImgs_Manual[bird.position].rectTransform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

        //Display error count
        ChangeText(errorCount_Manual, "Your solution contains " + errors + " errors");
    }

    private void BruteForce()
    {
        List<int> allBirds = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

        var solutionStartTime = System.DateTime.Now;
        gameManager.Solve(allBirds, new List<int>());
        var solutionEndTime = System.DateTime.Now;

        SwitchSolution(0);

        ChangeText(errorCount_Brute, "Least amount of errors is " + gameManager.leastErrorsFound);
        ChangeText(timeTxt, "evaluated " + gameManager.arrangementsEvaluated + " arrangements in " + (solutionEndTime - solutionStartTime).TotalMilliseconds + " ms");
    }
}