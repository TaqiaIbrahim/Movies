using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Models;
using Movies.ViewModel;

namespace Movies.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var movies=_context.Movies.OrderBy(m=>m.Rate).ToList();
            return View(movies);
        }
        public IActionResult Create()
        {
            var viewModel = new MovieFormViewModel
            {
                Genres = _context.Genres.OrderBy(m => m.Name).ToList()
            };
            return View( "MovieForm",viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create(MovieFormViewModel model)
        {
           // var viewModel = new MovieFormViewModel
           if(ModelState.IsValid)
            {
               model.Genres = _context.Genres.OrderBy(m => m.Name).ToList();
                return View("MovieForm", model);
            }
            var files = Request.Form.Files;
            if(!files.Any())
            {
                model.Genres=_context.Genres.OrderBy(m => m.Name).ToList();
                ModelState.AddModelError("Poster", "Please select movie poster...");
            }
            var poster = files.FirstOrDefault();
            var allowedExtenstions= new List<string> {".png" };
            if (!allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = _context.Genres.OrderBy(m => m.Name).ToList();
                ModelState.AddModelError("Poster", "Only GPJ and PNG are allowed...");
                return View("MovieForm", model);
            }
            if (poster.Length>1048576)
            {
                model.Genres = _context.Genres.OrderBy(m => m.Name).ToList();
                ModelState.AddModelError("Poster", "Only GPJ and PNG are allowed...");
                return View("MovieForm", model);
            }
            using var datastream = new MemoryStream();
             poster.CopyTo(datastream);
            var movies = new Movie
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Rate = model.Rate,
                Year = model.Year,
                StoryLine = model.StoreLine,
                Poster = datastream.ToArray(),

            };
            _context.Movies.Add(movies);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int ? id)
        {
            if (id == null)
                return BadRequest();
            var movie = await _context.Movies.FindAsync(id);
            if(movie == null)
                return NotFound();
            var viewModel = new MovieFormViewModel
            {
                Id=movie.Id,
                Title= movie.Title,
                GenreId=movie.GenreId,
                Rate=movie.Rate,
                Year = movie.Year,
                Poster=movie.Poster,
                StoreLine=movie.StoryLine,
                Genres=_context.Genres.OrderBy(m=>m.Name).ToList()


            };
            return View("MovieForm",viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Edit(MovieFormViewModel model)
        {
           
            if (!ModelState.IsValid)
            {
                model.Genres =  await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            var movie = await _context.Movies.FindAsync(model.Id);
            if (movie == null)
                return NotFound();
            movie.Title = model.Title;
                movie.GenreId = model.GenreId;
                movie.Rate = model.Rate;
                movie.StoryLine = model.StoreLine;
                movie.Year= model.Year;
          
            _context.Movies.Update(movie);
           await _context.SaveChangesAsync();
           
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Details( int ?id)
        {

            if (id == null)
                return BadRequest();
            var movie =  _context.Movies.Include(m=>m.Genre).SingleOrDefault(m=>m.Id==id);
            if (movie == null)
                return NotFound();
            var viewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                GenreId = movie.GenreId,
                Rate = movie.Rate,
                Year = movie.Year,
                Poster = movie.Poster,
                StoreLine = movie.StoryLine,
                Genres = _context.Genres.OrderBy(m => m.Name).ToList()


            };
            return View( viewModel);
      

            
        }
    }
}
