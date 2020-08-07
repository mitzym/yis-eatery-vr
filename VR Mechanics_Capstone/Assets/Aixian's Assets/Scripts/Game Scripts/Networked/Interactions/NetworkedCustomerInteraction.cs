using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedCustomerInteraction : NetworkBehaviour
{
    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    [SyncVar]
    public int customerGroupSize;

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    #region Remote Methods
    //Pick up customer
    public void PickUpCustomer()
    {

        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.customer);

        //Get customer's group size
        CmdPickUpCustomer(networkedPlayerInteraction.detectedObject.GetComponent<CustomerBehaviour_Queueing>().groupSizeNum);

        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);

        RpcPickUpCustomer();
        

        //if inventory full, change state to holding customer
        if (networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingCustomer);
        }

    }

    //Seat customer
    public void SeatCustomer()
    {
        Debug.Log("NetworkedCustomerInteraction - Seat customer");

        CmdSeatCustomer(networkedPlayerInteraction.detectedObject, networkedPlayerInteraction.playerInventory);

    }

    //Taking customers orders
    public void CheckHandsEmpty()
    {
        Debug.Log("NetworkedCustomerInteraction - CheckHandsEmpty");
        CmdCheckHandsEmpty(networkedPlayerInteraction.detectedObject, networkedPlayerInteraction.IsInventoryFull());
    }

    //Spawning dishes
    public void SpawnDish()
    {

    }

    #endregion

    #region Commands

    #region Pick Up Customers

    [Command]
    public void CmdPickUpCustomer(int groupSize)
    {
        customerGroupSize = groupSize;
    }

    [ClientRpc]
    public void RpcPickUpCustomer()
    {
        TableColliderManager.ToggleTableDetection(true);
    }

    #endregion

    #region Seat Customers

    [Command]
    public void CmdSeatCustomer(GameObject detectedObject, GameObject playerInventory)
    {
        Debug.Log("NetworkedCustomerInteraction - CmdSeatCustomer");
        Debug.Log("NetworkedCustomerInteraction - Detected object: " + detectedObject.tag);

        //check if detected object is table
        if (!detectedObject.GetComponent<TableScript>())
        {
            Debug.Log("NetworkedCustomerInteraction- Player is not looking at a table");
            return;
        }

        //get table's table script
        TableScript tableScript = detectedObject.GetComponent<TableScript>();
        Debug.Log(tableScript);

        var heldCustomer = networkedPlayerInteraction.attachmentPoint.transform.GetChild(0);

        //if table has enough seats
        if (tableScript.CheckSufficientSeats(heldCustomer.GetComponent<CustomerBehaviour_BeingHeld>().groupSizeNum))
        {
            Debug.Log("NetworkedCustomerInteraction - Enough seats for customers");
            RpcSeatCustomer();

            //remove from inventory
            playerInventory = null;

            networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing); //stop holding customer
        }
    }

    [ClientRpc]
    public void RpcSeatCustomer()
    {
        TableColliderManager.ToggleTableDetection(false);
        networkedPlayerInteraction.ChangePlayerState(PlayerState.Default, true);
    }

    #endregion

    #region Take Orders

    [Command]
    //check hands empty
    public void CmdCheckHandsEmpty(GameObject detectedObject, bool inventoryFull)
    {
        //check if player is looking at table
        if (!detectedObject.GetComponent<TableScript>())
        {
            Debug.Log("NetworkedCustomerInteraction- Player is not looking at a table");
            return;
        }

        //get table script
        TableScript tableScript = detectedObject.GetComponent<TableScript>();

        //if player's hands are full, do not take order
        if (inventoryFull)
        {
            Debug.Log("NetworkedCustomerInteraction- Player's hands are full, do not take order");
            tableScript.TableFeedbackScript.HandsFullFeedback();
            return;
        }

        RpcCheckHandsEmpty(detectedObject);
        ////Else, take order of customers at table
        //tableScript.TakeOrder();
        //networkedPlayerInteraction.ChangePlayerState(PlayerState.Default);
    }

    [ClientRpc]
    public void RpcCheckHandsEmpty(GameObject detectedObject)
    {
        //Else, take order of customers at table
        detectedObject.GetComponent<TableScript>().ServerTakeOrder();
        networkedPlayerInteraction.ChangePlayerState(PlayerState.Default);
    }


    #endregion

    #region Spawn Orders

    //for now, spawn orders on keypress
    private void Update()
    {
        //get the input
        var input = Input.inputString;

        //ignore null input to avoid unnecessary computation
        if (!string.IsNullOrEmpty(input))
        {
            switch (input)
            {
                case "1":
                    Debug.Log("Spawn RoastedChicWRiceBall");
                    break;

                case "2":
                    Debug.Log("Spawn RoastedChicWPlainRice");
                    break;

                case "3":
                    Debug.Log("Spawn RoastedChicWRiceBallEgg");
                    break;

                case "4":
                    Debug.Log("Spawn RoastedChicWPlainRiceEgg");
                    break;

                case "5":
                    Debug.Log("Spawn SteamedChicWRiceBall");
                    break;

                case "6":
                    Debug.Log("Spawn SteamedChicWPlainRice");
                    break;

                case "7":
                    Debug.Log("Spawn SteamedChicWRiceBallEgg");
                    break;

                case "8":
                    Debug.Log("Spawn SteamedChicWPlainRiceEgg");
                    break;

            }
        }
            
    }

    [ServerCallback]
    public void ServerSpawnDish()
    {
        //spawn dish
        //pass in an int to call the right method with right parameters
    }


        #endregion

        #endregion

    }
