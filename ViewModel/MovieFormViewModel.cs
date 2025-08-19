using Movies.Models;
using System.ComponentModel.DataAnnotations;

namespace Movies.ViewModel
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }

        [Required,StringLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }
        [Range(1,10)]
        public double Rate { get; set; }
        [Required,StringLength(2500)]
        public string StoreLine { get; set; }
        
        public byte[] Poster { get; set; }
        [Display(Name ="Genre")]
        public byte GenreId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public Genre Genre { get; set; }

    }
}
