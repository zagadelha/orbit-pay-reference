-- Contas: o limite é da conta, compartilhado entre titular e adicionais
CREATE TABLE accounts (
    account_id    BIGINT        NOT NULL PRIMARY KEY,
    customer_id   BIGINT        NOT NULL,
    credit_limit  DECIMAL(12,2) NOT NULL,
    available     DECIMAL(12,2) NOT NULL,
    currency      CHAR(3)       NOT NULL DEFAULT 'EUR',
    updated_at    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    row_version   ROWVERSION,
    CONSTRAINT ck_accounts_available CHECK (available >= 0 AND available <= credit_limit)
);
CREATE INDEX ix_accounts_customer ON accounts(customer_id);

-- Cartões: sempre por token, nunca PAN
CREATE TABLE cards (
    card_token    VARCHAR(40)  NOT NULL PRIMARY KEY,
    account_id    BIGINT       NOT NULL REFERENCES accounts(account_id),
    status        VARCHAR(20)  NOT NULL DEFAULT 'ACTIVE',
    is_additional BIT          NOT NULL DEFAULT 0,
    created_at    DATETIME2    NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX ix_cards_account ON cards(account_id);

-- Autorizações: chave sequencial para inserção eficiente sob carga
CREATE TABLE authorizations (
    authorization_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    card_token       VARCHAR(40)   NOT NULL,
    account_id       BIGINT        NOT NULL,
    amount           DECIMAL(12,2) NOT NULL,
    currency         CHAR(3)       NOT NULL,
    merchant_id      VARCHAR(40)   NOT NULL,
    mcc              CHAR(4)       NOT NULL,
    status           VARCHAR(20)   NOT NULL,   -- APPROVED | DECLINED
    decline_reason   VARCHAR(40)   NULL,
    stand_in         BIT           NOT NULL DEFAULT 0,  -- aprovada via STIP
    created_at       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX ix_authorizations_card ON authorizations(card_token, created_at);

-- Idempotência: a mesma chave devolve a mesma resposta
CREATE TABLE idempotency_keys (
    idempotency_key VARCHAR(100)  NOT NULL PRIMARY KEY,
    request_hash    BINARY(32)    NOT NULL,
    status_code     INT           NULL,
    response_body   NVARCHAR(MAX) NULL,
    created_at      DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    expires_at      DATETIME2     NOT NULL
);
CREATE INDEX ix_idempotency_expires ON idempotency_keys(expires_at);

-- Outbox: evento gravado na mesma transação do estado
CREATE TABLE outbox (
    id            BIGINT IDENTITY  NOT NULL PRIMARY KEY,
    event_id      UNIQUEIDENTIFIER NOT NULL UNIQUE,
    partition_key VARCHAR(40)      NOT NULL,
    event_type    VARCHAR(100)     NOT NULL,
    payload       NVARCHAR(MAX)    NOT NULL,
    traceparent   VARCHAR(55)      NULL,
    created_at    DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    published_at  DATETIME2        NULL
);
CREATE INDEX ix_outbox_pending ON outbox(id) WHERE published_at IS NULL;