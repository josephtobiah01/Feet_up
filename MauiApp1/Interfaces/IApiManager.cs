using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Interfaces
{
    public interface IApiManager
    {
        public Task<T> PostForResponseAsync<T>(string uriRequest, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
    }
}
