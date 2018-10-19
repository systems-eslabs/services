using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Repository;
using EmailService.DbEntities;

namespace EmailService
{
    public class  EMailRepository<T> : Repository<T> where T : class
    { 
        public EMailRepository():base(new email_botContext())
        {
        }
    }
}
