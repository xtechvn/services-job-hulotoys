using static HuloToys_Service.Utilities.constants.NinjaVan.ShippingFeeModel;

namespace HuloToys_Service.Utilities.constants.NinjaVan
{
    public static class NinjaVanShippingFee
    {
        public static int WareHouse_Province_id = 1;
        public static List<int> AREA_NORTH_ProvinceId = new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
        public static List<int> AREA_CENTER_ProvinceId = new List<int>() { 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44 };
        public static List<int> AREA_SOUTH_ProvinceId = new List<int>() { 45, 46, 47, 48, 49, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63 };
        public static int HCM_ProvinceId = 50;
        public static List<ShippingFeeModel> FEE = new List<ShippingFeeModel>
        {
            //--IN_TOWN
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.UNDER_500G, amount=15 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.UNDER_1KG, amount=16 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.UNDER_2KG, amount=16 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.UNDER_4KG, amount=17 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.UNDER_5KG, amount=24 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.UNDER_7KG, amount=33 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.UNDER_10KG, amount=43 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.UNDER_20KG, amount=77 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_TOWN,max_weight=(int)ShippingWeightStandardLevel.ADDITIONAL_1KG, amount=3 },

           //--IN_AREA
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_500G, amount=15 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_1KG, amount=16 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_2KG, amount=16 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_4KG, amount=17 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_5KG, amount=24 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_7KG, amount=34 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_10KG, amount=46 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_20KG, amount=97 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.IN_AREA,max_weight=(int)ShippingWeightStandardLevel.ADDITIONAL_1KG, amount=4 },

             //--BETWEEN_CITY (HN <-> HCM)
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.UNDER_500G, amount=16 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.UNDER_1KG, amount=16 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.UNDER_2KG, amount=18 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.UNDER_4KG, amount=22 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.UNDER_5KG, amount=40 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.UNDER_7KG, amount=47 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.UNDER_10KG, amount=67 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.UNDER_20KG, amount=177 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_CITY,max_weight=(int)ShippingWeightStandardLevel.ADDITIONAL_1KG, amount=7 },


              //--BETWEEN_AREA
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_500G, amount=17 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_1KG, amount=17 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_2KG, amount=21 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_4KG, amount=24 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_5KG, amount=40 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_7KG, amount=57 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_10KG, amount=70 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_20KG, amount=227 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_AREA,max_weight=(int)ShippingWeightStandardLevel.ADDITIONAL_1KG, amount=8 },

              //--BETWEEN_NEARBY_AREA
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_500G, amount=15 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_1KG, amount=16 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_2KG, amount=17 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_4KG, amount=20 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_5KG, amount=40 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_7KG, amount=57 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_10KG, amount=70 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.UNDER_20KG, amount=147 },
            new ShippingFeeModel(){ area=(int)ShippingAreaType.BETWEEN_NEARBY_AREA,max_weight=(int)ShippingWeightStandardLevel.ADDITIONAL_1KG, amount=6 },

        };
        public static int RATE = 1000;
    }
    public class ShippingFeeModel
    {
        public int area { get; set; }
        public int amount { get; set; }
        public int max_weight { get; set; }
    }
    public enum ShippingAreaType
    {
        IN_TOWN=1,
        IN_AREA=2,
        BETWEEN_CITY = 3,
        BETWEEN_AREA = 4,
        BETWEEN_NEARBY_AREA = 5
    }
    public enum ShippingWeightStandardLevel
    {
        UNDER_500G = 1,
        UNDER_1KG = 2,
        UNDER_2KG = 3,
        UNDER_4KG = 4,
        UNDER_5KG = 5,
        UNDER_7KG = 6,
        UNDER_10KG = 7,
        UNDER_20KG = 8,
        ADDITIONAL_1KG = 9,
    }
}
