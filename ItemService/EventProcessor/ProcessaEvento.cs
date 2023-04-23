using AutoMapper;
using ItemService.Data;
using ItemService.Dtos;
using ItemService.Models;
using System.Text.Json;

namespace ItemService.EventProcessor
{
    public class ProcessaEvento : IProcessaEvento
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory; // Usado isso pois o RabbitMq é AddSingleton e o IItemRepository é AddScoped, pelo fato dele ser AddScoped precisa disso

        public ProcessaEvento(IMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _mapper = mapper;
            _scopeFactory = scopeFactory;
        }

        public void Processa(string mensagem)
        {
            using var scope = _scopeFactory.CreateScope();
            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
            var restauranteReadDto = JsonSerializer.Deserialize<RestauranteReadDto>(mensagem);
            var restaurante = _mapper.Map<Restaurante>(restauranteReadDto);
            if (itemRepository.ExisteRestauranteExterno(restaurante.Id))
                return;

            itemRepository.CreateRestaurante(restaurante);
            itemRepository.SaveChanges();
        }
    }
}
