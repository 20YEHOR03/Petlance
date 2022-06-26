namespace Petlance
{
    interface Entity
    {
        public string InsertQuery { get; }
        public string UpdateQuery { get; }
        public int Id { get; set; }
        public void Update();
        public bool Delete();
    }
}