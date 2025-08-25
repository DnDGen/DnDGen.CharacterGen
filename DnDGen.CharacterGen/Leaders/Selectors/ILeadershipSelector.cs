namespace DnDGen.CharacterGen.Leaders.Selectors
{
    internal interface ILeadershipSelector
    {
        int SelectCohortLevelFor(int leadershipScore);
        FollowerQuantities SelectFollowerQuantitiesFor(int leadershipScore);
    }
}
