# Implementação de interfaces
A utilização de interfaces na nossa aplicação demonstradora demonstrou-se realmente útil em algumas situações, promovendo principalmente o baixo acoplamento entre componentes e garantindo uma maior escalabilidade devido à sua utilização. 

Foram aplicadas duas interfaces no projeto: ICombustivelService e IValido:
### Implementação da interface ICombustivelService:
Esta foi adicionada para permitir uma menor dependência entre o Model e a classe que chama a API da DGEG. Desta forma e se eventualmente tivermos a necessidade de utilizar persistência ou outra REST API, conseguimos adaptar o sistema sem grandes modificações no Model desde que utilizemos os contratos definidos na interface.

### Implementação da interface IValido:
De forma a garantir que todos os dados obtidos da API são validos e utilizáveis, após a desserialização do JSON de resposta e interpretação dos dados para os respetivos modelos de dados, demonstrou-se necessário realizar a validação. Aqui a interface demonstrou-se essencial, uma vez que permiti-nos a validação dos dados nesse momento (antes de os utilizar no Model e apresentar na View) sem depender propriamente de um tipo de dados, desde que estes modelos utilizem a interface IValido.


## Observações:
Para realizar a entrega do código foi necessário remover os ficheiros que permitem a compilação e debug do projeto (bin e obj), 
pelo que é necessário executar os passos necessários para recompilação e execução do projeto:

##### Pre-requisitos: 
- .NET 10 SDK** (ou superior) instalado na máquina.

##### Passos para execução do projeto:
- Abrir o Terminal/Linha de Comandos na raiz do projeto descompactado.
- Navegar até à pasta do projeto principal (onde se encontra o ficheiro ContaGotas.csproj): cd ContaGotas/ContaGotas 
- Executar: dotnet run
 
##### Passos para execução dos testes:
- Navegar até à pasta dos testes: cd ContaGotas/ContaGotas.Tests
- Executar: dotnet tes