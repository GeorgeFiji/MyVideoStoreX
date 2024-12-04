using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVideostore.Data;
using MyVideostore.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace MyVideostore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject the ApplicationDbContext into the controller through the constructor
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // This action handles GET requests and shows the video submission form
        public async Task<IActionResult> Index()
        {
            // Get the list of genres from the database
            ViewBag.Genres = await _context.Genre.ToListAsync();
            return View();
        }

        // This action handles the POST request when the form is submitted to add a new video
        [HttpPost]
        public async Task<IActionResult> Index(Video video, IFormFile poster)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (poster != null && poster.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await poster.CopyToAsync(memoryStream);
                        video.Poster = memoryStream.ToArray();  // Store the image as a byte array
                    }

                    _context.Video.Add(video);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("VideoDetails");  // Redirect to a page where videos are displayed
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while uploading the poster: " + ex.Message);
                }
            }

            // Re-populate genres if validation failed
            ViewBag.Genres = await _context.Genre.ToListAsync();
            return View(video);
        }


        // This action handles the "Video Details" page
        public async Task<IActionResult> VideoDetails(string searchString, int? genreId, string director)
        {
            // Start by getting the base video list with genre information
            var videos = _context.Video.Include(v => v.Genre).AsQueryable();

            // Apply filters if there are search criteria
            if (!string.IsNullOrEmpty(searchString))
            {
                videos = videos.Where(v => v.Title.Contains(searchString));
            }

            if (genreId.HasValue)
            {
                videos = videos.Where(v => v.GenreID == genreId);
            }

            if (!string.IsNullOrEmpty(director))
            {
                videos = videos.Where(v => v.Director.Contains(director));
            }

            // Populate genres for the dropdown in the view
            ViewBag.Genres = await _context.Genre.ToListAsync();

            // Return the filtered list of videos to the view
            return View(await videos.ToListAsync());
        }


        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Search for Video by ID
        [HttpGet]
        public async Task<IActionResult> Search(int? id)
        {
            if (id == null)
            {
                return View();  // Return the search form if no ID is provided
            }

            // Try to find the video by ID
            var video = await _context.Video
                .Include(v => v.Genre)  // Include Genre if needed for the dropdown
                .FirstOrDefaultAsync(v => v.VideoID == id);

            if (video == null)
            {
                ViewBag.ErrorMessage = "Video not found";  // Show error if video is not found
                return View();  // Return to the search form
            }

            // Provide genres for the dropdown list (if editing is allowed)
            ViewBag.Genres = await _context.Genre.ToListAsync();
            return View("Edit", video);  // Return the video model to the Edit form
        }

        // GET: Edit Video by ID
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var video = await _context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            // Pass genres to the view for dropdown list
            ViewBag.Genres = await _context.Genre.ToListAsync();
            return View(video);
        }

        // POST: Edit Video details - Handles updating the video
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Video video, IFormFile poster)
        {
            if (id != video.VideoID)
            {
                return NotFound();  // Ensure the ID matches
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Handle the poster image upload if a new one is selected
                    if (poster != null && poster.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await poster.CopyToAsync(memoryStream);
                            video.Poster = memoryStream.ToArray();  // Store the image as a byte array
                        }
                    }
                    else
                    {
                        // Keep the existing poster image if no new file is uploaded
                        var existingVideo = await _context.Video.AsNoTracking().FirstOrDefaultAsync(v => v.VideoID == id);
                        if (existingVideo != null)
                        {
                            video.Poster = existingVideo.Poster;
                        }
                    }

                    // Update the video in the database
                    _context.Update(video);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(VideoDetails));  // Redirect to the video details page
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Video.Any(v => v.VideoID == video.VideoID))
                {
                    return NotFound();  // Return 404 if the video does not exist
                }
                else
                {
                    throw;  // Re-throw the exception if something else went wrong
                }
            }

            // Re-populate genres if validation failed
            ViewBag.Genres = await _context.Genre.ToListAsync();
            return View(video);  // Return with validation errors if any
        }

        // DELETE: Delete Video by ID
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var video = await _context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();  // Return 404 if the video is not found
            }

            _context.Video.Remove(video);  // Remove the video from the database
            await _context.SaveChangesAsync();  // Save the changes
            return RedirectToAction(nameof(VideoDetails));  // Redirect to the video details page
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}




