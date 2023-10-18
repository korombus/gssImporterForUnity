using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UniRx;
using System.Linq;

public class MasterGeneretor : EditorWindow
{
    /// <summary>
    /// マスター読み出し用のスキーマ
    /// </summary>
    public struct SchemaForMasterReadout
    {
        /// <summary>
        /// マスターデータ名
        /// </summary>
        public string name;
        /// <summary>
        /// マスターデータのScriptableObject
        /// </summary>
        public ScriptableObject masterDataSO;
        /// <summary>
        /// 読み出しフラグ
        /// </summary>
        public bool isSet;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="i_name"></param>
        /// <param name="i_SO"></param>
        /// <param name="i_isSet"></param>
        public SchemaForMasterReadout(string i_name, ScriptableObject i_SO, bool i_isSet)
        {
            this.name = i_name;
            this.masterDataSO = i_SO;
            this.isSet = i_isSet;
        }
    }

    /// <summary>
    /// データベース一覧
    /// </summary>
    private List<SchemaForMasterReadout> databases = new List<SchemaForMasterReadout>();

    /// <summary>
    /// 読み込み完了通知用RP
    /// </summary>
    private BoolReactiveProperty currentIsLoadMasterData = new BoolReactiveProperty(false);

    [MenuItem("Tools/データベース生成", false, 1)]
    private static void ShowWindow()
    {
        MasterGeneretor window = GetWindow<MasterGeneretor>();
        window.titleContent = new GUIContent("データベース生成");
    }

    private async void OnEnable()
    {
        List<string> masterNameList = Masters.GetAllMasterDataName();
        foreach(string masterName in masterNameList)
        {
            ScriptableObject data = await Addressables.LoadAssetAsync<ScriptableObject>(masterName).Task;
            databases.Add(
                new SchemaForMasterReadout(masterName, data, false)
            );
        }
        currentIsLoadMasterData.Value = true;
    }

    private void OnDisable()
    {
        currentIsLoadMasterData.Value = false;    
    }

    private void OnGUI()
    {
        // データベースの読み込みが完了するまで待機
        if(currentIsLoadMasterData.Value)
        {
            GUILayout.Label("データベース一覧");
            databases.ForEach(d => 
            {
                GUILayout.Label(d.name);
            });

            GUILayout.Space(20f);

            if (GUILayout.Button("生成"))
            {
                GenerateMaster();
            }
        }
        else
        {
            GUILayout.Label("読み込み中…");
        }
    }

    /// <summary>
    /// データベース生成
    /// </summary>
    private async void GenerateMaster()
    {
        // すべてのマスターデータを取得して、ScriptableObjectに入れ込む
        List<string> masterNameList = Masters.GetAllMasterDataName();
        Dictionary<string, string> masterDataUrls = Masters.GetAllMasterDataUrl();

        foreach (string masterName in masterNameList)
        {
            string url = masterDataUrls[masterName];
            string json = await GetMasterData(url);

            LoadMasterToSO(json, masterName);
        }
    }

    /// <summary>
    /// データベースのデータをScriptableObjectに反映
    /// </summary>
    /// <param name="json">マスターデータのjson</param>
    /// <param name="masterName">対象のマスター名</param>
    private void LoadMasterToSO(string json, string masterName)
    {
        switch (masterName)
        {
            case "ItemMaster":
                List<ItemMaster> itemMasterList = JsonHelper.FromJson<ItemMaster>(json).ToList();
                ItemMasterScriptableObject itemMasterSO = (ItemMasterScriptableObject)databases.FirstOrDefault(d => d.name == masterName).masterDataSO;
                itemMasterSO.itemMasterList = itemMasterList;
            break;

            case "MonsterMaster":
                List<MonsterMaster> monsterMasterList = JsonHelper.FromJson<MonsterMaster>(json).ToList();
                MonsterMasterScriptableObject monsterMasterSO = (MonsterMasterScriptableObject)databases.FirstOrDefault(d => d.name == masterName).masterDataSO;
                monsterMasterSO.monsterMasterList = monsterMasterList;
            break;
        }
    }

    /// <summary>
    /// GoogleSpreadSheetからjson形式でマスターデータを取得
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private async Task<string> GetMasterData(string url)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        await req.SendWebRequest();

        return req.downloadHandler.text;
    }
}
