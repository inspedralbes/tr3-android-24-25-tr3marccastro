using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;

public class ShopScreen : MonoBehaviour
{
    // Referencias a elementos UI
    private VisualElement rootElement;
    private ScrollView skinsContainer;
    private Button buttonBack;
    private Label purchaseMessage;

    // Rutas de API
    private string apiUrlPost = "http://localhost:3002/api/purchases/new-purchase";
    private string apiUrl = "http://localhost:3002/api/skins";
    private string apiUrlIdSkin = "http://localhost:3002/api/purchases/history";

    // Se llama al habilitar el objeto (pantalla de la tienda)
    private void OnEnable()
    {
        // Obtener referencias UI
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        skinsContainer = rootElement.Q<ScrollView>("itemsContainer");
        buttonBack = rootElement.Q<Button>("buttonBack");
        purchaseMessage = rootElement.Q<Label>("purchaseMessage");

        // Botón para volver al menú principal
        buttonBack.clicked += () => SceneManager.LoadScene("MainMenu");

        // Iniciar descarga de skins desde el servidor
        StartCoroutine(GetSkinsFromServer());
    }

    // Obtener lista de skins desde el servidor
    private IEnumerator GetSkinsFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var skinsData = JsonUtility.FromJson<SkinList>(request.downloadHandler.text);

            // Si hay skins disponibles, crear la UI para cada una
            if (skinsData.skins != null && skinsData.skins.Length > 0)
            {
                foreach (var skin in skinsData.skins)
                {
                    if (skin != null) CreateSkinUI(skin);
                }
            }
        }
    }

    // Crear elementos visuales de cada skin
    private void CreateSkinUI(Skin skin)
    {
        VisualElement skinElement = new();
        skinElement.AddToClassList("item");

        // Imagen de la skin
        Image skinImage = new();
        skinImage.style.backgroundImage = new StyleBackground(new Texture2D(1, 1));
        StartCoroutine(LoadImage(skin.imagePath, skinImage));
        skinImage.AddToClassList("image");
        skinElement.Add(skinImage);

        // Nombre de la skin
        var nameLabel = new Label(skin.name);
        nameLabel.AddToClassList("name");
        skinElement.Add(nameLabel);

        // Precio
        var priceLabel = new Label("$" + skin.price);
        priceLabel.AddToClassList("price");
        skinElement.Add(priceLabel);

        // Botón de compra o descarga
        Button buyButton = new() { text = "Verificant..." };
        buyButton.AddToClassList("buy-button");
        skinElement.Add(buyButton);

        skinsContainer.Add(skinElement);

        // Verificamos si el jugador ya posee esta skin
        StartCoroutine(CheckPurchaseHistory(skin.id, isOwned =>
        {
            buyButton.text = isOwned ? "Descarregar" : "Comprar";
            buyButton.clicked += () => HandleSkinAction(skin, isOwned);
        }));
    }

    // Manejar acción del botón (comprar o descargar)
    private void HandleSkinAction(Skin skin, bool isOwned)
    {
        purchaseMessage.style.display = DisplayStyle.None;

        if (isOwned)
        {
            // Si ya la posee, descargarla
            StartCoroutine(DownloadAndSaveAssetBundle(skin, () => {
                purchaseMessage.text = "Descarregat amb èxit.";
                purchaseMessage.style.display = DisplayStyle.Flex;
            }));
        }
        else if (UserSession.GetUserEmail() != null)
        {
            // Si no la tiene, comprarla
            BuySkin(skin, () => {
                purchaseMessage.text = "Comprat i descarregat.";
                purchaseMessage.style.display = DisplayStyle.Flex;
            });
        }
    }

    // Cargar imagen de skin desde URL
    private IEnumerator LoadImage(string imageUrl, Image imageElement)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("http://localhost:3002" + imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            imageElement.style.backgroundImage = new StyleBackground(texture);
        }
    }

    // Comprar una skin y luego descargarla
    private void BuySkin(Skin skin, System.Action onComplete = null)
    {
        StartCoroutine(SaveBillToServer(skin.id, () => {
            StartCoroutine(DownloadAndSaveAssetBundle(skin, onComplete));
        }));
    }

    // Guardar compra en el servidor
    private IEnumerator SaveBillToServer(int skinId, System.Action onSuccess = null)
    {
        string email = UserSession.GetUserEmail();
        BuyData buyData = new() { skinId = skinId, email = email };

        string jsonData = JsonUtility.ToJson(buyData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new(apiUrlPost, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResponsePurchaseData response = JsonUtility.FromJson<ResponsePurchaseData>(request.downloadHandler.text);
            if (response.message == "success")
            {
                onSuccess?.Invoke();
            }
        }
    }

    // Descargar y guardar el AssetBundle localmente
    private IEnumerator DownloadAndSaveAssetBundle(Skin skin, System.Action onComplete = null)
    {
        string localPath = Application.persistentDataPath + "/" + skin.name;

        UnityWebRequest bundleRequest = UnityWebRequest.Get("http://localhost:3002" + skin.assetBundlePath);
        yield return bundleRequest.SendWebRequest();

        if (bundleRequest.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes(localPath, bundleRequest.downloadHandler.data);
            PlayerSkins.AddSkin(skin.id, skin.name, localPath);
            onComplete?.Invoke();
        }
    }

    // Verificar si el jugador ya compró la skin
    private IEnumerator CheckPurchaseHistory(int id, System.Action<bool> callback)
    {
        string email = UserSession.GetUserEmail();
        if (string.IsNullOrEmpty(email))
        {
            callback(false);
            yield break;
        }

        SkinData skinData = new() { skinId = id, email = email };

        string jsonData = JsonUtility.ToJson(skinData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new(apiUrlIdSkin, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResponseHistory response = JsonUtility.FromJson<ResponseHistory>(request.downloadHandler.text);
            callback(response.message == "success");
        }
        else
        {
            callback(false);
        }
    }

    // --- Clases auxiliares para datos JSON ---
    [System.Serializable]
    public class BuyData
    {
        public int skinId;
        public string email;
    }

    [System.Serializable]
    public class ResponsePurchaseData
    {
        public string message;
    }

    [System.Serializable]
    public class SkinData
    {
        public int skinId;
        public string email;
    }

    [System.Serializable]
    public class ResponseHistory
    {
        public string message;
    }

    [System.Serializable]
    public class Skin
    {
        public int id;
        public string name;
        public int price;
        public string imagePath;
        public string assetBundlePath;
        public bool active;
    }

    [System.Serializable]
    public class SkinList
    {
        public Skin[] skins;
    }
}