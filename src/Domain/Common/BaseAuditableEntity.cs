namespace CleanArchitecture.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }
    
    #if (UseAuthentication)
    public string? CreatedBy { get; set; }
    #endif

    public DateTimeOffset LastModified { get; set; }

    #if (UseAuthentication)
    public string? LastModifiedBy { get; set; }
    #endif
}
