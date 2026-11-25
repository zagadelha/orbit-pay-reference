namespace OrbitPay.Authorization;

public sealed record AuthorizationRequest(
    string CardToken,
    decimal Amount,
    string Currency,
    string MerchantId,
    string Mcc);

public sealed record AuthorizationResponse(
    Guid AuthorizationId,
    string Status,
    string? DeclineReason,
    decimal Amount,
    string Currency,
    DateTime CreatedAt);