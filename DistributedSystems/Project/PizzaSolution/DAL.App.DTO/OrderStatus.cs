﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ee.itcollege.rotoni.pizzaApp.DAL.Base;


namespace DAL.App.DTO
{
    public class OrderStatus : DomainEntityIdMetadata
    {
        [MaxLength(256, ErrorMessageResourceName = "ErrorMessage_MaxLength", ErrorMessageResourceType = typeof(Resources.Common))] [MinLength(1, ErrorMessageResourceName = "ErrorMessage_MinLength", ErrorMessageResourceType = typeof(Resources.Common))] public string Status { get; set; } = default!;

        public ICollection<Invoice>? Invoices { get; set; }
    }
}