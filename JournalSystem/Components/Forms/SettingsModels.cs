using System.ComponentModel.DataAnnotations;

namespace JournalSystem.Components.Forms;

public sealed class AddItemFormModel
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}

public sealed class UpdateNameFormModel
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
}

public sealed class ResetPinFormModel
{
    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string OldPin { get; set; } = string.Empty;

    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string NewPin { get; set; } = string.Empty;

    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string ConfirmPin { get; set; } = string.Empty;
}
