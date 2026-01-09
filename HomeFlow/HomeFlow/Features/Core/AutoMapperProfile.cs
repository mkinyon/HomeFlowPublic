namespace HomeFlow.Features.Core;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<Features.Core.ImageFiles.ImageFileRequest, Features.Core.ImageFiles.ImageFileEntity>().ReverseMap();
        CreateMap<Features.Core.ImageFiles.ImageFile, Features.Core.ImageFiles.ImageFileEntity>().ReverseMap();
        CreateMap<Features.Core.FamilyMembers.FamilyMember, Features.Core.FamilyMembers.FamilyMemberEntity>().ReverseMap();
        CreateMap<Features.Core.Tags.Tag, Features.Core.Tags.TagsRequest>().ReverseMap();
        CreateMap<Features.Core.Tags.Tag, Features.Core.Tags.TagEntity>().ReverseMap();
        CreateMap<Features.Core.SystemSettings.SystemSetting, Features.Core.SystemSettings.SystemSettingEntity>().ReverseMap();
    }
}
