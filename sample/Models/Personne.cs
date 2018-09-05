using Nest;

namespace sample.Models
{
    [ElasticsearchType(Name = "personne", IdProperty = "ID")]
    public class Personne
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LastName  { get; set; }

        public override string ToString(){
            return $"ID : {ID} / Name : {Name} / LastName : {LastName}";
        }
    }
}