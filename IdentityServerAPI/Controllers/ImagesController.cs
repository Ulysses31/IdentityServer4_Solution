using AutoMapper;
using IdentityServerAPI.Services;
using IdentityServerModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IdentityServerAPI.Controllers
{
	[ApiController]
	[Route("api/images")]
	public class ImagesController : ControllerBase
	{
		private readonly IGalleryRepository _galleryRepository;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly IMapper _mapper;

		public ImagesController(
				IGalleryRepository galleryRepository,
				IWebHostEnvironment hostingEnvironment,
				IMapper mapper)
		{
			_galleryRepository = galleryRepository ??
					throw new ArgumentNullException(nameof(galleryRepository));
			_hostingEnvironment = hostingEnvironment ??
					throw new ArgumentNullException(nameof(hostingEnvironment));
			_mapper = mapper ??
					throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet()]
		//[Authorize(Policy = "FullAccessPolicy")]

		public IActionResult GetImages()
		{
			var ownerId = User.Claims.FirstOrDefault(u => u.Type == "sub")?.Value;
			ownerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7";

			// get from repo
			var imagesFromRepo = _galleryRepository.GetImages(ownerId);

			// map to model
			var imagesToReturn = _mapper.Map<IEnumerable<IdentityServerModel.Image>>(imagesFromRepo);

			// return
			return Ok(imagesToReturn);
		}

		[HttpGet("{id}", Name = "GetImage")]
		public IActionResult GetImage(Guid id)
		{
			var imageFromRepo = _galleryRepository.GetImage(id);

			if (imageFromRepo == null)
			{
				return NotFound();
			}

			var imageToReturn = _mapper.Map<IdentityServerModel.Image>(imageFromRepo);

			return Ok(imageToReturn);
		}

		[HttpPost()]
		public IActionResult CreateImage([FromBody] ImageForCreation imageForCreation)
		{
			// Automapper maps only the Title in our configuration
			var imageEntity = _mapper.Map<Entities.Image>(imageForCreation);

			// Create an image from the passed-in bytes (Base64), and
			// set the filename on the image

			// get this environment's web root path (the path
			// from which static content, like an image, is served)
			var webRootPath = _hostingEnvironment.WebRootPath;

			// create the filename
			string fileName = Guid.NewGuid().ToString() + ".jpg";

			// the full file path
			var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

			// write bytes and auto-close stream
			System.IO.File.WriteAllBytes(filePath, imageForCreation.Bytes);

			// fill out the filename
			imageEntity.FileName = fileName;

			var ownerId = User.Claims.FirstOrDefault(o => o.Type == "sub")?.Value;
			imageEntity.OwnerId = ownerId;

			// add and save.
			_galleryRepository.AddImage(imageEntity);

			_galleryRepository.Save();

			var imageToReturn = _mapper.Map<Image>(imageEntity);

			return CreatedAtRoute("GetImage",
					new { id = imageToReturn.Id },
					imageToReturn);
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteImage(Guid id)
		{
			var imageFromRepo = _galleryRepository.GetImage(id);

			if (imageFromRepo == null)
			{
				return NotFound();
			}

			_galleryRepository.DeleteImage(imageFromRepo);

			_galleryRepository.Save();

			return NoContent();
		}

		[HttpPut("{id}")]
		public IActionResult UpdateImage(Guid id,
				[FromBody] ImageForUpdate imageForUpdate)
		{
			var imageFromRepo = _galleryRepository.GetImage(id);
			if (imageFromRepo == null)
			{
				return NotFound();
			}

			_mapper.Map(imageForUpdate, imageFromRepo);

			_galleryRepository.UpdateImage(imageFromRepo);

			_galleryRepository.Save();

			return NoContent();
		}
	}
}