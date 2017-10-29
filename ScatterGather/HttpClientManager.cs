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
        private static Dictionary<int, SGHttpClient> dict;

        public HttpClientManager()
        {
            if(dict == null)
            {
                dict = new Dictionary<int, SGHttpClient>();
            }
        }

        public IClient<HttpResponseMessage, HttpRequestMessageWrapper> GetClient(string host)
        {
            var hostnameHash = host.GetHashCode();
            if (!dict.ContainsKey(hostnameHash))
            {
                AddClient(host, new SGHttpClient {
                    BaseAddress = new Uri(host)
                });
            }
            return dict[hostnameHash];
        }

        public void AddClient(string host, IClient<HttpResponseMessage, HttpRequestMessageWrapper> client)
        {
            if (!dict.ContainsKey(host.GetHashCode()))
            {
                var sgHttpClient = client as SGHttpClient;
                dict.Add(sgHttpClient.BaseAddress.ToString().GetHashCode(), sgHttpClient);
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
        IClient<TOut, TIn> GetClient(string host);
        /// <summary>
        /// Adds a client
        /// </summary>
        /// <param name="host">uniquely identifies a destination of the message</param>
        void AddClient(string host, IClient<TOut, TIn> client);
    }

    public interface IClient<TOut, TIn>
    {
        Task<TOut> SendAsync(TIn request, CancellationToken ct);
    }

    public class SGHttpClient : HttpClient, IClient<HttpResponseMessage, HttpRequestMessageWrapper>
    {
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessageWrapper request, CancellationToken ct)
        {
            return base.SendAsync(request, ct);
        }
    }
}
