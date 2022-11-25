﻿using PartsHoleLib.Enums;
using PartsHoleLib.Interfaces;

namespace PartsHoleLib;

public class PartNumber : IComparable<PartNumber>, IModel
{
   #region Local Props
   public string _id { get; set; } = null!;
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
    public uint ID { get; set; }
    #endregion

    #region Constructors
    public PartNumber() { }
    public PartNumber(uint category, uint subCategory)
    {
        Category = category;
        SubCategory = subCategory;
    }
    public PartNumber(uint category, uint subCategory, uint id)
    {
        Category = category;
        SubCategory = subCategory;
        ID = id;
    }
    #endregion

    #region Methods
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
                newModel.ID = id;
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

    private double CalcValue() => (Category * Math.Pow(10, 6)) + (SubCategory * Math.Pow(10, 4)) + ID;
    public override string ToString() => $"{Category:D2}{SubCategory:D2}-{ID:D4}";
    #endregion

    #region Full Props
    public uint FullCategory => (uint)(Category * Math.Pow(10, 2)) + SubCategory;
    #endregion
}