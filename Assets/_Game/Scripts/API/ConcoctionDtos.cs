using System;
using UnityEngine;

[Serializable]
public class ConcoctionItemDto
{
    public string slug;
    public float seed;
}

[Serializable]
public class ConcoctionDto
{
    public string code;
    public string createdAt;
    public ConcoctionItemDto[] items;
}

[Serializable]
public class CreateConcoctionRequest
{
    public string[] items;

    public CreateConcoctionRequest(string[] items)
    {
        this.items = items;
    }
}

[Serializable]
public class CreateConcoctionResponse
{
    public bool success;
    public string code;
}

[Serializable]
public class HealthResponse
{
    public bool ok;
    public string service;
}

public static class JsonArrayHelper
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }

    public static T[] FromJsonArray<T>(string json)
    {
        var wrapped = "{\"items\":" + json + "}";
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return wrapper?.items ?? Array.Empty<T>();
    }
}
