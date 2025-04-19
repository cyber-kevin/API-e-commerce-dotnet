namespace RO.DevTest.Domain.Enums;

/// <summary>
/// Enum representing the payment methods available in the API
/// </summary>

public enum PaymentMethod
{
    /// <summary>
    /// Payment made in cash
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Payment made by credit card
    /// </summary>
    CreditCard = 2,

    /// <summary>
    /// Payment made by debit card
    /// </summary>
    DebitCard = 3,

    /// <summary>
    /// Payment made by bank transfer
    /// </summary>
    BankTransfer = 4,

    /// <summary>
    /// Payment made by PIX
    /// </summary>
    Pix = 5,

    /// <summary>
    /// Payment made by Boleto
    /// </summary>
    Boleto = 6,
}
