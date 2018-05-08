using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class SetWindButton : MonoBehaviour, IInputClickHandler, IInputHandler
{
    // public GameManager gameManager;

    
  public void OnInputClicked(InputClickedEventData eventData)
  {
    // gameManager.CmdSetWindLevel(0.5f);
  }
  public void OnInputDown(InputEventData eventData)
  { }
  public void OnInputUp(InputEventData eventData)
  { }
}


