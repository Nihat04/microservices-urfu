namespace Common.Contracts.Orders;

public record OrderFilterDto(
    Guid? UserId,
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    DateTime? UpdatedFrom,
    DateTime? UpdatedTo,
    OrderState? State
);
