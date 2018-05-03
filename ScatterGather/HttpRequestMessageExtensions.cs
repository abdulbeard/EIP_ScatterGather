using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using ScatterGather.Request;

namespace ScatterGather
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage Copy(this HttpRequestMessage request)
        {
            var result = new HttpRequestMessage {Method = request.Method};
            foreach (var header in request.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            foreach (var property in request.Properties)
            {
                result.Properties.Add(property);
            }

            result.RequestUri = request.RequestUri;
            result.Version = request.Version;
            var memoryStream = new MemoryStream();
            request.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            result.Content = new StreamContent(memoryStream);
            return result;
        }

        public static HttpRequestMessageWrapper Copy(this HttpRequestMessageWrapper request)
        {
            var result = new HttpRequestMessageWrapper { Method = request.Method };
            foreach (var header in request.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            foreach (var property in request.Properties)
            {
                result.Properties.Add(property);
            }

            result.RequestUri = request.RequestUri;
            result.Version = request.Version;
            var memoryStream = new MemoryStream();
            request.Content?.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            result.Content = new StreamContent(memoryStream);
            return result;
        }
    }
}
