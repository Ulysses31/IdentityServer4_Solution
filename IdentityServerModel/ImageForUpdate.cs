using System.ComponentModel.DataAnnotations;

namespace IdentityServerModel
{
	public class ImageForUpdate
	{
		[Required]
		[MaxLength(150)]
		public string Title { get; set; }
	}
}