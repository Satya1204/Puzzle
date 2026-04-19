namespace PuzzleApp.Features.MatchObjects
{
    public interface IMatchObjectsDataService
    {
        MatchObjectsItemPair[] GetRoundPairs(MatchObjectsDefinition definition);
    }
}
