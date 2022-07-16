using NotificationsBE.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;

namespace NotificationsBE.DB
{
    public class MessageService : IMessageService
    {
        private readonly DocumentClient _client;
        private readonly IConfiguration _configuration;
        private readonly Uri _uri;

        public MessageService(
            DocumentClient client,
            IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            _uri = UriFactory.CreateDocumentCollectionUri(configuration["DatabaseName"], configuration["CollectionName"]);
        }

        public async Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var resp = await _client.CreateDocumentAsync(_uri, message, null, false, cancellationToken);
            return JsonConvert.DeserializeObject<Message>(resp.Resource.ToString());
        }

        public async Task DeleteMessageAsync(string id, CancellationToken cancellationToken)
        {
            var documentUri = UriFactory.CreateDocumentUri(_configuration["DatabaseName"], _configuration["CollectionName"], id);
            await _client.DeleteDocumentAsync(documentUri, null, cancellationToken);
        }

        public async Task<Message> GetMessageAsync(string id, CancellationToken cancellationToken)
        {
            var documentUri = UriFactory.CreateDocumentUri(_configuration["DatabaseName"], _configuration["CollectionName"], id);
            return await _client.ReadDocumentAsync<Message>(documentUri, new RequestOptions { PartitionKey = new PartitionKey(id) }, cancellationToken);
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(int count, CancellationToken cancellationToken)
        {
            var options = new FeedOptions { EnableCrossPartitionQuery = true }; // Enable cross partition query
            var query = _client.CreateDocumentQuery<Message>(_uri, options).OrderByDescending(x => x.DateTime).Take(count).AsDocumentQuery();
            var list = new List<Message>();
            while (query.HasMoreResults)
            {
                foreach (var x in await query.ExecuteNextAsync<Message>(cancellationToken))
                {
                    list.Add(x);
                }
            }
            return list;
        }

        public async Task<Message> UpdateMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var doc = await _client.UpsertDocumentAsync(_uri, message, null, false, cancellationToken);
            return JsonConvert.DeserializeObject<Message>(doc.Resource.ToString());
        }
    }
}
