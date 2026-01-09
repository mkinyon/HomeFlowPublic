using System.ComponentModel.DataAnnotations;

namespace HomeFlow.Models;

public abstract class Model
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    public Guid? ForeignId { get; set; } = null;
}
