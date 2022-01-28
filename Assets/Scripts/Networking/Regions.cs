public enum Regions
{
    Auto,
    Asia,
    Europe,
    SouthAmerica,
    UsEast,
    UsWest
}

public static class RegionExtensions
{
    public static string GetRegionCode(this Regions region)
    {
        switch (region)
        {
            case Regions.Auto:
                return string.Empty;

            case Regions.Asia:
                return "asia";
            case Regions.Europe:
                return "eu";
            case Regions.SouthAmerica:
                return "sa";
            case Regions.UsEast:
                return "us";
            case Regions.UsWest:
                return "usw";

            default:
                throw new System.NotImplementedException();
        }
    }
}
