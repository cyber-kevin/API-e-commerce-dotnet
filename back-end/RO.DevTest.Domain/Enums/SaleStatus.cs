namespace RO.DevTest.Domain.Enums;

/// <summary>
/// Enum representing the sale status in the API
/// </summary>
public enum SaleStatus
{
    /// <summary>
    /// Sale is pending
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Sale is approved
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Sale is sent
    /// </summary>
    Sent = 3,

    /// <summary>
    /// Sale is delivered
    /// </summary>
    Delivered = 4,

    /// <summary>
    /// Sale is cancelled
    /// </summary>
    Cancelled = 3,
}
