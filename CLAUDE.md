# Orbit Pay — guia para assistentes de IA

## Contexto
Arquitetura de referência fictícia de um emissor de cartões de varejo.
Meta: 1M clientes, 10M transações/mês (~3,9 TPS médios), picos de 500 TPS.
Projeto público e didático. Nenhum dado, nome ou número de empresa real.

## Stack
- .NET 10, ASP.NET Core Minimal APIs, Aspire (AppHost + ServiceDefaults)
- SQL Server (ledger, limites, outbox), Redis (cache), Kafka local / Event Hubs no Azure
- Polly v8 para resiliência, OpenTelemetry para telemetria
- Testes: xUnit + Testcontainers

## Estrutura
- `OrbitPay.AppHost` — orquestração local
- `OrbitPay.ServiceDefaults` — telemetria, health checks e resiliência compartilhados
- `src/services/*` — um serviço por pasta
- `src/simulators/*` — simuladores de dependências externas
- `docs/adr/` — decisões de arquitetura (formato: contexto, decisão, consequências)

## Regras obrigatórias
1. Toda escrita na API exige o header `Idempotency-Key`.
2. Gravar estado e evento na MESMA transação SQL (outbox). Nunca publicar no Kafka dentro da requisição.
3. Atualização de limite com `UPDATE ... WHERE available >= @amount` (condicional e atômico). Nunca ler e depois atualizar.
4. Partição dos eventos = `cardToken`.
5. Consumidores sempre idempotentes (entrega at-least-once).
6. `traceparent` propagado nos headers das mensagens, nunca no payload.
7. Toda chamada externa passa por um pipeline Polly com timeout explícito.

## Nunca fazer
- Armazenar ou logar PAN, CVV ou dados pessoais reais. Cartões são sempre tokens (`tok_...`).
- Colocar segredos ou connection strings no código ou no repositório.
- Fazer chamada síncrona entre consumidores.
- Adicionar pacotes sem justificativa; preferir o que já está na solução.

## Convenções
- Nomes de código em inglês; documentação em inglês e português.
- Métricas com prefixo `orbit.` (ex.: `orbit.authorizations`).
- Commits no padrão Conventional Commits (`feat:`, `fix:`, `chore:`, `docs:`, `test:`).
- Todo código novo vem acompanhado de teste.