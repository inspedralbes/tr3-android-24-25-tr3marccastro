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
        spriteContainer = rootElement.Q<ScrollView>("spriteContainer");
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
        // Crear el contenedor para el botón
        VisualElement spriteElement = new VisualElement();
        spriteElement.AddToClassList("sprite-element");

        // Crear y añadir la imagen del sprite
        Image spriteImage = new Image();
        StartCoroutine(LoadSpriteImageFromBundle(localPath, name, spriteImage));
        spriteElement.Add(spriteImage);

        // Botón para seleccionar el sprite
        Button selectButton = new Button(() => SelectSprite(localPath, name)) { text = "Seleccionar" };
        spriteElement.Add(selectButton);

        // Añadir el sprite a la lista
        spriteContainer.Add(spriteElement);
    }

    private IEnumerator LoadSpriteImageFromBundle(string localPath, string name, Image spriteImage)
    {
        Debug.Log(localPath);
        // Cargar el AssetBundle desde el archivo local
        AssetBundle bundle = AssetBundle.LoadFromFile(localPath);

        if (bundle == null)
        {
            Debug.LogError("No se pudo cargar el AssetBundle desde la ruta: " + localPath);
            yield break;
        }

        // Imprimir todos los activos en el AssetBundle para asegurarse de que el nombre es correcto
        string[] assetNames = bundle.GetAllAssetNames();
        foreach (var asset in assetNames)
        {
            Debug.Log("Asset: " + asset);
        }

        // Intentar obtener la textura del AssetBundle
        Texture2D texture = bundle.LoadAsset<Texture2D>(name);  // Usar el nombre correcto de tu activo

        if (texture != null)
        {
            Debug.Log("Textura cargada con éxito, tamaño: " + texture.width + "x" + texture.height);

            // Establecer el tamaño de la imagen en la UI
            spriteImage.style.width = texture.width;
            spriteImage.style.height = texture.height;

            // Asignar la textura al fondo de la imagen
            spriteImage.style.backgroundImage = new StyleBackground(texture);
        }
        else
        {
            Debug.LogError("No se encontró la textura dentro del AssetBundle.");
        }

        // Liberar el AssetBundle para evitar fugas de memoria
        bundle.Unload(false);
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
        AssetBundle bundle = AssetBundle.LoadFromFile(localPath);

        if (bundle == null)
        {
            Debug.LogError("No se pudo cargar el AssetBundle desde la ruta: " + localPath);
            return;
        }

        // Intentar obtener la textura del AssetBundle (cambiar "fanta" al nombre correcto de la textura)
        Texture2D texture = bundle.LoadAsset<Texture2D>(name);

        if (texture != null)
        {
            // Crear un sprite a partir de la textura
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Obtener el SpriteRenderer del objeto y asignar el sprite
            SpriteRenderer renderer = targetObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = sprite;
                Debug.Log("Imagen aplicada al objeto correctamente.");
            }
            else
            {
                Debug.LogError("El objeto no tiene un SpriteRenderer.");
            }
        }
        else
        {
            Debug.LogError("No se encontró la textura dentro del AssetBundle.");
        }

        // Liberar el AssetBundle para evitar fugas de memoria
        bundle.Unload(false);
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
