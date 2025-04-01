using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;

public class ShopScreen : MonoBehaviour
{
    private VisualElement rootElement;
    private ScrollView skinsContainer;
    private Button buttonBack;
    private string apiUrlPost = "http://localhost:3002/api/purchases/new-purchase";
    private string apiUrl = "http://localhost:3002/api/skins";
    private string apiUrlIdSkin = "http://localhost:3002/api/purchases/history";

    private void Awake()
     {
        // PlayerSkins.DeleteAll();
 
        // UserSession.DeleteEmail();
     }

    void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        skinsContainer = rootElement.Q<ScrollView>("itemsContainer");
        buttonBack = rootElement.Q<Button>("buttonBack");

        buttonBack.clicked += () => SceneManager.LoadScene("MainMenu");

        // Cargar los skins desde el servidor
        StartCoroutine(GetSkinsFromServer());
    }

    private IEnumerator GetSkinsFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Deserializar los datos JSON (ahora es un objeto con una propiedad "skins")
            var skinsData = JsonUtility.FromJson<SkinList>(request.downloadHandler.text);

            if (skinsData?.skins != null && skinsData.skins.Length > 0)
            {
                Debug.Log($"🔄 Se encontraron {skinsData.skins.Length} skins.");
                foreach (var skin in skinsData.skins)
                {
                    if (skin == null)
                    {
                        Debug.LogError("❌ Una skin es null en el array.");
                        continue;
                    }
                    CreateSkinUI(skin);
                }
            }
            else
            {
                Debug.LogError("❌ skinsData.skins es null o está vacío.");
            }
        }
        else
        {
            Debug.LogError("Error al obtener los ítems: " + request.error);
        }
    }

    private void CreateSkinUI(Skin skin)
    {
        VisualElement skinElement = new VisualElement();
        skinElement.AddToClassList("item");

        Image skinImage = new();
        skinImage.style.backgroundImage = new StyleBackground(new Texture2D(1, 1));
        StartCoroutine(LoadImage(skin.imagePath, skinImage));

        skinImage.AddToClassList("image");
        skinElement.Add(skinImage);

        var nameLabel = new Label(skin.name);
        nameLabel.AddToClassList("name");
        skinElement.Add(nameLabel);

        var priceLabel = new Label("$" + skin.price);
        priceLabel.AddToClassList("price");
        skinElement.Add(priceLabel);

        Button buyButton = new Button { text = "Verificando..." };
        buyButton.AddToClassList("buy-button");
        skinElement.Add(buyButton);

        skinsContainer.Add(skinElement);

        StartCoroutine(CheckPurchaseHistory(skin.id, isOwned =>
        {
            buyButton.text = isOwned ? "Descargar" : "Comprar";
            buyButton.clicked += () => HandleSkinAction(skin, isOwned);
        }));
    }

    private IEnumerator LoadImage(string imageUrl, Image imageElement)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("http://localhost:3002" + imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            imageElement.style.backgroundImage = new StyleBackground(texture);
        }
        else
        {
            Debug.LogError("Error al cargar la imagen: " + request.error);
        }
    }

    private void HandleSkinAction(Skin skin, bool isOwned)
    {
        if (isOwned)
        {
            StartCoroutine(DownloadAndSaveAssetBundle(skin));
        }
        else
        {
            BuySkin(skin);
        }
    }

    private IEnumerator CheckPurchaseHistory(int id, System.Action<bool> callback)
    {

        if (PlayerSkins.HasSkin(id))
        {
            Debug.Log("Skin " + id + " encontrada en PlayerPrefs.");
            callback(true);
            yield break;
        }

        Debug.Log("Skin " + id + " no encontrada en PlayerPrefs, consultando base de datos...");

        string email = UserSession.GetUserEmail();
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
            string result = request.downloadHandler.text;
            Debug.Log("Respuesta del servidor: " + result);

            try
            {
                ResponseHistory response = JsonUtility.FromJson<ResponseHistory>(result);
                if (response.message == "success")
                {
                    callback(true);
                    yield break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error al procesar la respuesta JSON: " + e.Message);
                callback(false);
            }
        }
        else
        {
            //Debug.LogError("Error de conexión: " + request.error);
            //Debug.LogError("Código de respuesta HTTP: " + request.responseCode);
            //Debug.LogError("Respuesta completa del servidor: " + request.downloadHandler.text);
            callback(false);
        }
    }

    private void BuySkin(Skin skin)
    {
        StartCoroutine(SaveBillToServer(skin.id));
        StartCoroutine(DownloadAndSaveAssetBundle(skin));
    }

    private IEnumerator DownloadAndSaveAssetBundle(Skin skin)
    {
        string imageName = Path.GetFileNameWithoutExtension(skin.imagePath);
        string fullBundleUrl = "http://localhost:3002" + skin.assetBundlePath;
        string localPath = Application.persistentDataPath + "/" + skin.name;

        UnityWebRequest bundleRequest = UnityWebRequest.Get(fullBundleUrl);
        yield return bundleRequest.SendWebRequest();

        if (bundleRequest.result == UnityWebRequest.Result.Success)
        {
            // Obtener los datos binarios del AssetBundle
            byte[] assetData = bundleRequest.downloadHandler.data;

            // Comprobar si los datos son válidos
            if (assetData != null && assetData.Length > 0)
            {
                // Guardar el AssetBundle en local como un archivo binario
                File.WriteAllBytes(localPath, assetData);
                Debug.Log("AssetBundle guardado en: " + localPath);
                PlayerSkins.AddSkin(skin.id, imageName, localPath);
            }
            else
            {
                Debug.LogError("No se recibieron datos válidos del AssetBundle.");
            }
        }
        else
        {
            Debug.LogError("Error al descargar el archivo: " + bundleRequest.error);
        }
    }

    private IEnumerator SaveBillToServer(int skinId)
    {
        string email = UserSession.GetUserEmail();

        BuyData buyData = new()
        {
            skinId = skinId,
            email = email
        };

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
            string result = request.downloadHandler.text;
            Debug.Log("Respuesta del servidor: " + result);

            try
            {
                ResponsePurchaseData response = JsonUtility.FromJson<ResponsePurchaseData>(result);

                if (response.message == "success")
                {
                    Debug.Log("Compra existosa");
                }
                else
                {
                    Debug.LogError(response.message);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error en la respuesta del servidor: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Error de conexión: " + request.error);
        }
    }

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

    // Clases para deserializar los datos del servidor
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