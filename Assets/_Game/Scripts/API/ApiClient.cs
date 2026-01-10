using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    [SerializeField] private string backendBaseUrl = "http://localhost:3000";

    public static ApiClient Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private string BuildUrl(string path)
    {
        return $"{backendBaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
    }

    private static bool IsError(UnityWebRequest request)
    {
        return request.result == UnityWebRequest.Result.ConnectionError ||
               request.result == UnityWebRequest.Result.ProtocolError;
    }

    private static async Task<UnityWebRequest> Send(UnityWebRequest request)
    {
        var operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }
        return request;
    }

    public async Task<bool> PingAsync()
    {
        var request = UnityWebRequest.Get(BuildUrl("api/health"));
        request.downloadHandler = new DownloadHandlerBuffer();

        var response = await Send(request);
        if (IsError(response))
        {
            Debug.LogWarning($"Ping failed: {response.error}");
            return false;
        }

        var payload = response.downloadHandler.text;
        var data = JsonUtility.FromJson<HealthResponse>(payload);
        return data != null && data.ok;
    }

    public async Task<CreateConcoctionResponse> CreateConcoctionAsync(IEnumerable<string> items)
    {
        var payload = new CreateConcoctionRequest(items?.ToArray() ?? Array.Empty<string>());
        var json = JsonUtility.ToJson(payload);

        var request = new UnityWebRequest(BuildUrl("api/concoctions"), UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var response = await Send(request);
        if (IsError(response))
        {
            Debug.LogError($"CreateConcoction failed: {response.error} | body: {response.downloadHandler.text}");
            return null;
        }

        return JsonUtility.FromJson<CreateConcoctionResponse>(response.downloadHandler.text);
    }

    public async Task<ConcoctionDto> GetConcoctionAsync(string code)
    {
        var normalized = (code ?? string.Empty).Trim().ToUpperInvariant();
        if (normalized.Length == 0)
        {
            Debug.LogWarning("GetConcoction called with empty code");
            return null;
        }

        var request = UnityWebRequest.Get(BuildUrl($"api/concoctions/{normalized}"));
        request.downloadHandler = new DownloadHandlerBuffer();

        var response = await Send(request);
        if (IsError(response))
        {
            Debug.LogError($"GetConcoction failed: {response.error} | body: {response.downloadHandler.text}");
            return null;
        }

        return JsonUtility.FromJson<ConcoctionDto>(response.downloadHandler.text);
    }

    public async Task<ConcoctionDto[]> ListConcoctionsAsync()
    {
        var request = UnityWebRequest.Get(BuildUrl("api/concoctions"));
        request.downloadHandler = new DownloadHandlerBuffer();

        var response = await Send(request);
        if (IsError(response))
        {
            Debug.LogWarning($"ListConcoctions failed: {response.error} | body: {response.downloadHandler.text}");
            return Array.Empty<ConcoctionDto>();
        }

        return JsonArrayHelper.FromJsonArray<ConcoctionDto>(response.downloadHandler.text);
    }
}
