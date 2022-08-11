using FC.Api.Helpers;
using FC.Api.Validators.Item;
using FC.Core.Domain.Items;
using FluentValidation.Attributes;
using System;

namespace FC.Api.DTOs.Item
{

    [Validator(typeof(ItemAttributeDTOValidator))]
    public class ItemAttributeDTO
    {

        public int Id { get; set; }

        public Purpose1Enum? Purpose1 { get; set; }

        public string Purpose1Str
        {
            get
            {
                if (this.Purpose1.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(Purpose1Enum), this.Purpose1));
                }

                return null;
            }
        }

        public Purpose2Enum? Purpose2 { get; set; }

        public string Purpose2Str
        {
            get
            {
                if (this.Purpose2.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(Purpose2Enum), this.Purpose2));
                }

                return null;
            }
        }

        public TrafficEnum? Traffic { get; set; }

        public string TrafficStr
        {
            get
            {
                if (this.Traffic.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(TrafficEnum), this.Traffic));
                }

                return null;
            }
        }

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
