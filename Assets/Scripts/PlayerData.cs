using System;

[Serializable]
public class PlayerData
{
    public int[] levelOffsetArray;
    public int[] completeArray;
    public int[] movesPerfectArray;
    public int[] movesTakenArray;
    public int[] starsArray;
    public int[] timeTakenArray;
    public int[] attemptsTakenArray;
    public int[] hintsRemainingArray;
    public long[] hintsLastTimeArray;
    public int[] postcardCompleteStateArray;

    public PlayerData(int levelsPerPack, int numPacks)
    {
        levelOffsetArray = new int[levelsPerPack * numPacks];
        completeArray = new int[levelsPerPack * numPacks];
        movesPerfectArray = new int[levelsPerPack * numPacks];
        movesTakenArray = new int[levelsPerPack * numPacks];
        starsArray = new int[levelsPerPack * numPacks];
        timeTakenArray = new int[levelsPerPack * numPacks];
        attemptsTakenArray = new int[levelsPerPack * numPacks];
        hintsRemainingArray = new int[levelsPerPack * numPacks];
        hintsLastTimeArray = new long[levelsPerPack * numPacks];
        postcardCompleteStateArray = new int[numPacks];
    }
}


