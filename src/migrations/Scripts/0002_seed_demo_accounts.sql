-- Dados mínimos para testes manuais. O gerador da Etapa 4 cria o volume real.
INSERT INTO accounts (account_id, customer_id, credit_limit, available) VALUES
    (1, 1, 1000.00, 1000.00),
    (2, 2,  500.00,  500.00),
    (3, 3,   50.00,   50.00);

INSERT INTO cards (card_token, account_id, status, is_additional) VALUES
    ('tok_demo_0001', 1, 'ACTIVE',  0),
    ('tok_demo_0002', 1, 'ACTIVE',  1),   -- adicional: divide o limite da conta 1
    ('tok_demo_0003', 3, 'ACTIVE',  0),   -- limite baixo: testa saldo insuficiente
    ('tok_demo_0004', 2, 'BLOCKED', 0);   -- bloqueado