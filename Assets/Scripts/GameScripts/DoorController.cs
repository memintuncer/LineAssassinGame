using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    Animator DoorAnimator;
    bool IsDoorOpened;
    
   

    public void OpenTheDoor()
    {
        DoorAnimator = GetComponent<Animator>();
        EventManager.TriggerEvent(GameConstants.GameEvents.OPEN_THE_DOOR, new EventParam());
        DoorAnimator.SetTrigger("OpenThDoor");
        IsDoorOpened = true;
    }
}
