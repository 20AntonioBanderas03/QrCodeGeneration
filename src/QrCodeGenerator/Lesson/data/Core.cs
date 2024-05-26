namespace Lesson.data
{
    public class Core
    {
        private static LessonDatabaseEntities _context = new LessonDatabaseEntities();

        public static LessonDatabaseEntities GetContext() { 
            if(_context == null)
                _context = new LessonDatabaseEntities();

            return _context;
        }
    }
}
