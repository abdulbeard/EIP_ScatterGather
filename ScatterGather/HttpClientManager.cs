using ScatterGather.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterGather
{
    public class HttpClientManager : IClientManager<HttpResponseMessage, HttpRequestMessageWrapper>
    {
        private static Dictionary<int, SgHttpClient> _dict;

        public HttpClientManager()
        {
            if(_dict == null)
            {
                _dict = new Dictionary<int, SgHttpClient>();
            }
        }

        public IClient<HttpResponseMessage, HttpRequestMessageWrapper> GetClient(IHost host)
        {
            if (!(host is HttpHost))
            {
                return null;
            }
            var httpHost = (HttpHost) host;
            var hostnameHash = host.Host.GetHashCode();
            if (!_dict.ContainsKey(hostnameHash))
            {
                AddClient(httpHost, new SgHttpClient(new HttpClient() {
                    BaseAddress = new Uri(httpHost.Host.ToString())
                }));
            }
            return _dict[hostnameHash];
        }

        public void AddClient(IHost host, IClient<HttpResponseMessage, HttpRequestMessageWrapper> client)
        {
            if (!_dict.ContainsKey(host.GetHashCode()))
            {
                if (client is SgHttpClient sgHttpClient)
                    _dict.Add(sgHttpClient.BaseAddress.ToString().GetHashCode(), sgHttpClient);
            }
        }
    }

    public interface IClientManager<TOut, TIn>
    {
        /// <summary>
        /// Gets a client 
        /// </summary>
        /// <param name="host">Is used to uniquely identify a client i.e. one client per host</param>
        /// <returns></returns>
        IClient<TOut, TIn> GetClient(IHost host);

        /// <summary>
        /// Adds a client
        /// </summary>
        /// <param name="host">uniquely identifies a destination of the message</param>
        /// <param name="client">client that communicates with the controller/host at the destination</param>
        void AddClient(IHost host, IClient<TOut, TIn> client);
    }

    public interface IClient<TOut, TIn>
    {
        Task<TOut> SendAsync(TIn request, CancellationToken ct);
    }

    public class SgHttpClient : IClient<HttpResponseMessage, HttpRequestMessageWrapper>
    {
        private readonly HttpClient _httpClient;
        public SgHttpClient(HttpClient client)
        {
            _httpClient = client;
        }
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessageWrapper request, CancellationToken ct)
        {
            return _httpClient.SendAsync(request, ct);
        }

        public Uri BaseAddress => _httpClient.BaseAddress;
    }
}
