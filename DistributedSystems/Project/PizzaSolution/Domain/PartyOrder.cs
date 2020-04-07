﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Base;
using Domain.Identity;

namespace Domain
{
    public class PartyOrder : DomainEntity
    {
        [Display(Name = nameof(Start), ResourceType = typeof(Resources.Domain.Shared))]
        public DateTime Start { get; set; } = DateTime.Now;
        [Display(Name = nameof(End), ResourceType = typeof(Resources.Domain.Shared))]
        public DateTime? End { get; set; }

        [Display(Name = nameof(Total), ResourceType = typeof(Resources.Domain.Shared))]
        [NotMapped] public decimal Total { get; set; }

        [Display(Name = nameof(Address), ResourceType = typeof(Resources.Domain.Shared))]
        [MaxLength(256)] [MinLength(1)] public string Address { get; set; } = default!;

        [Display(Name = nameof(InviteKey), ResourceType = typeof(Resources.Domain.Shared))]
        [MaxLength(8)] [MinLength(6)] public string InviteKey { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Owner))] public Guid OwnerId { get; set; } = default!;
        public AppUser? Owner { get; set; }

        public ICollection<PartyOrderInvoice>? PartyOrderInvoices { get; set; }
    }
}