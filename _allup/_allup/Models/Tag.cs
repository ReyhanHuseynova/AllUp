﻿namespace _allup.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeactive { get; set; }    
        public List<ProductTag> ProductTags { get; set; }   
        

    }
}
