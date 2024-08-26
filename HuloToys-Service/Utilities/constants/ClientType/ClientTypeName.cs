namespace HuloToys_Service.Utilities.constants.ClientType
{
    public static class ClientTypeName
    {
        public static readonly Dictionary<Int16, string> service = new Dictionary<Int16, string>
        {
            {Convert.ToInt16(ClientType.AGENT), "DL" },
            {Convert.ToInt16(ClientType.TIER_1_AGENT), "CL" },
            {Convert.ToInt16(ClientType.TIER_2_AGENT), "DL" },
            {Convert.ToInt16(ClientType.TIER_3_AGENT), "DL" },
            {Convert.ToInt16(ClientType.CUSTOMER), "KL" },
            {Convert.ToInt16(ClientType.SALE), "SL" },
            {Convert.ToInt16(ClientType.ENTERPRISE), "DN" },
            {Convert.ToInt16(ClientType.COLLABORATORS), "CT" }
        };
    }
}
