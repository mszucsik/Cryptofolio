/*
 *  Cryptofolio
 *  Version 1.0 (November 30, 2018)
 *  by Michael Szucsik
 *  
 *  I, Michael Szucsik, 000286230, certify that this is my original work.
 *  No other persons work was used without due acknowledgement.
 *  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cryptofolio.Data;
using Cryptofolio.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cryptofolio.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OwnerComments
        /// <summary>
        /// User: This method gets all of a users self made comments
        /// Admin: This method gets all comments ordered by date
        /// </summary>
        /// <returns>A list of Comments</returns>
        /// 
        [Authorize]
        public async Task<IActionResult> OwnerComments()
        {
            List<Comment> comments = await _context.Comment.ToListAsync();
            List<Comment> displayComments = new List<Comment>();
            foreach (Comment comment in comments)
            {
                if ((User.Identity.Name == comment.OwnerID) || User.IsInRole("Admin"))
                {
                    Portfolio portfolio = await _context.Portfolio.FirstOrDefaultAsync(o => o.ID == comment.Portfolio_ID);
                    if (portfolio != null)
                    {
                        comment.Portfolio_Name = portfolio.Name;
                        displayComments.Add(comment);
                    }
                }
            }

            return View(displayComments);
        }

        // GET: PortfolioComments
        /// <summary>
        /// User: This method gets all of comments posted to their own portfolio
        /// Admin: This method gets all comments ordered by portfolio
        /// </summary>
        /// <returns>A list of Comments</returns>
        /// 
        [Authorize]
        public async Task<IActionResult> PortfolioComments()
        {

            List<Portfolio> portfolios = await _context.Portfolio.ToListAsync();
            List<Comment> comments = await _context.Comment.ToListAsync();
            List<Comment> displayComments = new List<Comment>();
            foreach (Portfolio portfolio in portfolios)
            {
                if (User.Identity.Name == portfolio.OwnerID)
                {
                    foreach (Comment comment in comments)
                    {
                        if (portfolio.ID == comment.Portfolio_ID)
                        {
                            comment.Portfolio_Name = portfolio.Name;
                            displayComments.Add(comment);
                        }
                    }
                }
                else if (User.IsInRole("Admin"))
                {
                    foreach (Comment comment in comments)
                    {
                        if (portfolio.ID == comment.Portfolio_ID)
                        {
                            comment.Portfolio_Name = portfolio.Name;
                            displayComments.Add(comment);
                        }
                    }
                }
            }

            return View(displayComments);
        }

        // POST: EditComment
        /// <summary>
        /// This method posts a change to a comment
        /// </summary>
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditComment(int id, [Bind("ID,Message")] Comment comment)
        {
            var editComment = await _context.Comment.FindAsync(comment.ID);
            editComment.Message = comment.Message;
            if ((User.Identity.Name == editComment.OwnerID) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(editComment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;

                    }
                }
            }
            return RedirectToAction(nameof(OwnerComments));
        }

        // POST: DeleteComment
        /// <summary>
        /// This method deletes an existing comment
        /// </summary>
        /// 
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteComment([Bind("ID,OwnerID,Portfolio_ID,Creation_Date,Message")] Comment comment)
        {
            if ((User.Identity.Name == comment.OwnerID) || User.IsInRole("Admin"))
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(OwnerComments));
        }
    }
}
