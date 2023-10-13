using LibraryApp.Data.Models;
using LibraryApp.DataAccess;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.RabbitMQ
{
    public class DocumentConsumer
    {
        #region Singleton Section

        private static readonly Lazy<DocumentConsumer>
         _instance = new Lazy<DocumentConsumer>(() => new DocumentConsumer());

        private DocumentConsumer()
        {
        }

        public static DocumentConsumer Instance => _instance.Value;

        #endregion

        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _saveQueueModel;
        private IModel _queryQueueModel;

        public void StartConsume(string queueName)
        {
            queueName = $"Library-{queueName}";
            string uri = Environment.GetEnvironmentVariable("RABBIT_URI");
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(uri)
            };

            _connection = _connectionFactory.CreateConnection();
            _saveQueueModel = _connection.CreateModel();
            _queryQueueModel = _connection.CreateModel();

            _saveQueueModel.QueueDeclare(queueName, true, false, false, null);
            EventingBasicConsumer documentConsumer = new EventingBasicConsumer(_saveQueueModel);
            documentConsumer.Received += DocumentReceived;
            _saveQueueModel.BasicConsume(queueName, false, documentConsumer);
        }

        public void DocumentReceived(object sender, BasicDeliverEventArgs e)
        {
            var jsonData = Encoding.UTF8.GetString(e.Body.ToArray());
            BookModel queueItem = JsonConvert.DeserializeObject<BookModel>(jsonData);
            using (MongoRepository<BookModel> repository = new MongoRepository<BookModel>())
            {
                repository.Add(queueItem);
            }
            _saveQueueModel.BasicAck(e.DeliveryTag, false);
        }
    }
}
