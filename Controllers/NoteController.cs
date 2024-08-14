using Microsoft.AspNetCore.Mvc;
using asp.net.Models;
using asp.net.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace asp.net.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly INoteAction _noteAction;
        private readonly ApplicationDbContext _context;

        public NoteController(INoteAction noteAction, ApplicationDbContext context)
        {
            _noteAction = noteAction;
            _context = context;
        }

        public IActionResult AddNote()
        {
            return View(new NoteViewModel());
        }

        [HttpPost]
        public IActionResult AddNote(NoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var accountIdString = User.FindFirstValue("AccountId");
                if (int.TryParse(accountIdString, out int accountId))
                {
                    var note = new Note
                    {
                        AccountId = accountId,
                        Description = model.Content,
                        CreateTime = DateTime.Now,
                        LastUpdateTime = DateTime.Now
                    };

                    _noteAction.AddNote(note);
                    return RedirectToAction("ListNotes");
                }
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult ListNotes()
        {
            var accountIdString = User.FindFirstValue("AccountId");
            if (int.TryParse(accountIdString, out int accountId))
            {
                var notes = _noteAction.GetNoteList(accountId);
                return View(notes);
            }
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult UpdateNote(int id)
        {
            var accountIdString = User.FindFirstValue("AccountId");
            if (int.TryParse(accountIdString, out int accountId))
            {
                var note = _noteAction.GetNoteList(accountId).FirstOrDefault(n => n.Id == id);
                if (note == null)
                {
                    return RedirectToAction("ListNotes");
                }

                var model = new NoteViewModel
                {
                    Id = note.Id,
                    Content = note.Description
                };

                return View(model);
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public IActionResult UpdateNote(NoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var accountIdString = User.FindFirstValue("AccountId");
                if (int.TryParse(accountIdString, out int accountId))
                {
                    var note = new Note
                    {
                        Id = model.Id,
                        AccountId = accountId,
                        Description = model.Content,
                        LastUpdateTime = DateTime.Now
                    };
                    _noteAction.UpdateNote(note);
                    return RedirectToAction("ListNotes");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteNote(int id)
        {
            var accountIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(accountIdString, out int accountId))
            {
                var note = _noteAction.GetNoteList(accountId).FirstOrDefault(n => n.Id == id);
                if (note == null)
                {
                    return RedirectToAction("ListNotes");
                }
                return View("ConfirmDelete", note);
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var accountIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(accountIdString, out int accountId))
            {
                _noteAction.DeleteNote(id, accountId);
                return RedirectToAction("ListNotes");
            }
            return RedirectToAction("Login", "User");
        }
    }
}