//namespace Bally;

//public interface IMachineLogic
//{
//    public WinComboBits GetWinCombo(Symbol Symbol1, Symbol Symbol2, Symbol Symbol3);
//    Dictionary<WinComboBits, int> WinAmounts { get; }
//}

//public abstract class MachineLogicBase(Dictionary<WinComboBits, int> WinAmounts) : IMachineLogic
//{
//    public Dictionary<WinComboBits, int> WinAmounts { get; } = WinAmounts;

//    public abstract WinComboBits GetWinCombo(Symbol Symbol1, Symbol Symbol2, Symbol Symbol3);
//}
//public class MachineLogic : MachineLogicBase
//{
//    public MachineLogic(Dictionary<WinComboBits, int> WinAmounts) : base(WinAmounts)
//    {
//    }
//    public static MachineLogic Bally831()
//    {
//        return new MachineLogic(new Dictionary<WinComboBits, int>()
//        {
//            [WinComboBits.Sevens] = (1000 + 500 + 200) / 3,
//            [WinComboBits.Bars] = 100,
//            [WinComboBits.Melons] = 20,
//            [WinComboBits.Bells] = 18,
//            [WinComboBits.Plums] = 14,
//            [WinComboBits.Oranges] = 10,
//            [WinComboBits.TwoCherries] = 5,
//            [WinComboBits.OneCherry] = 2
//        });
//    }
//    public override WinComboBits GetWinCombo(Symbol Symbol1, Symbol Symbol2, Symbol Symbol3)
//    {
//        switch (Symbol1)
//        {
//            case Symbol.Seven:
//                if (Symbol2 == Symbol.Seven && Symbol3 == Symbol.Seven)
//                    return WinComboBits.Sevens;
//                break;
//            case Symbol.Melon:
//                if (Symbol2 == Symbol.Melon && Symbol3 == Symbol.Melon)
//                    return WinComboBits.Melons;
//                break;
//            case Symbol.Bell:
//                if (Symbol2 == Symbol.Bell && (Symbol3 == Symbol.Bell || Symbol3 == Symbol.Bar))
//                    return WinComboBits.Bells;
//                break;
//            case Symbol.Plum:
//                if (Symbol2 == Symbol.Plum && (Symbol3 == Symbol.Plum || Symbol3 == Symbol.Bar))
//                    return WinComboBits.Plums;
//                break;
//            case Symbol.Orange:
//                if (Symbol2 == Symbol.Orange && (Symbol3 == Symbol.Orange || Symbol3 == Symbol.Bar))
//                    return WinComboBits.Oranges;
//                break;
//            case Symbol.Cherry:
//                if (Symbol2 == Symbol.Cherry)
//                    return WinComboBits.TwoCherries;
//                else
//                    return WinComboBits.OneCherry;
//        }
//        return 0;
//    }
//}
//public record struct SymbolCount(Symbol Symbol, int Count);
