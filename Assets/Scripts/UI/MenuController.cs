using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

  [SerializeField] private Button _startButton;

  void Start() {
    _startButton.onClick.AddListener(LoadGameScene);
  }

  private void LoadGameScene() {
    SceneManager.LoadScene(GameConstants.GAME_SCENE);
  }
}
