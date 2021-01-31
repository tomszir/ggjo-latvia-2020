using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
  public void MainMenuStart() {
    Application.LoadLevel(0);
  }

  public void MainMenuCredits() {
    Application.LoadLevel(2);
  }

  public void MainMenuExit() {
    Application.Quit();
  }

  public void CreditsBack() {
    Application.LoadLevel(1);
  }
}
