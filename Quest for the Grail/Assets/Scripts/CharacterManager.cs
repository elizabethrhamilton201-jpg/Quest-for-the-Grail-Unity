using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject joinPopup;
    [SerializeField] private TextMeshProUGUI joinPopupText;

    private bool infrontOfPartyMember;
    private GameObject joinableMember;
    private PlayerControls playerControls;
    private List<GameObject> overworldCharacters = new List<GameObject>();

    private const string PARTY_JOINED_MESSAGE = " joined the party!";
    private const string NPC_JOINABLE_TAG = "NPCJoinable";

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControls.Player.Interact.performed += _ => Interact();
        SpawnOverworldMembers();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Interact()
    {
        if(infrontOfPartyMember == true && joinableMember != null)
        {
            // add member
            MemberJoined(joinableMember.GetComponent<JoinableCharacterScript>().MemberToJoin);
            infrontOfPartyMember = false;
            joinableMember = null;
        }
    }

    private void MemberJoined(PartyMemberInfo partyMember)
    {
        // add party member
        GameObject.FindAnyObjectByType<PartyManager>().AddMemberToPartyByName(partyMember.MemberName);
        // disable joinable member
        joinableMember.GetComponent<JoinableCharacterScript>().CheckIfJoined();
        // join pop up
        joinPopup.SetActive(true);
        joinPopupText.text = partyMember.MemberName + PARTY_JOINED_MESSAGE;
        // adding an overworld member
        SpawnOverworldMembers();
    }
    
    private void SpawnOverworldMembers()
    {
        for (int i = 0; i < overworldCharacters.Count; i++)
        {
            Destroy(overworldCharacters[i]);
        }
        overworldCharacters.Clear();
        
        List<PartyMember> currentParty = GameObject.FindAnyObjectByType<PartyManager>().GetCurrentParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            if(i == 0) // first member will be the player
            {
                // get player 
                GameObject player = gameObject;
                // spawn the member visual
                GameObject playerVisual = Instantiate(currentParty[i].MemberOverworldVisualPrefab, player.transform.position, Quaternion.identity);

                playerVisual.transform.SetParent(player.transform);
                // assign the player controller values
                player.GetComponent<PlayerController>().SetOverworldVisuals(playerVisual.GetComponent<Animator>(), playerVisual.GetComponent<SpriteRenderer>(), playerVisual.transform.localScale);
                playerVisual.GetComponent<MemberFollowAI>().enabled = false;
                // add the overworld character visual to the list
                overworldCharacters.Add(playerVisual);
            }
            else // any other will be a follower
            {   
                // get the followers spawn position
                Vector3 positionToSpawn = transform.position;
                positionToSpawn.x -= 1;

                // spawn the follower
                GameObject tempFollower = Instantiate(currentParty[i].MemberOverworldVisualPrefab, positionToSpawn, Quaternion.identity);

                // set follow ai settings
                tempFollower.GetComponent<MemberFollowAI>().SetFollowDistance(i);

                // add the follower visual to our list
                overworldCharacters.Add(tempFollower);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            // enable prompt
            infrontOfPartyMember = true;
            joinableMember = other.gameObject;
            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(true);
        }
    }   

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            // disable prompt
            infrontOfPartyMember = false;
            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(false);
            joinableMember = null;
        }
    }

}
