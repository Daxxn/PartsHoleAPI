namespace PartsHoleLib.Enums;

public enum ModelIDSelector
{
   PARTS = 0,
   INVOICES = 1,
   BINS = 2,
   PARTNUMBERS = 3,
   NONE = -1,
}

public enum EIAStandard
{
   E6 = 20,
   E12 = 10,
   E24 = 5,
   E48 = 2,
   E96 = 1,
   E192 = 0,
};

public enum SupplierType
{
   DigiKey,
   Mouser,
};

#region Passives
public enum PassiveSearchProp
{
   Value,
   Desc,
   Tolerance,
};

public enum PackageType
{
   SMD,
   PTH,
   UNK,
};

public enum PassiveType
{
   Resistor = 0,
   Capacitor = -12,
   Inductor = -3,
};
#endregion

#region Part Number
public enum PartNumberType
{
   Other,

   Passives,
   Protection,
   Mechanical,
   Connector,
   Lighting,
   Diode,
   Transistor,
   IC,
   Display,
   ElectroMechanical,
   Switch_Input,
};

public enum PartNumberSubTypes
{
   Other = 0,
   Resistor = 0101,
   capacitor = 0102,
   Inductor = 0103,
   Ferrites = 0104,
   Crystal = 0105,
   Resonator = 0106,
   Fuse = 0201,
   CircuitBreaker = 0202,
   Varistor = 0203,
   PTCFuse = 0204,
   Screw = 0301,
   Nut = 0302,
   Standoff = 0303,
   PinHeader = 0401,
   PinSocket = 0402,
   TerminalBlock = 0403,
   DSub = 0404,
   Circular = 0405,
   FlatFlex = 0406,
   Audio = 0407,
   USB = 08,
   BarrelJack = 0409,
   MOLEX = 0410,
   Programming = 0411,
   PCIe = 0412,
   LED = 0501,
   Fillament = 0502,
   Fluorescent = 0503,
   GeneralPurpose = 0601,
   Schottky = 0602,
   Zener = 0603,
   TVS = 0604,
   NPN = 0701,
   PNP = 0702,
   Nch = 0703,
   Pch = 0704,
   MicroController = 0801,
   Logic = 0802,
   OPAMP = 0803,
   LinearReg = 0804,
   SwitchingReg = 0805,
   Interface = 0806,
   AnalogLogic = 0807,
   ADC = 0809,
   DAC = 0810,
   ROM = 0811,
   EEPROM = 0812,
   Memory = 0813,
   Processor = 0814,
   Sensing = 0815,
   CurrentSense = 0816,
   LCDCharacter = 0901,
   LCDPanel = 0902,
   SevenSegment = 0903,
   BarGraph = 0904,
   OLED = 0905,
   Relay = 1001,
   Contactor = 1002,
   Mic = 1003,
   Speaker = 1004,
   Buzzer = 1005,
   Motor = 1006,
   Tactile = 1101,
   Toggle = 1102,
   DIP = 1103,
   Limit = 1104,
   Rotary = 1105,
   Slide = 1106,
   Rocker = 1107,
   RotaryEncoder = 1108,
   Potentiometer = 1109,
   Keypad = 1110,
   Keylock = 1111,
   Navigation = 1112,
};

public enum PartType
{
   Unknown = 0,

   Resistor,
   Capacitor,
   Inductor,
   IC,
   Arduino,
   Battery,
   CapacitorNetwork,
   ResistorNetwork,
   Diode,
   DiodeNetwork,
   Display,
   Fuse,
   FerriteBead,
   Fiducial,
   Filter,
   InfraredDiode,
   Connector,
   Jumper,
   Relay,
   Speaker,
   Motor,
   Microphone,
   OptoIsolator,
   PowerSupply,
   Transistor,
   Thermistor,
   Varistor,
   Switch,
   Transformer,
   Thermocouple,
   TestPoint,
   Tuner,
   VacuumTube,
   VoltageRegulator,
   Potentiometer,
   Crystal,
   Oscillator,
   BridgeRectifier,
   Attenuator,
   DelayLine,
   Hardware,
   DirectionalCoupler,
   Socket,

   USBConnector,
   SenseResistor,
   RotaryEncoder,
}
#endregion
