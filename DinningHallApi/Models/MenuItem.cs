using System;

namespace DinningHallApi.Models
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PreparationTime { get; set; }
        public int Complexity { get; set; }
        public CookingApparatus? CookingApparatus { get; set; }

        public MenuItem()
        {
        }
    }
}