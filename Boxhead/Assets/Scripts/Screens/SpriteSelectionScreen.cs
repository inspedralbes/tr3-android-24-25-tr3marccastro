using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SpriteSelectionScreen : MonoBehaviour
{
    private VisualElement rootElement;
    private ScrollView spriteContainer;
    private Button closeButton;
    public GameObject targetObject;
    private EnemySpawner enemySpawner;


    void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        spriteContainer = rootElement.Q<ScrollView>("animationContainer");
        closeButton = rootElement.Q<Button>("closeButton");

        closeButton.clicked += CloseScreen;

        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(false);
        }

        StartCoroutine(LoadDownloadedSprites());
    }

    private IEnumerator LoadDownloadedSprites()
    {
        var ownedSkins = PlayerSkins.GetOwnedSkins();
        if (ownedSkins.Count == 0)
        {
            yield break;
        }

        foreach (var skin in ownedSkins)
        {
            string localPath = skin.assetBundlePath;

            if (System.IO.File.Exists(localPath))
            {
                CreateSpriteButton(localPath, skin.name);
            }

            yield return null;
        }
    }

    private void CreateSpriteButton(string localPath, string name)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(localPath);

        if (bundle == null)
        {
            return;
        }

        string[] assetNames = bundle.GetAllAssetNames();
        string controllerPath = null;
        string keyword = "base";
        string image = null;

        foreach (var asset in assetNames)
        {
            if (asset.EndsWith(".controller"))
            {
                controllerPath = asset;
            }

            if(asset.Contains(keyword))
            {
                image = asset;
                break;
            }
        }

        VisualElement spriteElement = new VisualElement();
        spriteElement.AddToClassList("animation-element");

        Image spriteImage = new Image();
        StartCoroutine(LoadSpriteImage(bundle, image, spriteImage));
        spriteElement.Add(spriteImage);

        Button selectButton = new Button(() => SelectSprite(localPath, name)) { text = "Seleccionar" };
        spriteElement.Add(selectButton);

        spriteContainer.Add(spriteElement);

        bundle.Unload(false);
    }

    private IEnumerator LoadSpriteImage(AssetBundle bundle, string imagePath, Image spriteImage)
    {
        Texture2D texture = bundle.LoadAsset<Texture2D>(imagePath);

        if (texture != null)
        {
            float scaleFactor = 2.0f;
            int newWidth = (int)(texture.width * scaleFactor);
            int newHeight = (int)(texture.height * scaleFactor);

            spriteImage.style.width = newWidth;
            spriteImage.style.height = newHeight;

            spriteImage.style.backgroundImage = new StyleBackground(texture);
        }

        yield return null;
    }


    private void SelectSprite(string localPath, string name)
    {
        ApplySelectedSprite(localPath, name);

        gameObject.SetActive(false);

        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(true);
        }
    }

    private void ApplySelectedSprite(string localPath, string name)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(localPath);

        if (bundle == null)
        {
            return;
        }

        string[] assetNames = bundle.GetAllAssetNames();
        string controllerPath = null;

        foreach (var asset in assetNames)
        {
            if (asset.EndsWith(".controller") && controllerPath == null)
            {
                controllerPath = asset;
            }
        }

        if (!string.IsNullOrEmpty(controllerPath))
        {
            RuntimeAnimatorController animatorController = bundle.LoadAsset<RuntimeAnimatorController>(controllerPath);

            if (animatorController != null)
            {
                Animator animator = targetObject.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = animatorController;

                    animator.Update(0f);
                }
            }
        }

        bundle.Unload(false);
    }

    private void CloseScreen()
    {
        gameObject.SetActive(false);

        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(true);
        }
    }
}
