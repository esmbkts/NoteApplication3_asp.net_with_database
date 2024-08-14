using System;
using System.Collections.Generic;
using System.Linq;
using asp.net.Controllers;
using asp.net.Data;
using Microsoft.EntityFrameworkCore;

namespace asp.net.Models
{
    public class NoteAction : INoteAction
    {
        private readonly ApplicationDbContext _context;

        public NoteAction(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddNote(Note note)
        {
            note.CreateTime = DateTime.Now;
            note.LastUpdateTime = DateTime.Now;
            _context.Notes.Add(note);
            _context.SaveChanges();
        }

        public List<Note> GetNoteList(int accountId)
        {
            return _context.Notes
                .Where(n => n.AccountId == accountId)
                .ToList();
        }

        public void UpdateNote(Note updatedNote)
        {
            var note = _context.Notes.FirstOrDefault(n => n.Id == updatedNote.Id && n.AccountId == updatedNote.AccountId);
            if (note != null)
            {
                note.Description = updatedNote.Description;
                note.LastUpdateTime = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void DeleteNote(int id, int accountId)
        {
            var note = _context.Notes.FirstOrDefault(n => n.Id == id && n.AccountId == accountId);
            if (note != null)
            {
                _context.Notes.Remove(note);
                _context.SaveChanges();
            }
        }
    }
}
