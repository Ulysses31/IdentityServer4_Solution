using AutoMapper;

namespace IdentityServerAPI.Profiles
{
	public class ImageProfile : Profile
	{
		public ImageProfile()
		{
			// map from Image (entity) to Image, and back
			CreateMap<Entities.Image, IdentityServerModel.Image>().ReverseMap();

			// map from ImageForCreation to Image
			// Ignore properties that shouldn't be mapped
			CreateMap<IdentityServerModel.ImageForCreation, Entities.Image>()
					.ForMember(m => m.FileName, options => options.Ignore())
					.ForMember(m => m.Id, options => options.Ignore())
					.ForMember(m => m.OwnerId, options => options.Ignore());

			// map from ImageForUpdate to Image
			// ignore properties that shouldn't be mapped
			CreateMap<IdentityServerModel.ImageForUpdate, Entities.Image>()
					.ForMember(m => m.FileName, options => options.Ignore())
					.ForMember(m => m.Id, options => options.Ignore())
					.ForMember(m => m.OwnerId, options => options.Ignore());
		}
	}
}