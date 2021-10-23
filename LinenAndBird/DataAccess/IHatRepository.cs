using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinenAndBird.DataAccess
{
   public interface IHatRepository
    {
        Hat GetById(Guid hatId);
        IEnumerable<Hat> GetByStyle(HatStyle style);
        IEnumerable<Hat> GetAll();
        void Add(Hat newHat);
        void Remove(Guid id);
        Hat Update(Guid id, Hat hat);

    }
}
