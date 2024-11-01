using Microsoft.AspNetCore.Mvc;
using System.Text;
using MyApp.Services;
using MyApp.Models;
using MyApp.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace web_app_performance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoRepository _repository;
        private readonly RabbitMQService _rabbitMQService;
        private readonly IConnectionMultiplexer _redis;


        public ProdutoController(IProdutoRepository repository, RabbitMQService rabbitMQService, IConnectionMultiplexer redis)
        {
            _repository = repository;
            _rabbitMQService = rabbitMQService;
            _redis = redis;
        }

        [HttpGet]
        public async Task<IActionResult> GetProdutos()
        {
            string key = "getprodutos";
            var db = _redis.GetDatabase();

            // Verifique se existe no cache
            string produtosCache = await db.StringGetAsync(key);
            if (!string.IsNullOrEmpty(produtosCache))
            {
                return Ok(produtosCache);
            }

            // Se não houver cache, busca no banco
            var produtos = await _repository.ListarProdutosAsync();
            if (produtos == null)
            {
                return NotFound();
            }

            string produtosJson = JsonConvert.SerializeObject(produtos);

            // Salve no cache e defina um tempo de expiração
            await db.StringSetAsync(key, produtosJson, TimeSpan.FromSeconds(10));

            return Ok(produtos);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            await _repository.SalvarProdutoAsync(produto);

            // Publica a mensagem no RabbitMQ
            _rabbitMQService.Publish(produto, "produtos");

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Produto produto)
        {
            await _repository.AtualizarProdutoAsync(produto);

            // Limpar o cache
            string key = "getprodutos";
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeletarProdutoAsync(id);

            // Limpar o cache
            string key = "getprodutos";
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }
    }
}

