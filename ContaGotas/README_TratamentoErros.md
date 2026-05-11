# Tratamento de erros e exceções
Nesta etapa foram implementadas exceções e validações para fornecer uma melhor experiência de utilização da aplicação. 
Esta versão contempla a funcionalidade de listagem das médias com validações nas chamadas REST API da DGEG e nos dados utilizados provenientes da sua resposta de forma a garantir a estabilidade e funcionamento correto.

### Testes de unidade
Para comprovar o funcionamento de alguns métodos e funcionalidades incluímos também alguns testes unitários, identificados no plano de testes e implementados com o avanço do desenvolvimento.

### Observações
Na fase atual foi detetada uma instabilidade critica do programa em sistemas operativos diferentes de windows para a implementação da API ZedGraph. 
Para evitar um crash do programa e devido a restrições de tempo, algumas funcionalidades não essenciais não foram incluídas nesta versão. No entanto, estão presentes e desenvolvidas no repositório online, em fase de testes para avaliar o comportamento nos restantes sistemas operativos, MacOS e linux, com o objetivo de avaliar melhor a compatibilidade da sua implementação. Desta forma garantimos a integridade do sistema enquanto procuramos soluções permanentes.