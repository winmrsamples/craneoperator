using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class ResetSceneButton : MonoBehaviour, IInputClickHandler, IInputHandler
{
	public GameManager gameManager;
  public GameObject meshedObjectsPlaceable;
  private bool testSet = false;
    
  public void OnInputClicked(InputClickedEventData eventData)
  {
    if (!testSet){
      meshedObjectsPlaceable.GetComponent<TapToPlace>().enabled = false;
			gameManager.CmdPlaceBollards();
    } else {
      meshedObjectsPlaceable.GetComponent<TapToPlace>().enabled = true;
	    gameManager.CmdRemoveBollards();
    }
    
    testSet = !testSet;

    GetComponent<HoloToolkit.Unity.Buttons.CompoundButtonIcon>().OverrideIcon = !testSet;

    if (testSet) {
      GetComponent<HoloToolkit.Unity.Buttons.CompoundButtonText>().Text = "Clear Test";
    } else {
      GetComponent<HoloToolkit.Unity.Buttons.CompoundButtonText>().Text = "Start Test";
    }
  }

  public void SetButtonToClear(){
    testSet = true;
    GetComponent<HoloToolkit.Unity.Buttons.CompoundButtonIcon>().OverrideIcon = false;
    GetComponent<HoloToolkit.Unity.Buttons.CompoundButtonText>().Text = "Clear Test";
  }

  public void OnInputDown(InputEventData eventData)
  { }

  public void OnInputUp(InputEventData eventData)
  { }
}


