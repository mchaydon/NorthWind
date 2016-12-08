using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Northwind.Models;
using Northwind.Security;


namespace Northwind.Controllers
{
    public class CartController : Controller
    {
        // POST: Cart/AddToCart
        [HttpPost]
        public JsonResult AddToCart(CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
            return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            // create cart item from Json object
            Cart sc = new Cart();
            sc.ProductID = cartDTO.ProductID;
            sc.CustomerID = cartDTO.CustomerID;
            sc.Quantity = cartDTO.Quantity;
            using (NorthwindEntities db = new NorthwindEntities())
            {
                // if there is a duplicate product id in cart, simply update the quantity
                if (db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID ==
               sc.CustomerID).Any())
                {
                    // this product is already in the customer's cart,
                    // update the existing cart item's quantity
                    Cart cart = db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID ==
                    sc.CustomerID).FirstOrDefault();
                    cart.Quantity += sc.Quantity;
                    sc = new Cart()
                    {
                        CartID = cart.CartID,
                        ProductID = cart.ProductID,
                        CustomerID = cart.CustomerID,
                        Quantity = cart.Quantity
                    };
                }
                else
                {
                    // this product is not in the customer's cart, add the product
                    db.Carts.Add(sc);
                }
                db.SaveChanges();
            }
            return Json(sc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCartQuantity(CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            // create cart item from Json object
            Cart sc = new Cart();
            sc.ProductID = cartDTO.ProductID;
            sc.CustomerID = cartDTO.CustomerID;
            sc.Quantity = cartDTO.Quantity;
            using (NorthwindEntities db = new NorthwindEntities())
            {
                // this product is already in the customer's cart,
                // update the existing cart item's quantity
                Cart cart = db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID ==
                sc.CustomerID).FirstOrDefault();
                cart.Quantity = sc.Quantity;
                sc = new Cart()
                {
                    CartID = cart.CartID,
                    ProductID = cart.ProductID,
                    CustomerID = cart.CustomerID,
                    Quantity = cart.Quantity
                };
                db.SaveChanges();
            }
            return Json(sc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFromCart(CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            // create cart item from Json object
            Cart sc = new Cart();
            sc.ProductID = cartDTO.ProductID;
            sc.CustomerID = cartDTO.CustomerID;
            using (NorthwindEntities db = new NorthwindEntities())
            {
                // this product is already in the customer's cart,
                // update the existing cart item's quantity
                Cart cart = db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID ==
                sc.CustomerID).FirstOrDefault();
                db.Carts.Remove(cart);
                db.SaveChanges();
            }
            return Json(sc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cart()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                Customer customer = db.Customers.Find(UserAccount.GetUserID());
                var getCart = db.Carts.Include("Product").Include("Customer").Where(c => c.CustomerID == customer.CustomerID).ToList();

                return View(getCart);

            }
        }
    }
}