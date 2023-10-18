using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterMaster", menuName = "ScriptableObjects/MonsterMaster", order = 1)]
public class MonsterMasterScriptableObject : ScriptableObject
{
    public List<MonsterMaster> monsterMasterList;
}
