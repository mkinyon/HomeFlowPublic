namespace HomeFlow.Features.Core.SystemSettings;

public class UpdateSystemSettingCommandValidator : AbstractValidator<UpdateSystemSettingCommand>
{
    public UpdateSystemSettingCommandValidator()
    {
        RuleFor( x => x.SystemSetting )
            .NotNull().WithMessage( "System setting is required." );

        When( x => x.SystemSetting != null, () =>
        {
            RuleFor( x => x.SystemSetting.Key )
                .NotEmpty().WithMessage( "Setting key is required." )
                .MaximumLength( 100 ).WithMessage( "Setting key cannot exceed 100 characters." );

            RuleFor( x => x.SystemSetting.Value )
                .MaximumLength( 1000 ).WithMessage( "Setting value cannot exceed 1000 characters." );
        } );
    }
}