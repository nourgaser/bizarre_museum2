using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string arSceneName = "ARScene";
    [SerializeField] private string vrSceneName = "VRScene";

    private Button _arButton;
    private Button _vrButton;
    private Button _quitButton;

    private void Awake()
    {
        var doc = GetComponent<UIDocument>();
        if (doc == null)
        {
            Debug.LogError("MainMenuController requires a UIDocument on the same GameObject.");
            return;
        }

        var root = doc.rootVisualElement;
        _arButton = root.Q<Button>("btn-ar");
        _vrButton = root.Q<Button>("btn-vr");
        _quitButton = root.Q<Button>("btn-quit");

        WireButton(_arButton, () => LoadScene(arSceneName));
        WireButton(_vrButton, () => LoadScene(vrSceneName));
        WireButton(_quitButton, QuitGame);
    }

    private static void WireButton(Button button, System.Action action)
    {
        if (button == null)
        {
            Debug.LogWarning("MainMenuController: Button reference missing.");
            return;
        }

        button.clicked += action;
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("MainMenuController: Scene name is empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
