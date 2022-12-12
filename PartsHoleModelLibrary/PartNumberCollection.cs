using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsHoleLib;

public class PartNumberCollection : IList<PartNumber>
{
   #region Local Props
   private List<PartNumber> _parts;
   public PartNumber this[int index] { get => _parts[index]; set => _parts[index] = value; }
   public int Count => _parts.Count;
   public bool IsReadOnly { get; } = false;
   #endregion

   #region Constructors
   public PartNumberCollection() => _parts = new();
   public PartNumberCollection(IEnumerable<PartNumber>? newPartNumbers) =>
      _parts = newPartNumbers is null ? new() : newPartNumbers.ToList();
   #endregion

   #region Methods
   #region Test

   #endregion
   public static uint MergeCategory(uint category, uint subCategory) =>
      (uint)(category * Math.Pow(10, 2)) + subCategory;

   public PartNumber New(uint fullCategory)
   {
      var newPartNumber = PartNumber.CreateTemp(fullCategory);
      _parts.Sort();
      var matchingParts = FindCategory(newPartNumber);
      var maxID = matchingParts.Any() ? matchingParts.Max(p => p.PartID) : 0;
      maxID++;
      newPartNumber.PartID = maxID;
      Add(newPartNumber);
      return newPartNumber;
   }

   public IEnumerable<PartNumber> FindCategory(uint category, uint subCategory)
   {
      var cat = MergeCategory(category, subCategory);
      return _parts.Where((p) => p.FullCategory == cat);
   }

   public IEnumerable<PartNumber> FindCategory(PartNumber pn) =>
      _parts.Where((p) => p.FullCategory == pn.FullCategory);

   public void Add(PartNumber item)
   {
      if (Contains(item))
      {
         throw new ArgumentException("Part number already exists.", nameof(item));
      }
      _parts.Add(item);
   }

   #region Unchanged IList Methods
   public void Clear() => _parts.Clear();
   public bool Contains(PartNumber item) => _parts.Contains(item);
   public void CopyTo(PartNumber[] array, int arrayIndex) => _parts.CopyTo(array, arrayIndex);
   public IEnumerator<PartNumber> GetEnumerator() => _parts.GetEnumerator();
   public int IndexOf(PartNumber item) => _parts.IndexOf(item);
   public void Insert(int index, PartNumber item) => _parts.Insert(index, item);
   public bool Remove(PartNumber item) => _parts.Remove(item);
   public void RemoveAt(int index) => _parts.RemoveAt(index);
   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   #endregion

   public override string ToString() => $"SupplierPartNumber Collection {_parts.Count}";
   #endregion

   #region Full Props

   #endregion
}
