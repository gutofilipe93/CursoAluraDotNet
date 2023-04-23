using RabbitMQ.Client;
using RestauranteService.Dtos;
using System.Text.Json;
using System.Text;
using System.Threading.Channels;

namespace RestauranteService.RabbitMqClient
{
    public class RabbitMqClient : IRabbitMqClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqClient(IConfiguration configuration)
        {                                    
            _configuration = configuration;
            _connection = new ConnectionFactory() { HostName = _configuration["RabbitMqHost"], Port = Int32.Parse(_configuration["RabbitMqPort"]) }.CreateConnection();
            _channel = _connection.CreateModel();            
            _channel.ExchangeDeclare(exchange: "trigger",type: ExchangeType.Fanout);
            //_channel.QueueDeclare(queue: "Nome_Fila");
        }

        public void PublicaRestaurante(RestauranteReadDto restauranteReadDto)
        {
            // Aqui não informa o nome da fala que ele esta enviando a mensagem
            // ExchangeDeclare e o Fanout -> Funciona como o Topic do serviceBus, ou seja, se eu tenho 10 filas ele vai enviar para as 10
            // Caso queria envia para somente uma fila olha na documentação como faz isso exatamente

            string mensagem = JsonSerializer.Serialize(restauranteReadDto);
            var body = Encoding.UTF8.GetBytes(mensagem);            
            _channel.BasicPublish(exchange: "trigger",             
             routingKey: "",
             basicProperties: null,
             body: body            
             );
        }
    }
}
