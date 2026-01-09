
using HomeFlow.Services;

namespace HomeFlow.Features.Core.FamilyMembers;

public interface IFamilyMemberService : IService<FamilyMember> { }

public class FamilyMemberService : BaseService<FamilyMember>, IFamilyMemberService
{
    public FamilyMemberService( IMediator mediator ) : base( mediator ) { }

    public override Task<Guid> CreateAsync( FamilyMember item ) =>
        _mediator.Send( new CreateFamilyMemberCommand( item ) );

    public override Task UpdateAsync( FamilyMember item ) =>
        _mediator.Send( new UpdateFamilyMemberCommand( item ) );

    public override Task DeleteAsync( Guid id ) =>
        _mediator.Send( new DeleteFamilyMemberCommand( id ) );

    public override Task<FamilyMember?> GetByIdAsync( Guid id ) =>
        _mediator.Send( new GetFamilyMemberById( id ) );

    public override Task<List<FamilyMember>> GetListAsync() =>
        _mediator.Send( new GetFamilyMembersQuery() );

    public override Task<TableData<FamilyMember>> GetTableDataAsync( QueryOptions options ) =>
        throw new NotImplementedException( "GetListAsync with QueryOptions is not implemented for FamilyMemberService." );
}
