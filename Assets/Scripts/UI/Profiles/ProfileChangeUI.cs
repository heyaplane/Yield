using UnityEngine;
using UnityEngine.UI;

public class ProfileChangeUI : BaseUI
{
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button createNewButton;

    [SerializeField] BaseScrollView profileScrollView;
    
    string profileNameInput;
    void SetSelectedProfileName(string profileName) => profileNameInput = profileName;
    
    void OnEnable()
    {
        confirmButton.onClick.AddListener(HandleConfirmButton);
        cancelButton.onClick.AddListener(HandleCancelButton);
        createNewButton.onClick.AddListener(HandleCreateNeButton);

        profileScrollView.OnScrollViewItemClickedEvent += SetSelectedProfileName;
    }

    void OnDisable()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        createNewButton.onClick.RemoveAllListeners();
        
        profileScrollView.OnScrollViewItemClickedEvent -= SetSelectedProfileName;
    }

    void HandleConfirmButton() => SaveManager.Instance.ChangeProfile(profileNameInput);
    void HandleCreateNeButton() => GameManager.Instance.RequestSceneTransition(SceneController.Instance.ProfileCreate);
    void HandleCancelButton()
    {
        CloseWindow();
        OnCancelAction?.Invoke();
    }
}
