using System.Collections;
using System.Collections.Generic;

public class Masters
{
    private const string DEPLOY_ID = "";
    private const string SPREAD_SHEET_URL_HEAD = "https://script.google.com/macros/s/";
    public static List<string> GetAllMasterDataName()
    {
        return new List<string>()
        {
            "ItemMaster",
            "MonsterMaster"
        };
    }

    public static Dictionary<string, string> GetAllMasterDataUrl()
    {
        Dictionary<string, string> masterDataUrlTable = new Dictionary<string, string>();
        foreach(string masterDataName in GetAllMasterDataName())
        {
            masterDataUrlTable.Add(masterDataName, SPREAD_SHEET_URL_HEAD + DEPLOY_ID + "/exec?name=" + masterDataName);
        }
        return masterDataUrlTable;
    }
}
