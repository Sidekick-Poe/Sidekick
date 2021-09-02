namespace Sidekick.Modules.Cheatsheets.Betrayal
{
    public class AgentModel
    {
        public AgentModel(string name, string image)
        {
            Name = name;
            Image = image;
        }

        public string Name { get; set; }

        public string Image { get; set; }

        public RewardModel Transportation { get; set; }

        public RewardModel Fortification { get; set; }

        public RewardModel Research { get; set; }

        public RewardModel Intervention { get; set; }
    }
}
