Para a implanta��o do Redis � necess�rio instal�-lo no servidor.
O execut�vel pode ser encontrado em: https://github.com/rgl/redis/downloads
Al�m disso essa classe tem como depend�ncia o NewtonSoft.Json e StackExchange.Redis que podem ser instalados via NuGet Package Manager no Visual Studio.

A descri��o dos m�todos est� no c�digo.

Mas deve-se tomar cuidado quando salvar objetos nele, pois todos s�o transformados em Json antes de irem para o Cache e caso queira salvar uma entidade que tem relacionamentos, isso pode gerar conflitos, pois uma entidade que contenha a outra e vice-versa gerar� um loop de refer�ncia.