using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkerServiceRabbitmq
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public string con { get; set; } = "server=localhost;user=root;database=mysql;port=3306;password=sprint1234";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string queueName = "qtest1";
            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";
            factory.Port = 5672;

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();
            channel.QueueDeclare(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
                Request req = JsonConvert.DeserializeObject<Request>(message);
                Console.WriteLine(req.command + " - " + req.data.Nama + "-" + req.data.Status);
                switch (req.command)
                {
                    case "create":
                        Insert(req.data.Nama, req.data.Status);
                        break;
                    case "update":
                        Update(req.data.Nama, req.data.Status, req.data.id);
                        break;
                    case "delete":
                        Delete(req.data.id);
                        break;
                }
            };
            channel.BasicConsume(queueName, true, consumer);

        }
        public bool Insert(string nama, int status)
        {
            Response r = new Response();
            int result = 0;
            bool re = false;
            using (MySqlConnection conn = new MySqlConnection(this.con))
            {
                conn.Open();
                string insert = "insert into Test01 (nama,status) values ('" + nama + "'," + status + ")";

                MySqlCommand cmdQueue = new MySqlCommand(insert, conn);
                cmdQueue.CommandTimeout = 3600;
                result = cmdQueue.ExecuteNonQuery();
                if (result > 0)
                {
                    r.message = "Berhasil Insert";
                    re = true;

                }
            }
            return re;
        }
        public bool Update(string nama, int status, int id)
        {
            Response r = new Response();
            bool re = false;
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(this.con))
            {
                conn.Open();
                string insert = "update Test01 set nama = '" + nama + "',status=" + status + " where id =" + id + "";

                MySqlCommand cmdQueue = new MySqlCommand(insert, conn);
                cmdQueue.CommandTimeout = 3600;
                result = cmdQueue.ExecuteNonQuery();
                if (result > 0)
                {
                    r.message = "Berhasil Update";
                    re = true;

                }
            }
            return re;
        }
        public bool Delete(int id)
        {
            Response r = new Response();
            bool re = false;
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(this.con))
            {
                conn.Open();
                string insert = "delete from Test01 where id =" + id + "";

                MySqlCommand cmdQueue = new MySqlCommand(insert, conn);
                cmdQueue.CommandTimeout = 3600;
                result = cmdQueue.ExecuteNonQuery();
                if (result > 0)
                {
                    r.message = "Berhasil Delete";
                    re = true;

                }
            }
            return re;
        }
    }
}
