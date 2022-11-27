using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using PartsHoleLib.Enums;
using PartsHoleLib.Interfaces;

namespace PartsHoleLib;

public class PartNumber : IComparable<PartNumber>, IModel
{
   #region Local Props
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string _id { get; set; } = ObjectId.GenerateNewId().ToString();
   public static Dictionary<PartNumberType, PartNumberSubTypes[]> SubTypeDisplay = new()
   {
      { PartNumberType.Passives, new PartNumberSubTypes[] { PartNumberSubTypes.Resistor, PartNumberSubTypes.capacitor, PartNumberSubTypes.Inductor, PartNumberSubTypes.Ferrites, PartNumberSubTypes.Crystal, PartNumberSubTypes.Resonator } },
      { PartNumberType.Protection, new PartNumberSubTypes[] { PartNumberSubTypes.Fuse, PartNumberSubTypes.CircuitBreaker, PartNumberSubTypes.Varistor, PartNumberSubTypes.PTCFuse } },
      { PartNumberType.Mechanical, new PartNumberSubTypes[] { PartNumberSubTypes.Screw, PartNumberSubTypes.Nut, PartNumberSubTypes.Standoff } },
      { PartNumberType.Connector, new PartNumberSubTypes[] { PartNumberSubTypes.PinHeader, PartNumberSubTypes.PinSocket, PartNumberSubTypes.TerminalBlock, PartNumberSubTypes.DSub, PartNumberSubTypes.Circular, PartNumberSubTypes.FlatFlex, PartNumberSubTypes.Audio, PartNumberSubTypes.USB, PartNumberSubTypes.BarrelJack, PartNumberSubTypes.MOLEX, PartNumberSubTypes.Programming, PartNumberSubTypes.PCIe } },
      { PartNumberType.Lighting, new PartNumberSubTypes[] { PartNumberSubTypes.LED, PartNumberSubTypes.Fillament, PartNumberSubTypes.Fluorescent } },
      { PartNumberType.Diode, new PartNumberSubTypes[] { PartNumberSubTypes.GeneralPurpose, PartNumberSubTypes.Schottky, PartNumberSubTypes.Zener, PartNumberSubTypes.TVS } },
      { PartNumberType.Transistor, new PartNumberSubTypes[] { PartNumberSubTypes.NPN, PartNumberSubTypes.PNP, PartNumberSubTypes.Nch, PartNumberSubTypes.Pch } },
      { PartNumberType.IC, new PartNumberSubTypes[] {PartNumberSubTypes.MicroController, PartNumberSubTypes.Logic, PartNumberSubTypes.OPAMP, PartNumberSubTypes.LinearReg, PartNumberSubTypes.SwitchingReg, PartNumberSubTypes.Interface, PartNumberSubTypes.AnalogLogic, PartNumberSubTypes.USB, PartNumberSubTypes.ADC, PartNumberSubTypes.DAC, PartNumberSubTypes.ROM, PartNumberSubTypes.EEPROM, PartNumberSubTypes.Memory, PartNumberSubTypes.Processor, PartNumberSubTypes.Sensing, PartNumberSubTypes.CurrentSense } },
      { PartNumberType.Display, new PartNumberSubTypes[] {PartNumberSubTypes.LCDCharacter, PartNumberSubTypes.LCDPanel, PartNumberSubTypes.SevenSegment, PartNumberSubTypes.BarGraph, PartNumberSubTypes.OLED} },
      { PartNumberType.ElectroMechanical, new PartNumberSubTypes[] { PartNumberSubTypes.Relay, PartNumberSubTypes.Contactor, PartNumberSubTypes.Mic, PartNumberSubTypes.Speaker, PartNumberSubTypes.Buzzer, PartNumberSubTypes.Motor } },
      { PartNumberType.Switch_Input, new PartNumberSubTypes[] { PartNumberSubTypes.Tactile, PartNumberSubTypes.Toggle, PartNumberSubTypes.DIP, PartNumberSubTypes.Limit, PartNumberSubTypes.Rotary, PartNumberSubTypes.Slide, PartNumberSubTypes.Rocker, PartNumberSubTypes.RotaryEncoder, PartNumberSubTypes.Potentiometer, PartNumberSubTypes.Keypad, PartNumberSubTypes.Keylock, PartNumberSubTypes.Navigation } }
   };
   public uint Category { get; set; }
   public uint SubCategory { get; set; }
   public uint PartID { get; set; }
   #endregion

   #region Constructors
   public PartNumber() { }
   public PartNumber(uint category, uint subCategory)
   {
      Category = category;
      SubCategory = subCategory;
   }
   public PartNumber(uint category, uint subCategory, uint partId)
   {
      Category = category;
      SubCategory = subCategory;
      PartID = partId;
   }
   #endregion

   #region Methods
   public static PartNumber CreateTemp(uint fullCategory)
   {
      return new()
      {
         FullCategory = fullCategory,
      };
   }
   public static PartNumber? Parse(string input)
   {
      if (string.IsNullOrEmpty(input))
         return null;

      PartNumber newModel = new();

      var spl = input.Split('-');

      if (spl.Length == 2)
      {
         if (uint.TryParse(spl[0][..1], out uint categoryNum))
         {
            newModel.Category = categoryNum;
         }
         if (uint.TryParse(spl[0][2..3], out uint subCategoryNum))
         {
            newModel.SubCategory = subCategoryNum;
         }
         if (uint.TryParse(spl[1], out uint id))
         {
            newModel.PartID = id;
         }
      }
      return newModel;
   }

   public int CompareTo(PartNumber? other)
   {
      if (other is null)
         return -1;
      var value = other.CalcValue() - CalcValue();
      return value > 0 ? -1 : value == 0 ? 0 : 1;
   }

   public override bool Equals(object? obj)
   {
      if (obj is PartNumber pn)
         return Equals(pn);
      return base.Equals(obj);
   }
   public bool Equals(PartNumber? other) => other != null && CalcValue() == other.CalcValue();

   public override int GetHashCode() => base.GetHashCode();

   private double CalcValue() => (Category * Math.Pow(10, 6)) + (SubCategory * Math.Pow(10, 4)) + PartID;
   public override string ToString() => $"{FullCategory:D4}-{PartID:D4}";
   #endregion

   #region Full Props
   [BsonIgnore]
   public uint FullCategory
   {
      get => (uint)(Category * Math.Pow(10, 2)) + SubCategory;
      set
      {
         var str = $"{value:D4}";
         if (uint.TryParse(str.Substring(0, 2), out uint category))
         {
            Category = category;
         }
         if (uint.TryParse(str.Substring(2, 2), out uint subCategory))
         {
            SubCategory = subCategory;
         }
      }
   }
   #endregion
}