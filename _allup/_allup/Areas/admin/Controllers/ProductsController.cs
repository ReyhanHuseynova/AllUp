using _allup.DAL;
using _allup.Helpers;
using _allup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _allup.Areas.admin.Controllers
{
    [Area("admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _db.Products.Include(x => x.ProductDetails).
                Include(x => x.Brand).Include(x => x.ProductImages).
                Include(x => x.ProductCategories).
                ThenInclude(x => x.Category).Include(x => x.ProductTags).
                 ThenInclude(x => x.Tag).ToListAsync();

            return View(products);
        }

        #region Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _db.Brands.ToListAsync();
            ViewBag.Tags = await _db.Tags.ToListAsync();
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            Category? firstMainCategory = await _db.Categories.Include(x => x.Children).FirstOrDefaultAsync(x => x.IsMain);
            ViewBag.ChildCategories = firstMainCategory.Children;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int brandId, int[] tagsId, int mainCatId, int? childCatId)
        {
            #region Get
            ViewBag.Brands = await _db.Brands.ToListAsync();
            ViewBag.Tags = await _db.Tags.ToListAsync();
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            Category? firstMainCategory = await _db.Categories.Include(x => x.Children).FirstOrDefaultAsync(x => x.IsMain);
            ViewBag.ChildCategories = firstMainCategory.Children;
            #endregion

            #region isExist
            bool isExist = await _db.Products.AnyAsync(x => x.Name == product.Name);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This product already is exist");
                return View();
            }
            #endregion

            #region Image
            if (product.Photos == null)
            {
                ModelState.AddModelError("Photos", "Sekil formati secin ");
            }
            List<ProductImage> productImages = new List<ProductImage>();

            foreach (IFormFile Photo in product.Photos)
            {
                if (!Photo.IsImage())
                {
                    ModelState.AddModelError("Photos", "Shekil formati sech");
                    return View();
                }
                if (Photo.IsOlder2Mb())
                {
                    ModelState.AddModelError("Photos", "Max 2Mb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "images", "product");


                ProductImage productImage = new ProductImage
                {
                    Image = await Photo.SaveImageAsync(folder),

                };
                productImages.Add(productImage);
            }
            product.ProductImages = productImages;
            #endregion

            #region Tags
            List<ProductTag> productTags = new List<ProductTag>();

            foreach (int tagId in tagsId)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = tagId,

                };
                productTags.Add(productTag);

            }

            #endregion

            #region ProductCategory
            List<ProductCategory> productCategories = new List<ProductCategory>();

            ProductCategory mainProductCategory = new ProductCategory
            {
                CategoryId = mainCatId
            };
            productCategories.Add(mainProductCategory);

            if (childCatId != null)
            {
                ProductCategory childProductCategory = new ProductCategory
                {
                    CategoryId = (int)childCatId
                };
                productCategories.Add(childProductCategory);
            }
            product.ProductCategories = productCategories;
            #endregion

            product.BrandId = brandId;
            product.ProductTags = productTags;
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion


        public async Task<IActionResult> LoadChildCategories(int mainCatId)
        {
            List<Category> childCategories = await _db.Categories.Where(x => x.ParentId == mainCatId).ToListAsync();
            return PartialView("_LoadChildCategoriesPartial", childCategories);
        }

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Brands = await _db.Brands.ToListAsync();
            ViewBag.Tags = await _db.Tags.ToListAsync();
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();


            if (id == null)
            {
                return NotFound();
            }

            Product? dbProduct = await _db.Products.Include(x => x.ProductTags).
            Include(x => x.ProductTags).Include(x => x.ProductDetails).Include(x => x.ProductCategories).
             ThenInclude(x => x.Category).ThenInclude(x => x.Children).
            Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if (dbProduct == null)
            {
                return BadRequest();
            }
            return View(dbProduct);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product, int brandId, int[] tagsId, int mainCatId, int? childCatId)
        {
            #region Get

            ViewBag.Brands = await _db.Brands.ToListAsync();
            ViewBag.Tags = await _db.Tags.ToListAsync();
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();


            if (id == null)
            {
                return NotFound();
            }

            Product? dbProduct = await _db.Products.Include(x => x.ProductTags).
            Include(x => x.ProductTags).Include(x => x.ProductDetails).Include(x => x.ProductCategories).
            ThenInclude(x => x.Category).ThenInclude(x => x.Children).
            Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if (dbProduct == null)
            {
                return BadRequest();
            }
            #endregion

            #region isExist
            bool isExist = await _db.Products.AnyAsync(x => x.Name == product.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This product already is exist");
                return View();
            }
            #endregion

            #region Image
            if (product.Photos != null)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile Photo in product.Photos)
                {
                    if (!Photo.IsImage())
                    {
                        ModelState.AddModelError("Photos", "Shekil formati sech");
                        return View();
                    }
                    if (Photo.IsOlder2Mb())
                    {
                        ModelState.AddModelError("Photos", "Max 2Mb");
                        return View();
                    }
                    string folder = Path.Combine(_env.WebRootPath, "assets", "images", "product");


                    ProductImage productImage = new ProductImage
                    {
                        Image = await Photo.SaveImageAsync(folder),

                    };
                    productImages.Add(productImage);
                }
                dbProduct.ProductImages.AddRange(productImages);
            }
            #endregion


            #region Tags
            List<ProductTag> productTags = new List<ProductTag>();

            foreach (int tagId in tagsId)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = tagId,

                };
                productTags.Add(productTag);

            }
            dbProduct.ProductTags = productTags;

            #endregion

            #region ProductCategory
            List<ProductCategory> productCategories = new List<ProductCategory>();

            ProductCategory mainProductCategory = new ProductCategory
            {
                CategoryId = mainCatId
            };
            productCategories.Add(mainProductCategory);

            if (childCatId != null)
            {
                ProductCategory childProductCategory = new ProductCategory
                {
                    CategoryId = (int)childCatId
                };
                productCategories.Add(childProductCategory);
            }
            dbProduct.ProductCategories = productCategories;
            #endregion

            dbProduct.BrandId = brandId;
            dbProduct.Name = product.Name;
            dbProduct.Price = product.Price;
            dbProduct.ProductDetails.Tax = product.ProductDetails.Tax;
            dbProduct.Rate = product.Rate;
            dbProduct.ProductDetails.Description = product.ProductDetails.Description;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        public IActionResult DeleteImage(int proImageId)
        {
            ProductImage productImage = _db.ProductImages.FirstOrDefault(x => x.Id == proImageId);
            int count = _db.ProductImages.Where(x => x.ProductId == productImage.ProductId).Count();

            if (count == 1)
            {
                return Ok();
            }

            _db.ProductImages.Remove(productImage);
            _db.SaveChanges();


            return Ok(count - 1);
        }


        #region Activity
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product? dbProduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (dbProduct == null)
            {
                return BadRequest();
            }
            if (dbProduct.IsDeactive == true)
            {
                dbProduct.IsDeactive = false;
            }
            else
            {
                dbProduct.IsDeactive = true;
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
            Product? product = await _db.Products.Include(x =>x.Brand).Include(x => x.ProductTags).
                ThenInclude(x=>x.Tag).Include(x => x.ProductDetails).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return BadRequest();
            }


            return View(product);
           
        }

        #endregion


    }

}