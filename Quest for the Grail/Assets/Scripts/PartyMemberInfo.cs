using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemberInfo : ScriptableObject
{
    public string MemberName;
    public int StartingLevel;
    public int BaseHealth;
    public int BaseStr;
    public int BaseInitiative;
    //what will be displayed in battle scene
    public GameObject MemberBattleVisualPrefab; 
    // what will be displayed in the overworld scene
    public GameObject MemberOverworldVisualPrefab;
}
