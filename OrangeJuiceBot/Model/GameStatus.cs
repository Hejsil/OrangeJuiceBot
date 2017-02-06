using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeJuiceBot.Model
{
    public enum GameStatus
    {
        YourTurn,
        ChallengeToBattle,
        DoYouWantToStopAtHome,
        ChooseACardToUseField,
        ChooseACardToUseBattle,
        ChooseACardToDiscard,
        ChoosePanelToMoveTo,

        RollDiceMove,
        RollDiceBonus,
        RollDiceDrop,
        RollDiceAttacker,

        MakeDefensiveChoiceAttacker,

        NotYourTurn,
    }
}
