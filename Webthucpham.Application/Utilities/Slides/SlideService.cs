using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Webthucpham.Data.EF;
using Webthucpham.ViewModels.Utilities.Slide;
using Microsoft.EntityFrameworkCore;

namespace Webthucpham.Application.Utilities.Slides
{
    public class SlideService : ISlideService
    {
        private readonly WebthucphamDbContext _context;

        public SlideService(WebthucphamDbContext context)
        {
            _context = context;
        }

        public async Task<List<SlideVm>> GetAll()
        {
            var slides = await _context.Slides.OrderBy(x => x.SortOrder)
                .Select(x => new SlideVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Url = x.Url,
                    Image = x.Image
                }).ToListAsync();

            return slides;
        }

       
    }
}
