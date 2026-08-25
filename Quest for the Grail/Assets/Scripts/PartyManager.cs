using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// when AddMemberToPartyByName function is called, and a party member's name is passed through the function, the array containing every party member in the game is looped
// when the party member name in our array is equal to the name we input into the function, this means we have matched the party member. 
// make a new party member and assign all of it's data to something we can change, and then adds this party member to our current party. 
public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty; 

    [SerializeField] private PartyMemberInfo defaultPartyMember;

    private void Awake()
    {
        AddMemberToPartyByName(defaultPartyMember.MemberName);
    }

    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMembers.Length; i++)
        {
            if(allMembers[i].MemberName == memberName)
            {
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.MemberName = allMembers[i].MemberName;
                newPartyMember.Level = allMembers[i].StartingLevel;
                newPartyMember.CurrHealth = allMembers[i].BaseHealth;
                newPartyMember.MaxHealth = newPartyMember.CurrHealth;
                newPartyMember.Strength = allMembers[i].BaseStr;
                newPartyMember.Initiative = allMembers[i].BaseInitiative;
                newPartyMember.MemberBattleVisualPrefab = allMembers[i].MemberBattleVisualPrefab;
                newPartyMember.MemberOverworldVisualPrefab = allMembers[i].MemberOverworldVisualPrefab;

                currentParty.Add(newPartyMember);
            }
        }
    }

    public List<PartyMember> GetCurrentParty()
    {
        return currentParty;
    }
}

[System.Serializable]
public class PartyMember
{
    public string MemberName;
    public int Level;
    public int CurrHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public int CurrExp;
    public int MaxExp;
    public GameObject MemberBattleVisualPrefab;
    public GameObject MemberOverworldVisualPrefab;

}
