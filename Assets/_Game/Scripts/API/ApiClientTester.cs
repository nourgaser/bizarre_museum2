using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ApiClientTester : MonoBehaviour
{
    [Header("Test Flow")]
    [SerializeField] private bool createOnStart = true;
    [SerializeField] private bool fetchOnStart = true;

    [Header("Payloads")]
    [SerializeField] private string[] testItems =
    {
        "gravity-anomaly",
        "humming-relic",
        "chromatic-shifter"
    };

    [SerializeField] private string codeToFetch = "";

    private async void Start()
    {
        await RunTestAsync();
    }

    private static bool TryGetClient(out ApiClient client)
    {
        client = ApiClient.Instance;
        if (client == null)
        {
            Debug.LogError("ApiClientTester: ApiClient.Instance is null. Place ApiClient in the scene.");
            return false;
        }

        return true;
    }

    private async Task RunTestAsync()
    {
        if (!TryGetClient(out var client))
        {
            return;
        }

        Debug.Log("[Tester] Pinging backend...");
        var ok = await client.PingAsync();
        Debug.Log(ok ? "[Tester] Health OK" : "[Tester] Health FAILED");

        string createdCode = codeToFetch;

        if (createOnStart)
        {
            Debug.Log($"[Tester] Creating concoction with items: {string.Join(", ", testItems)}");
            var createResp = await client.CreateConcoctionAsync(testItems);
            if (createResp != null && !string.IsNullOrEmpty(createResp.code))
            {
                createdCode = createResp.code;
                codeToFetch = createResp.code; // expose in Inspector for reuse
                Debug.Log($"[Tester] Created code: {createResp.code}");
            }
            else
            {
                Debug.LogError("[Tester] CreateConcoction failed.");
            }
        }

        var targetCode = createdCode;
        if (fetchOnStart && !string.IsNullOrEmpty(targetCode))
        {
            Debug.Log($"[Tester] Fetching concoction: {targetCode}");
            var concoction = await client.GetConcoctionAsync(targetCode);
            if (concoction != null)
            {
                Debug.Log($"[Tester] Got concoction {concoction.code} at {concoction.createdAt}\n{FormatItems(concoction)}");
            }
            else
            {
                Debug.LogError("[Tester] GetConcoction returned null.");
            }
        }
    }

    private static string FormatItems(ConcoctionDto concoction)
    {
        if (concoction?.items == null || concoction.items.Length == 0)
        {
            return "Items: (none)";
        }

        var sb = new StringBuilder();
        sb.AppendLine("Items:");
        foreach (var item in concoction.items)
        {
            sb.AppendLine($" - {item.slug} | seed={item.seed:F4}");
        }
        return sb.ToString();
    }
}
