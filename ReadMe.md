Para a implantação do Redis é necessário instalá-lo no servidor.
O executável pode ser encontrado em: https://github.com/rgl/redis/downloads
Além disso essa classe tem como dependência o NewtonSoft.Json e StackExchange.Redis que podem ser instalados via NuGet Package Manager no Visual Studio.

A descrição dos métodos está no código.

Mas deve-se tomar cuidado quando salvar objetos nele, pois todos são transformados em Json antes de irem para o Cache e caso queira salvar uma entidade que tem relacionamentos, isso pode gerar conflitos, pois uma entidade que contenha a outra e vice-versa gerará um loop de referência.