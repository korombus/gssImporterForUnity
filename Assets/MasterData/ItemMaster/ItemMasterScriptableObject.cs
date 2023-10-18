using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemMaster", menuName = "ScriptableObjects/ItemMaster", order = 0)]
public class ItemMasterScriptableObject : ScriptableObject
{
    public List<ItemMaster> itemMasterList;
}
