﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Identity;
using ee.itcollege.rotoni.pizzaApp.DAL.Base;

namespace Domain
{    
    public class Cart : DomainEntityIdMetadataUser<AppUser>
    {
        [NotMapped]
        [Display(Name = nameof(Total), ResourceType = typeof(Resources.Domain.Shared))]
        public decimal? Total { get; set; }

        public bool Active { get; set; } = true;

        public ICollection<PizzaInCart>? PizzaInCarts { get; set; }
        
        public ICollection<DrinkInCart>? DrinkInCarts { get; set; }

    }
}