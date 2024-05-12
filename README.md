# Sobre o projeto

O projeto foi desenvolvido para analisar operações disponibilizadas em arquivos texto.
Na branch Master, encontra-se o código que analisa as categorias EXPIRED, HIGHRISK e MEDIUMRISK. Já na branch PEPCategory, está o código ajustado para considerar também a categoria PEP, no item 2 da especificação.
Não estava claro na especificação se uma operação pode ou não ser classificada em mais de uma categoria. E, por não haver nenhum exemplo de uma operação com duas ou mais categorias, considerei que a operação se encaixará na primeira categoria em que estiver totalmente aderente.

## Para criar uma nova categoria

* Ajustar a interface ITrade, acrescentando as propriedades necessárias
* Ajustar a classe TradeDTO:
  * Implementar as novas propriedades da interface
  * Ajustar o construtor para contemplar as novas propriedades
  * Ajustar o método TryParse para utilizar as novas propriedades
* Criar a classe de regra correspondente à nova categoria
* Atualizar o arquivo categories.json para criar a nova categoria
* Ajustar os arquivos de massa de dados para que sejam incluídas as novas informações
