using IdentityServerModel;
using System.Collections.Generic;

namespace IdentityServerClient.ViewModels
{
	public class GalleryIndexViewModel
	{
		public IEnumerable<Image> Images { get; private set; }
				= new List<Image>();

		public GalleryIndexViewModel(IEnumerable<Image> images)
		{
			Images = images;
		}
	}
}