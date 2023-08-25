using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public bool hardMode = false;

    public List<Bird> birds = new List<Bird>();
    public List<List<int>> bestSolutions = new List<List<int>>();

    public int leastErrorsFound = -1, arrangementsEvaluated = 0;
    private int birdManaging = 0, solutionViewing = 0;

    private void Start()
    {
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

    public void Solve(List<int> remainingBirds, List<int> birdIndexes)
    {
        if (remainingBirds.Count == 0)
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
            for (int i = 0; i < remainingBirds.Count; i++)
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