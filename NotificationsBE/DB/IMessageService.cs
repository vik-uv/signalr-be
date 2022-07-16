using NotificationsBE.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationsBE.DB
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetMessagesAsync(int count, CancellationToken cancellationToken);
        Task<Message> GetMessageAsync(string id, CancellationToken cancellationToken);
        Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken);
        Task<Message> UpdateMessageAsync(Message message, CancellationToken cancellationToken);
        Task DeleteMessageAsync(string id, CancellationToken cancellationToken);
    }
}
