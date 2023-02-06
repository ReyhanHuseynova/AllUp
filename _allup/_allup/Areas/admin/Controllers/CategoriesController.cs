using _allup.DAL;
using _allup.Helpers;
using _allup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;

namespace _allup.Areas.admin.Controllers
{
    [Area("admin")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CategoriesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _db.Categories.Include(x=>x.Children).Include(x => x.Parent).ToListAsync();
            return View(categories);
        }

        #region Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.Where(x=>x.IsMain).ToListAsync();
           return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, int mainCatId)
        {
            ViewBag.Categories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
         

            if (category.IsMain)
            {
                if (category.Photo == null)
                {
                    ModelState.AddModelError("Photo", "Shekil sech");
                    return View();
                }
                if (!category.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Shekil formati sech");
                    return View();
                }
                if (category.Photo.IsOlder2Mb())
                {
                    ModelState.AddModelError("Photo", "Max 2Mb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "images");
                category.Image = await category.Photo.SaveImageAsync(folder);
            }
            else
            {
                category.ParentId = mainCatId;
            }
             //category.ParentId= mainCatId;

            
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Categories = await _db.Categories.Where(x => x.IsMain).ToListAsync();

            if (id == null)
            {
                return NotFound();
            }
            Category dbCategory = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (dbCategory == null)
            {
                return BadRequest();
            }
           


            return View(dbCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Category category, int mainCatId)
        {
            ViewBag.Categories = await _db.Categories.Where(x => x.IsMain).ToListAsync();

            if (id == null)
            {
                return NotFound();
            }
            Category dbCategory = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (dbCategory == null)
            {
                return BadRequest();
            }

          
            if (category.IsMain)
            {
                if (category.Photo != null)
                {
                    if (!category.Photo.IsImage())
                    {
                        ModelState.AddModelError("Photo", "Shekil formati sech");
                        return View(dbCategory);
                    }
                    if (category.Photo.IsOlder2Mb())
                    {
                        ModelState.AddModelError("Photo", "Max 2Mb");
                        return View(dbCategory);
                    }
                    string folder = Path.Combine(_env.WebRootPath, "assets", "images");
                   
                    dbCategory.Image = await category.Photo.SaveImageAsync(folder);
                }
               
            }

            else
            {
                dbCategory.ParentId = mainCatId;
            }
             dbCategory.ParentId = category.ParentId;
            
             dbCategory.Name = category.Name;
            dbCategory.Image = category.Image;
          

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
            
        }
        #endregion

        #region Activity
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category dbCategory = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (dbCategory == null)
            {
                return BadRequest();
            }
            if (dbCategory.IsDeactive == true)
            {
                dbCategory.IsDeactive = false;
            }
            else
            {
                dbCategory.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category category = await _db.Categories.Include(x=>x.Children).Include(x => x.Parent).FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return BadRequest();
            }
        

            return View(category);
        }
        #endregion
    }
}
