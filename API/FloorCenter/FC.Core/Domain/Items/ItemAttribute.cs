namespace FC.Core.Domain.Items
{
    public class ItemAttribute : BaseEntity
    {

        public Purpose1Enum? Purpose1 { get; set; }

        public Purpose2Enum? Purpose2 { get; set; }

        public TrafficEnum? Traffic { get; set; }

        public string PrintTech { get; set; }

        public string WaterAbs { get; set; }

        public string Thickness { get; set; }

        public int Material { get; set; }

        public int Type { get; set; }

        public int SubType { get; set; }

        public int CakeLayer { get; set; }

        public int SurfaceFin { get; set; }

        public bool? Rectified { get; set; }

        public int Feature { get; set; }

        public string BreakageStrength { get; set; }

        public string SlipResistance { get; set; }

        public int Weight { get; set; }

        public int NoOfPattern { get; set; }

    }
}
