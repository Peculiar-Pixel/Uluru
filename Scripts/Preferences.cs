using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public enum PreferenceType
{
    none,
    sameAs,
    oppositeAs,
    nextTo,
    shareCornerWith,
    acrossFrom,
    twoSpacesFrom,
    neitherNextToNorAcrossFrom,
    longSide,
    shortSide,
    boomerang,
    notBoomerang
}

public static class Preferences
{
    public static bool EvaluateRequirement(PreferenceType preferenceType, Bird thisBird, Bird? otherBird = null)
    {
        //IMPORTANT: preference type argument seems redundant but it isn't in the case of "same as" or "opposite as" preferences,
        //as these are the preferences of a third bird, in relation to the original bird with aforementioned preferences (it would
        //thus be incorrect to get the preference type from 'thisBird', because 'thisBird' references another bird not passed as an argument)

        if (thisBird == otherBird) return true; //Avoid references to self

        switch (preferenceType) //Preferences that does not require another bird
        {
            case PreferenceType.none:
                return true; //Birds with no preference are always happy

            case PreferenceType.longSide:
                return LongSide(thisBird);

            case PreferenceType.shortSide:
                return ShortSide(thisBird);

            case PreferenceType.boomerang:
                return Boomerang(thisBird);

            case PreferenceType.notBoomerang:
                return NotBoomerang(thisBird);
        }

        if (otherBird == null)
        {
            Debug.Log("Missing other bird");
            return false;
        }

        switch (preferenceType) //Preferences that do require another bird
        {
            case PreferenceType.sameAs:
                return SameAs(thisBird, otherBird);

            case PreferenceType.oppositeAs:
                return OppositeAs(thisBird, otherBird);

            case PreferenceType.nextTo:
                return NextTo(thisBird, otherBird);

            case PreferenceType.shareCornerWith:
                return ShareCornerWith(thisBird, otherBird);

            case PreferenceType.acrossFrom:
                return AcrossFrom(thisBird, otherBird);

            case PreferenceType.twoSpacesFrom:
                return TwoSpacesFrom(thisBird, otherBird);

            case PreferenceType.neitherNextToNorAcrossFrom:
                return NeitherNextToNorAcrossFrom(thisBird, otherBird);
        }

        Debug.Log("Unhandled requirement");
        return false;
    }

    private static bool SameAs(Bird thisBird, Bird otherBird)
    {
        bool result = true;

        if (!EvaluateRequirement(otherBird.easyBirdPreference, thisBird, otherBird)) result = false;
        if (!EvaluateRequirement(otherBird.hardBirdPreference, thisBird, otherBird)) result = false;

        return result;
    }

    private static bool OppositeAs(Bird thisBird, Bird otherBird)
    {
        return !SameAs(thisBird, otherBird);
    }

    private static bool NextTo(Bird thisBird, Bird otherBird)
    {
        switch (thisBird.position)
        {
            case 0:
                return false; //no birds can sit next to a bird sat at index 0

            case 1:
                if (otherBird.position == 2) return true;
                else return false;

            case 2:
                if (otherBird.position == 1) return true;
                else return false;

            case 3:
                if (otherBird.position == 4) return true;
                else return false;

            case 4:
                if (otherBird.position == 3) return true;
                else return false;

            case 5:
                if (otherBird.position == 6) return true;
                else return false;

            case 6:
                if (otherBird.position == 7 || otherBird.position == 5) return true;
                else return false;

            case 7:
                if (otherBird.position == 6) return true;
                else return false;

            default:
                Debug.Log("NextTo switch case evaluated to default (invalid bird index)");
                return false;
        }
    }

    private static bool ShareCornerWith(Bird thisBird, Bird otherBird)
    {
        switch (thisBird.position)
        {
            case 0:
                if (otherBird.position == 1 || otherBird.position == 7) return true;
                else return false;

            case 1:
                if (otherBird.position == 0) return true;
                else return false;

            case 2:
                if (otherBird.position == 3) return true;
                else return false;

            case 3:
                if (otherBird.position == 2) return true;
                else return false;

            case 4:
                if (otherBird.position == 5) return true;
                else return false;

            case 5:
                if (otherBird.position == 4) return true;
                else return false;

            case 6:
                return false; //no birds can share a corner with a bird sat at index 6

            case 7:
                if (otherBird.position == 0) return true;
                else return false;

            default:
                Debug.Log("ShareCornerWith switch case evaluated to default (invalid bird index)");
                return false;
        }
    }

    private static bool AcrossFrom(Bird thisBird, Bird otherBird)
    {
        switch (thisBird.position)
        {
            case 0:
                if (otherBird.position == 3 || otherBird.position == 4) return true;
                else return false;

            case 1:
                if (otherBird.position == 7 || otherBird.position == 6) return true;
                else return false;

            case 2:
                if (otherBird.position == 5 || otherBird.position == 6) return true;
                else return false;

            case 3:
                if (otherBird.position == 0) return true;
                else return false;

            case 4:
                if (otherBird.position == 0) return true;
                else return false;

            case 5:
                if (otherBird.position == 2) return true;
                else return false;

            case 6:
                if (otherBird.position == 1 || otherBird.position == 2) return true;
                else return false;

            case 7:
                if (otherBird.position == 1) return true;
                else return false;

            default:
                Debug.Log("AcrossFrom switch case evaluated to default (invalid bird index)");
                return false;
        }
    }

    private static bool TwoSpacesFrom(Bird thisBird, Bird otherBird)
    {
        for (int jumps = 0; jumps > 5; jumps++)
        {
            if (otherBird.position == (thisBird.position + jumps) % 8)
            {
                if (jumps < 2) return false;
                else return true;
            }
        }
        return false;
    }

    private static bool NeitherNextToNorAcrossFrom(Bird thisBird, Bird otherBird)
    {
        bool result = true;

        if (NextTo(thisBird, otherBird)) result = false;
        if (AcrossFrom(thisBird, otherBird)) result = false;

        return result;
    }

    private static bool LongSide(Bird thisBird)
    {
        return !ShortSide(thisBird);
    }

    private static bool ShortSide(Bird thisBird)
    {
        if (thisBird.position == 0 || thisBird.position == 3 || thisBird.position == 4) return true;
        return false;
    }

    private static bool Boomerang(Bird thisBird)
    {
        return !NotBoomerang(thisBird);
    }

    private static bool NotBoomerang(Bird thisBird)
    {
        if (thisBird.position >= 0 && thisBird.position <= 2) return true;
        return false;
    }
}