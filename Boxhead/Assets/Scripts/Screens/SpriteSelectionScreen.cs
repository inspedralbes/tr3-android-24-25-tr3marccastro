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

    // Método llamado cuando el componente se habilita (como al activarse en escena)
    void OnEnable()
    {
        // Obtenemos la raíz del UI Document (UI Toolkit)
        rootElement = GetComponent<UIDocument>().rootVisualElement;

        // Buscamos el contenedor visual donde se mostrarán las skins
        spriteContainer = rootElement.Q<ScrollView>("animationContainer");

        // Botón para cerrar la pantalla de selección
        closeButton = rootElement.Q<Button>("closeButton");

        // Asignamos evento al botón de cerrar
        closeButton.clicked += CloseScreen;

        // Desactivamos el EnemySpawner temporalmente
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(false);
        }

        // Iniciamos la carga de las skins descargadas del jugador
        StartCoroutine(LoadDownloadedSprites());
    }

    // Corrutina que carga las skins que el jugador posee
    private IEnumerator LoadDownloadedSprites()
    {
        var ownedSkins = PlayerSkins.GetOwnedSkins();
        if (ownedSkins.Count == 0)
        {
            yield break;
        }

        // Iteramos cada skin y creamos su botón de selección si el archivo existe
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

    // Crea un botón visual con la imagen de la skin y un botón para seleccionarla
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

        // Buscamos el controller y la imagen asociada a la skin
        foreach (var asset in assetNames)
        {
            if (asset.EndsWith(".controller"))
            {
                controllerPath = asset;
            }

            if (asset.Contains(keyword))
            {
                image = asset;
                break;
            }
        }

        // Creamos el componente visual que representa la skin
        VisualElement spriteElement = new VisualElement();
        spriteElement.AddToClassList("animation-element");

        // Cargamos la imagen asociada a la skin
        Image spriteImage = new Image();
        StartCoroutine(LoadSpriteImage(bundle, image, spriteImage));
        spriteElement.Add(spriteImage);

        // Botón que permite seleccionar esta skin
        Button selectButton = new Button(() => SelectSprite(localPath, name)) { text = "Seleccionar" };
        spriteElement.Add(selectButton);

        // Añadimos el elemento al contenedor visual principal
        spriteContainer.Add(spriteElement);

        // Liberamos el asset bundle (pero no destruye assets cargados)
        bundle.Unload(false);
    }

    // Corrutina para cargar una imagen desde el asset bundle y aplicarla visualmente
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

    // Función que se llama cuando el jugador selecciona una skin
    private void SelectSprite(string localPath, string name)
    {
        // Aplicamos la skin seleccionada al objeto objetivo
        ApplySelectedSprite(localPath, name);

        // Cerramos la pantalla de selección
        gameObject.SetActive(false);

        // Reactivamos el EnemySpawner si estaba presente
        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(true);
        }
    }

    // Aplica el controlador de animaciones de la skin seleccionada al GameObject objetivo
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

                    // Fuerza la actualización del animator
                    animator.Update(0f);
                }
            }
        }

        bundle.Unload(false);
    }

    // Cierra la pantalla y reactiva el EnemySpawner
    private void CloseScreen()
    {
        gameObject.SetActive(false);

        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(true);
        }
    }
}
