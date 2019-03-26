using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace RedisCache
{
    public class RedisService
    {
        private static string ConnectionString;

        // <param name="connectionString">
        // Nome e porta do servidor onde o Redis está instalado.
        // O Parâmetro allowAdmin é setado como true automaticamente para usar o FlushAll.
        // </param>
        public RedisService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        // <summary>
        // Verifica se a chave especificada existe no Redis, se existir retorna, senão ele cria a chave usando a função de callback.
        // </summary>
        // <param name="key">
        // Nome da chave que será buscada ou criada.
        // </param>
        // <param name="callback">
        // Função que retorna o valor do banco de dados caso não seja encontrado no Redis e já salva esse valor no Redis para chamadas futuras.
        // OBS: Entidades com Many To Many podem dar erro de loop.
        // </param>
        // <param name="time">
        // Tempo de vida desse registro no Redis.
        // </param>
        // <param name="parameter">
        // Parâmetros da função de callback. 
        // Em geral pode ser deixado em branco, pois quando criar a função e colocar os parâmetros lá eles já serão chamados na execução da mesma.
        // </param>
        public T GetOrSetToRedis<T>(string key, Func<object, T> callback, int time = 0, params object[] parameter)
        {
            object retorno = null;

            IDatabase cache = Connection.GetDatabase();
            string serializedObj = cache.StringGet(key);
            if (serializedObj != null)
            {
                retorno = JsonConvert.DeserializeObject<T>(serializedObj);
            }
            else
            {
                retorno = callback(parameter);
                serializedObj = JsonConvert.SerializeObject(retorno);
                cache.StringSet(key, serializedObj);
                if (time > 0)
                    cache.KeyExpire(key, getSpan(time));
            }

            return (T)retorno;
        }

        // <summary>
        // Limpa os registros do Redis que coincidam com o pattern especificado.
        // </summary>
        // <param name="pattern">
        // Valor que será comparado.
        // </param>
        public void FlushAll(string pattern = "")
        {
            var server = Connection.GetServer(ConnectionString);
            if (string.IsNullOrEmpty(pattern))
            {
                server.FlushDatabase();
            }
            else
            {
                var keys = server.Keys();
                IDatabase cache = Connection.GetDatabase();
                foreach (var key in keys)
                {
                    if (key.ToString().Contains(pattern))
                    {
                        cache.KeyDelete(key);
                    }
                }
            }
        }

        private Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConnectionString;
            return ConnectionMultiplexer.Connect(cacheConnection + ", allowAdmin=true");
        });
        private ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        private TimeSpan getSpan(int time)
        {
            int expireTime = time;
            return new TimeSpan(0, 0, expireTime);
        }
    }
}
