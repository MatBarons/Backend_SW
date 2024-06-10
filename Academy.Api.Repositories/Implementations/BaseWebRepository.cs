using System.Reflection;
using System.Text;
using System.Text.Json;
using Academy.Api.Domain;
using Academy.Api.Domain.Exceptions;
using Academy.Api.Domain.Extensions;
using Academy.Api.Domain.Models.WebApi;
using Microsoft.Extensions.Logging;

namespace Academy.Api.Repositories.Implementations;

/// <summary>
/// Base class for all repositories that connect to a WebService, it provides basic handling of HTTP calls
///
/// Any repository that interfaces with a WebService may use this class as parent class
/// </summary>
public abstract class BaseWebRepository
{
    protected readonly HttpClient client;
    private readonly List<Uri> hosts;
    
    private static ILogger<BaseWebRepository> logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="h">List of hosts to connect to. If more than one is provided requests are sent to any of the hosts with a round robin policy</param>
    internal BaseWebRepository(IHttpClientFactory httpClientFactory, IEnumerable<Uri> h, ILogger<BaseWebRepository> l)
    {
        logger = l;
        client = httpClientFactory.CreateClient(Constants.HttpClientName);
        this.hosts = h.ToList();
        this.hosts.Shuffle();//Randomize the hosts usage order
    }

    internal BaseWebRepository(IHttpClientFactory httpClientFactory, Uri host, ILogger<BaseWebRepository> l) : this(httpClientFactory, new List<Uri>() { host }, l)
    {
    }

    /// <summary>
    /// Sends a GET request to the specified endpoint and with the specified query
    /// Tries to unserialize the JSON response into the T type
    /// </summary>
    /// <param name="endpoint">Relative URL of the endpoint to call</param>
    /// <param name="query"></param>
    /// <param name="header"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal async Task<WebServiceCallOutput<T>> DoGet<T>(Uri endpoint, Dictionary<string, string> query, Dictionary<string, string>? header = null)
    {
        string queryArguments = MakeGetQuery(query);

        var orderedHosts = hosts.ToList();
        orderedHosts.Shuffle();

        foreach (var host in orderedHosts)
        {
            try
            {
                UriBuilder uriBuilder = new UriBuilder(host);
                uriBuilder.Path = endpoint.ToString();
                uriBuilder.Query = queryArguments;

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri))
                {
                    if (header != null && header.Any())
                    {
                        foreach (var pair in header)
                        {
                            requestMessage.Headers.Add(pair.Key, pair.Value);
                        }
                    }

                    using (HttpResponseMessage response = await client.SendAsync(requestMessage))
                    {
                        return new WebServiceCallOutput<T>()
                        {
                            endpoint = endpoint,
                            arguments = query,
                            Result = await ProcessResponse<T>(response, endpoint, query)
                        };
                    }
                }
            }

            catch (HttpRequestException e)
            {
                int idx = orderedHosts.IndexOf(host);
                if (idx + 1 == orderedHosts.Count)
                {//Finished all hosts to try
                    logger.LogError(e,
                        "Request to {h} on {e} failed. There are no other alternative hosts to try, the request failed permanently", host,
                        endpoint);
                    throw;
                }
                else
                {//This host failed, let's try another one
                    logger.LogWarning(e, "Request to {h} on {e} failed. The request will be retried to a secondary host", host, endpoint);
                }
            }
        }

        return null;//Should never reach this
    }

    /// <summary>
    /// Sends a GET request to the specified endpoint and with the specified query
    /// Tries to unserialize the JSON response into the T type
    /// </summary>
    /// <param name="endpoint">Relative URL of the endpoint to call</param>
    /// <param name="query"></param>
    /// <returns>The output with the response's bytes length</returns>
    internal async Task<WebServiceCallOutput<Stream>> DoGetStream(Uri endpoint, Dictionary<string, string>? query = null)
    {
        string queryArguments = query == null ? "" : MakeGetQuery(query);

        var orderedHosts = hosts.ToList();
        orderedHosts.Shuffle();

        foreach (var host in orderedHosts)
        {
            try
            {
                UriBuilder uriBuilder = new UriBuilder(host);
                uriBuilder.Path = endpoint.ToString();
                uriBuilder.Query = queryArguments;

                return new WebServiceCallOutput<Stream>()
                {
                    endpoint = endpoint,
                    arguments = query,
                    Result = await client.GetStreamAsync(uriBuilder.Uri)
                };
            }
            catch (HttpRequestException e)
            {
                int idx = orderedHosts.IndexOf(host);
                if (idx + 1 == orderedHosts.Count)
                {//Finished all hosts to try
                    logger.LogError(e,
                        "Request to {h} on {e} failed. There are no other alternative hosts to try, the request failed permanently", host,
                        endpoint);
                    throw;
                }
                else
                {//This host failed, let's try another one
                    logger.LogWarning(e, "Request to {h} on {e} failed. The request will be retried to a secondary host", host, endpoint);
                }
            }
        }

        return null;//Should never reach this
    }

    /// <summary>
    /// Sends a POST request to the specified endpoint, form data in <paramref name="formData"/> 
    /// and writes the response to <paramref name="outputStream"/>
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="outputStream"></param>
    /// <param name="header"></param>
    /// <param name="formData"></param>
    internal async Task DoPostFormDataStreamable(
        Uri endpoint,
        MemoryStream outputStream,
        Dictionary<string, string>? header = null,
        MultipartFormDataContent? formData = null
        )
    {
        var orderedHosts = hosts.ToList();
        orderedHosts.Shuffle();

        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var http = new HttpClient(handler);

        foreach (var host in orderedHosts)
        {
            try
            {
                Uri fullUri = new Uri(host, endpoint);

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, fullUri))
                {
                    if (header != null && header.Any())
                    {
                        foreach (var pair in header)
                        {
                            requestMessage.Headers.Add(pair.Key, pair.Value);
                        }
                    }

                    requestMessage.Content = formData;

                    using (HttpResponseMessage response = await http.SendAsync(requestMessage))
                    using (var resStream = await response.Content.ReadAsStreamAsync())
                    {
                        await resStream.CopyToAsync(outputStream);
                        return;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                int idx = orderedHosts.IndexOf(host);
                if (idx + 1 == orderedHosts.Count)
                {
                    //Finished all hosts to try
                    logger.LogError(e,
                        "Request to {h} on {e} failed. There are no other alternative hosts to try, the request failed permanently",
                        host, endpoint);
                    throw;
                }
                else
                {
                    //This host failed, let's try another one
                    logger.LogWarning(e, "Request to {h} on {e} failed. The request will be retried to a secondary host",
                        host, endpoint);
                }
            }
        }

        // Should never reach this
        throw new Exception("Should never reach this");
    }

    /// <summary>
    /// Sends a POST request to the specified endpoint and with the JSON-serialized arguments specified
    /// Tries to unserialize the JSON response into the T type
    /// </summary>
    /// <param name="endpoint">Relative URL of the endpoint to call</param>
    /// <param name="arguments"></param>
    /// <param name="header"></param>
    /// <param name="formatArgumentJson"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal async Task<WebServiceCallOutput<T>> DoPost<T>(Uri endpoint, object arguments, Dictionary<string, string>? header = null, bool formatArgumentJson = true)
    {
        var orderedHosts = hosts.ToList();
        orderedHosts.Shuffle();

        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var http = new HttpClient(handler);

        foreach (var host in orderedHosts)
        {
            try
            {
                Uri fullUri = new Uri(host, endpoint);

                HttpContent? requestContent = null;
                if (arguments != null)
                {
                    string? serialized = null;
                    if (formatArgumentJson)
                    {
                        serialized = JsonSerializer.Serialize(arguments);
                        logger.LogTrace("Argomenti serializzati della richiesta POST a {url}: {s}", fullUri, serialized);
                        requestContent = new StringContent(serialized, Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        requestContent = MakeFormUrlEncodedArguments(arguments);
                        logger.LogTrace("Argomenti serializzati della richiesta POST a {url}: {s}", fullUri, requestContent);
                    }
                }

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, fullUri))
                {
                    if (header != null && header.Any())
                    {
                        foreach (var pair in header)
                        {
                            requestMessage.Headers.Add(pair.Key, pair.Value);
                        }
                    }

                    requestMessage.Content = requestContent;

                    using (HttpResponseMessage response = await http.SendAsync(requestMessage))
                    {
                        return new WebServiceCallOutput<T>()
                        {
                            endpoint = endpoint,
                            arguments = arguments,
                            Result = await ProcessResponse<T>(response, endpoint, arguments)
                        };
                    }
                }
            }
            catch (HttpRequestException e)
            {
                int idx = orderedHosts.IndexOf(host);
                if (idx + 1 == orderedHosts.Count)
                {
                    //Finished all hosts to try
                    logger.LogError(e,
                        "Request to {h} on {e} failed. There are no other alternative hosts to try, the request failed permanently",
                        host, endpoint);
                    throw;
                }
                else
                {
                    //This host failed, let's try another one
                    logger.LogWarning(e, "Request to {h} on {e} failed. The request will be retried to a secondary host",
                        host, endpoint);
                }
            }
        }

        return null;//Should never reach this
    }

    /// <summary>
    /// Processes Http response into a provided object type
    /// </summary>
    /// <param name="response"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected async Task<T> ProcessResponse<T>(HttpResponseMessage response, Uri endpoint, object arguments)
    {
        response.EnsureSuccessStatusCode();

        string responseText = await response.Content.ReadAsStringAsync();
        logger.LogTrace("Received response: {r}", responseText);
        if (String.IsNullOrWhiteSpace(responseText))
        {
            throw new EmptyResponseException("WebService's response was empty\nEndpoint: " + endpoint + "\nRequest data: " +
                                             JsonSerializer.Serialize(arguments));
        }

        return JsonSerializer.Deserialize<T>(responseText);
    }

    private string MakeGetQuery(Dictionary<string, string> query)
    {
        if (query is null)
        {
            return "";
        }

        return string.Join("&",
            query.Select(kvp => Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value)));
    }

    private FormUrlEncodedContent MakeFormUrlEncodedArguments(object arguments)
    {
        Type t = arguments.GetType();
        PropertyInfo[] props = t.GetProperties();

        Dictionary<string, string> output = new Dictionary<string, string>();
        foreach (PropertyInfo p in props)
        {
            output[p.Name] = p.GetValue(arguments)?.ToString() ?? "";
        }

        return new FormUrlEncodedContent(output);
    }
}