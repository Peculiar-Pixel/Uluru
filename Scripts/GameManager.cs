using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    //UI
    public GameObject diffSelect, prefSelect, birdSelect, solution, birdGrid;
    public Image prefBirdImg, birdImg;
    public List<Image> solutionPlaces;
    public TMP_Text prefText, reqText;
    public Color white, pink, yellow, orange, red, green, blue, black;
    private Color[] colors = new Color[8];
    //

    private bool hardMode = false, hardPreference = false;

    public List<Bird> birds = new List<Bird>();
    public List<List<int>> bestSolutions = new List<List<int>>();

    private int leastErrorsFound = -1, arrangementsEvaluated = 0;
    private int birdManaging = 0;

    private void Start()
    {
        //put colors in a array to iterate over (order matters!)
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
        prefSelect.SetActive(true);
        birdSelect.SetActive(false);
        solution.SetActive(false);
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

        diffSelect.SetActive(false);
        prefSelect.SetActive(false);
        birdSelect.SetActive(true);
        solution.SetActive(false);
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

            diffSelect.SetActive(false);
            prefSelect.SetActive(true);
            birdSelect.SetActive(false);
            solution.SetActive(false);
        }
        else
        {
            //done
            diffSelect.SetActive(false);
            prefSelect.SetActive(false);
            birdSelect.SetActive(false);
            solution.SetActive(true);

            List<int> allBirds = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            Solve(allBirds, new List<int>());

            for (int i = 0; i < solutionPlaces.Count(); i++)
            {
                solutionPlaces[i].color = colors[bestSolutions[0][i]]; //for now, just the first found solution
            }
            Debug.Log("Tested " + arrangementsEvaluated + " arrangements");
        }
    }

    public void Solve(List<int> remainingBirds, List<int> birdIndexes)
    {
        if (remainingBirds.Count() == 0)
        {
            //All birds are placed, solve
            arrangementsEvaluated++; //count evaluations

            //assign index to all birds
            for (int i = 0; i < birdIndexes.Count; i++)
            {
                birds[birdIndexes[i]].position = i;
            }

            //Evaluate and count errors

            int errors = 0;
            foreach (Bird bird in birds)
            {
                if (!Preferences.EvaluateRequirement(bird.easyBirdPreference, bird, bird.easyOtherBird))
                {
                    errors++;
                }
                if (hardMode)
                {
                    if (!Preferences.EvaluateRequirement(bird.hardBirdPreference, bird, bird.hardOtherBird))
                    {
                        errors++;
                    }
                }
            }

            if (errors < leastErrorsFound || leastErrorsFound == -1)
            {
                leastErrorsFound = errors;
                bestSolutions.Clear();
                bestSolutions.Add(birdIndexes);
            }
            else if (errors == leastErrorsFound)
            {
                bestSolutions.Add(birdIndexes);
            }
        }

        //Generate next unique arrangement of numbers 0-7
        else
        {
            for (int i = 0; i < remainingBirds.Count(); i++)
            {
                //generate a copy of lists
                List<int> newRemainingBirds = remainingBirds.ToList();
                List<int> birdIndexExtended = birdIndexes.ToList();

                birdIndexExtended.Add(remainingBirds[i]);
                newRemainingBirds.Remove(remainingBirds[i]);
                Solve(newRemainingBirds, birdIndexExtended);
            }
        }
    }
}