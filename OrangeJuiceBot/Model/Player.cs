using System.Text;

namespace OrangeJuiceBot.Model
{
    public class Player
    {
        public Norma Norma { get; set; }
        public int Stars { get; set; }
        public int Wins { get; set; }

        public Character Character { get; set; }
        public int HpCurrent { get; set; }
        public int HpTotal { get; set; }
        public int Attack { get; set; }
        public int Defends { get; set; }
        public int Evade { get; set; }
        public int Recovery { get; set; }

        public Card[] Cards { get; set; }
    }
}