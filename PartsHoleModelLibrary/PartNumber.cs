namespace PartsHoleModelLibrary;

public class PartNumber : IComparable<PartNumber>
{
   #region Local Props
   public uint TypeNum { get; set; }
   public uint SubType { get; set; }
   public uint ID { get; set; }
   #endregion

   #region Constructors
   public PartNumber() { }
   #endregion

   #region Methods
   public static PartNumber Next(uint typeNum, uint subtype)
   {

   }

   public static PartNumber? Parse(string input)
   {
      if (string.IsNullOrEmpty(input))
         return null;

      PartNumber newModel = new();

      var spl = input.Split('-');

      if (spl.Length == 2)
      {
         if (uint.TryParse(spl[0], out uint typeNum))
         {
            newModel.TypeNum = typeNum;
         }
         if (uint.TryParse(spl[1], out uint id))
         {
            newModel.ID = id;
         }
      }
      return newModel;
   }

   public int CompareTo(PartNumber? other)
   {
      if (other is null) return -1;
      var value = other.CalcValue() - CalcValue();
      return value > 0 ? 1 : value == 0 ? 0 : -1;
   }

   private double CalcValue() => (TypeNum * Math.Pow(10, 6)) + (SubType * Math.Pow(10, 4)) + ID;
   #endregion

   #region Full Props

   #endregion
}
