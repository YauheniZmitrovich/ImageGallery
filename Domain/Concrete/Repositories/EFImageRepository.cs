using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Entities;

namespace Domain.Concrete.Repositories
{
    public class EFImageRepository : IImageRepository
    {
        private readonly EFDbContext _context;

        public EFImageRepository(EFDbContext context)
        {
            _context = context;
        }

        public IQueryable<Image> Images => _context.Images;

        public void Save(Image image)
        {
            if (image.Id == 0)
            {
                _context.Images.Add(image);
            }
            else
            {
                Image dbEntry = _context.Images.Find(image.Id);

                if (dbEntry != null)
                {
                    dbEntry.Id = image.Id;
                    dbEntry.Data = image.Data;
                    dbEntry.CompressedData = image.CompressedData;
                    dbEntry.MimeType = image.MimeType;
                }
            }

            _context.SaveChanges();
        }

        public Image GetImageById(long id) => (from u in _context.Images
            where u.Id == id
            select u).FirstOrDefault();
    }
}
