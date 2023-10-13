using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.RabbitMQ
{
    public class DocumentProducer
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;

        #region Singleton Section
        private static readonly Lazy<DocumentProducer> _instance = new Lazy<DocumentProducer>(() => new DocumentProducer());

        private DocumentProducer()
        {
            string uri = Environment.GetEnvironmentVariable("RABBIT_URI");
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(uri)
            };
            _connection = _connectionFactory.CreateConnection();
        }

        public static DocumentProducer Instance => _instance.Value;

        #endregion

        public void EnqueItem(byte[] messageQueueItemBytes, string queueName)
        {
            using (IModel _model = _connection.CreateModel())
            {
                _model.QueueDeclare(queueName, true, false, false, null);
                _model.BasicPublish(exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: messageQueueItemBytes);
            }
        }
    }
}
