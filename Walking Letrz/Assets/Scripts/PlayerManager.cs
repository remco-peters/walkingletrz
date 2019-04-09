using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class PlayerManager : MyMonoBehaviour
    {
        public List<Player> players { get; set; }

        public void NextTurn(Player player)
        {
            int index = players.IndexOf(player);
            if (index == -1) return;
            index += 1;
            if (index >= players.Count)
                index = 0;
            Player newPlayer = players[index];
            GetCurrentActivePlayer().CanMove = false;
            newPlayer.CanMove = true;
        }

        public Player GetCurrentActivePlayer()
        {
            return players.FirstOrDefault(x => x.CanMove);
        }
    }
}
