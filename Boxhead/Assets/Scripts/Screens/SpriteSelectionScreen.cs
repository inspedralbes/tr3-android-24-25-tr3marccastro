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
        // Obtener la raíz del documento visual
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        spriteContainer = rootElement.Q<ScrollView>("animationContainer");
        closeButton = rootElement.Q<Button>("closeButton");  // Botón de cerrar

        // Configurar el evento del botón de cerrar
        closeButton.clicked += CloseScreen;

        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(false);
        }

        // Recargar los sprites descargados desde PlayerPrefs
        StartCoroutine(LoadDownloadedSprites());
    }

    private IEnumerator LoadDownloadedSprites()
    {
        // Obtener la lista de los items comprados (y los sprites correspondientes) desde PlayerPrefs
        var ownedItems = PlayerSkins.GetOwnedItems();
        if (ownedItems.Count == 0)
        {
            Debug.LogWarning("No se han descargado sprites.");
            yield break;
        }

        // Cargar cada sprite desde el directorio local
        foreach (var item in ownedItems)
        {
            // string itemName = item.name;
            string localPath = item.assetBundlePath;

            if (System.IO.File.Exists(localPath))
            {
                // Crear el botón para seleccionar el sprite
                CreateSpriteButton(localPath, item.name);
            }
            else
            {
                Debug.LogWarning("Sprite no encontrado: " + localPath);
            }

            yield return null;
        }
    }

    private void CreateSpriteButton(string localPath, string name)
    {
        // Cargar el AssetBundle desde el archivo local
        AssetBundle bundle = AssetBundle.LoadFromFile(localPath);

        if (bundle == null)
        {
            Debug.LogError("No se pudo cargar el AssetBundle desde la ruta: " + localPath);
            return;
        }

        // Obtener el nombre correcto del Animator Controller dentro del AssetBundle
        string[] assetNames = bundle.GetAllAssetNames();
        string controllerPath = null;
        string keyword = "base";
        string image = null;

        foreach (var asset in assetNames)
        {
            Debug.Log("Asset encontrado en AssetBundle: " + asset);
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

        Debug.Log("Los tengo: " + controllerPath + " " + image);

        // Crear el contenedor para el botón
        VisualElement spriteElement = new VisualElement();
        spriteElement.AddToClassList("animation-element");

        // Crear y añadir la imagen del sprite
        Image spriteImage = new Image();
        StartCoroutine(LoadSpriteImage(bundle, image, spriteImage));
        spriteElement.Add(spriteImage);

        // Botón para seleccionar el sprite
        Button selectButton = new Button(() => SelectSprite(localPath, name)) { text = "Seleccionar" };
        spriteElement.Add(selectButton);

        // Añadir el sprite a la lista
        spriteContainer.Add(spriteElement);

        // Liberar el AssetBundle para evitar fugas de memoria
        bundle.Unload(false);
    }

    private IEnumerator LoadSpriteImage(AssetBundle bundle, string imagePath, Image spriteImage)
    {
        // Intentar obtener la textura del AssetBundle
        Texture2D texture = bundle.LoadAsset<Texture2D>(imagePath);  // Usar la ruta correcta de la imagen

        if (texture != null)
        {
            Debug.Log("🖼️ Textura cargada con éxito, tamaño: " + texture.width + "x" + texture.height);
            
            float scaleFactor = 2.0f;  // Cambia este valor para hacer la imagen más grande o más pequeña
            int newWidth = (int)(texture.width * scaleFactor);
            int newHeight = (int)(texture.height * scaleFactor);

            // 🔹 Redimensionar la imagen en la UI
            spriteImage.style.width = newWidth;
            spriteImage.style.height = newHeight;

            spriteImage.style.backgroundImage = new StyleBackground(texture);
        }
        else
        {
            Debug.LogError("❌ No se encontró la textura dentro del AssetBundle.");
        }

        yield return null;
    }


    private void SelectSprite(string localPath, string name)
    {
        Debug.Log("Sprite seleccionado: " + localPath);
        // Aquí puedes aplicar el sprite al objeto del juego.
        ApplySelectedSprite(localPath, name);

        gameObject.SetActive(false);

        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(true);
        }
    }

    private void ApplySelectedSprite(string localPath, string name)
    {
        // Cargar el AssetBundle desde el archivo local
        //AssetBundle bundle = AssetBundle.LoadFromFile(localPath);

        //if (bundle == null)
        //{
        //    Debug.LogError("No se pudo cargar el AssetBundle desde la ruta: " + localPath);
        //    return;
        //}

        //// Obtener el nombre correcto del Animator Controller dentro del AssetBundle
        //string[] assetNames = bundle.GetAllAssetNames();
        //string controllerPath = null;

        //foreach (var asset in assetNames)
        //{
        //    Debug.Log("Asset encontrado en AssetBundle: " + asset);
        //    if (asset.EndsWith(".controller"))
        //    {
        //        controllerPath = asset;
        //        break; // Tomamos el primero que encontramos
        //    }
        //}

        // Si encontramos el Animator Controller, lo cargamos
        //if (!string.IsNullOrEmpty(controllerPath))
        //{
        //    RuntimeAnimatorController animatorController = bundle.LoadAsset<RuntimeAnimatorController>(controllerPath);

        //    if (animatorController != null)
        //    {
        //        // Obtener el componente Animator del objeto de destino
        //        Animator animator = targetObject.GetComponent<Animator>();
        //        if (animator != null)
        //        {
        //            // Asignar el Animator Controller al componente Animator
        //            animator.runtimeAnimatorController = animatorController;
        //            Debug.Log("✅ Animator Controller asignado correctamente: " + controllerPath);
        //        }
        //        else
        //        {
        //            Debug.LogError("⚠️ El objeto no tiene un componente Animator.");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("⚠️ No se pudo cargar el Animator Controller.");
        //    }
        //}
        //else
        //{
        //    Debug.LogError("⚠️ No se encontró ningún archivo .controller en el AssetBundle.");
        //}

        // Liberar el AssetBundle para evitar fugas de memoria
        // bundle.Unload(false);
    }

    private void CloseScreen()
    {
        // Aquí puedes ocultar o desactivar la pantalla de selección
        gameObject.SetActive(false); // Desactivar el objeto de la pantalla de selección de sprites

        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(true);
        }
    }
}
