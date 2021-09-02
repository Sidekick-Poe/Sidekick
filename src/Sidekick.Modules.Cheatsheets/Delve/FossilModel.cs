namespace Sidekick.Modules.Cheatsheets.Delve
{
    public class FossilModel
    {
        public FossilModel(string name, RewardValue value, bool behindFracturedWall = false, bool inFossilRoom = false)
        {
            Name = name;
            Value = value;
            BehindFracturedWall = behindFracturedWall;
            InFossilRoom = inFossilRoom;
        }

        public string Name { get; set; }
        public RewardValue Value { get; set; }
        public bool BehindFracturedWall { get; set; }
        public bool InFossilRoom { get; set; }
    }
}
