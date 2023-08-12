using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    //UI
    public GameObject diffSelect, prefSelect, birdSelect, birdGrid;
    public Image prefBirdImg, birdImg;
    public TMP_Text prefText, reqText;
    public Color white, pink, yellow, orange, red, green, blue, black;
    private Color[] colors = new Color[8];
    //

    private bool hardMode = false, hardPreference = false;

    public List<Bird> birds = new List<Bird>();
    public List<int[]> bestSolutions = new List<int[]>();

    private int leastErrorsFound = -1;
    private int indexEvaluating = 0;
    private int birdManaging = 0;

    private void Start()
    {
        //put colors in a array to iterate over
        colors = new Color[8] { white, pink, yellow, orange, red, green, blue, black };

        //assign colors to bird select items
        Image[] gridItems = birdGrid.GetComponentsInChildren<Image>();
        for (int i = 0; i < gridItems.Length; i++)
        {
            gridItems[i].color = colors[i];
        }

        //Generate list of birds
        for (int i = 0; i < 8; i++)
        {
            Bird currentBird = new Bird((BirdTypes)i);
            birds.Add(currentBird);
        }

        //pre-assign hard preferences if game is on easy mode
        if (!hardMode)
        {
            foreach (Bird bird in birds)
            {
                bird.hardBirdPreference = PreferenceType.none;
                bird.hardOtherBird = null;
            }
        }
    }

    public void GetDifficulty(bool hard)
    {
        hardMode = hard;
        diffSelect.SetActive(false);
    }

    public void GetPreference(int pref)
    {
        PreferenceType preferenceType = (PreferenceType)pref;

        if (hardPreference)
        {
            birds[birdManaging].hardBirdPreference = preferenceType;
        }
        else
        {
            birds[birdManaging].easyBirdPreference = preferenceType;
        }

        //Does this preference refer to another bird?
        switch (preferenceType)
        {
            case PreferenceType.none:
            case PreferenceType.longSide:
            case PreferenceType.shortSide:
            case PreferenceType.boomerang:
            case PreferenceType.notBoomerang:
                //no
                IncrementBirdManaging();
                return;
                break;

            //yes
            case PreferenceType.shareCornerWith:
                prefText.text = "will share a corner with";
                break;

            case PreferenceType.sameAs:
                prefText.text = "wants the same as";
                break;

            case PreferenceType.oppositeAs:
                prefText.text = "wants the oppsite of";
                break;

            case PreferenceType.nextTo:
                prefText.text = "will sit next to";
                break;

            case PreferenceType.acrossFrom:
                prefText.text = "will sit across from";
                break;

            case PreferenceType.twoSpacesFrom:
                prefText.text = "will be at least two spaces from";
                break;

            case PreferenceType.neitherNextToNorAcrossFrom:
                prefText.text = "will not sit across from nor next to";
                break;
        }

        prefSelect.SetActive(false);
    }

    public void GetOtherBird(int typ)
    {
        BirdTypes type = (BirdTypes)typ;

        Bird otherBird = birds.FirstOrDefault(b => b.type == type);

        if (hardPreference)
        {
            birds[birdManaging].hardOtherBird = otherBird;
        }
        else
        {
            birds[birdManaging].easyOtherBird = otherBird;
        }

        IncrementBirdManaging();
    }

    private void IncrementBirdManaging()
    {
        reqText.text = "Preference 1";

        if (hardMode)
        {
            if (hardPreference)
            {
                birdManaging++;
                hardPreference = false;
            }
            else
            {
                hardPreference = true;
                reqText.text = "Preference 2";
            }
        }
        else
        {
            birdManaging++;
        }

        if (birdManaging < 8)
        {
            prefBirdImg.color = colors[birdManaging];
            birdImg.color = colors[birdManaging];
            prefSelect.SetActive(true);
        }
        else
        {
            //done
            prefSelect.SetActive(false);
            birdSelect.SetActive(false);
        }
    }

    public void Solve(int currentIndex, int currentBird, Bird[] remainingBirds)
    {
        //assign next index to all birds

        //evaluate all birds, count errors

        //evaluate error count

        //repeat untill finished (count solutions to ensure all 8! hav been evaluated?)
    }
}