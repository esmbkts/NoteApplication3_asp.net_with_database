namespace asp.net.Models
{
    public interface INoteAction
    {
        void AddNote(Note note);
        List<Note> GetNoteList(int accountId); // Dönüş türünü List<Note> olarak değiştirdik
        void UpdateNote(Note note);
        void DeleteNote(int id, int accountId);
    }
}
