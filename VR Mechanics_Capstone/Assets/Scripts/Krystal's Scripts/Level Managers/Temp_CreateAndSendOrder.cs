using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Temp_CreateAndSendOrder : MonoBehaviour
{
    [SerializeField] private Transform UISpawnPos;

    private ChickenRice currentOrder;
    private GameObject orderIcon;

    public void CreateOrder()
    {
        currentOrder = OrderGeneration.Instance.CreateNewOrder();

        orderIcon = Instantiate(currentOrder.OrderIcon, UISpawnPos.position, UISpawnPos.rotation);
    }

    public void SendOrder()
    {
        if(currentOrder != null)
        {
            Temp_VRSpawnOrder.Instance.AddOrderToList(OrderGeneration.Instance.CreateCustomOrder(currentOrder.RoastedChic, currentOrder.RicePlain, currentOrder.HaveEgg));

            Destroy(orderIcon.gameObject);
            currentOrder = null;
        }

    }
}
