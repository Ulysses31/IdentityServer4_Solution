using System.ComponentModel.DataAnnotations;

namespace IdentityServerModel
{
	public class ImageForCreation
	{
		[Required]
		[MaxLength(150)]
		public string Title { get; set; }

		[Required]
		public byte[] Bytes { get; set; }
	}
}