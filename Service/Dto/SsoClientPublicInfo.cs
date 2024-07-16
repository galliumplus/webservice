using GalliumPlus.WebApi.Core.Applications;

namespace GalliumPlus.WebApi.Dto
{
    public class SsoClientPublicInfo
    {
        public string DisplayName { get; set; } = "";
        public string? LogoUrl { get; set; }

        public class Mapper : Mapper<SsoClient, SsoClientPublicInfo>
        {
            public override SsoClient ToModel(SsoClientPublicInfo dto)
            {
                throw new InvalidOperationException(
                    "Les informations publiques des applications SSO sont en lecture seule.");
            }

            public override SsoClientPublicInfo FromModel(SsoClient model)
            {
                return new SsoClientPublicInfo { DisplayName = model.Name, LogoUrl = model.LogoUrl };
            }
        }
    }
}