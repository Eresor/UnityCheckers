using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class Heuristics
    {
        static float MinGrade;
        static float MaxGrade;

        public static float SimpleGrade(CheckerData[,] board, int size, Player currentPlayer)
        {
            float ret = 0.0f;
            const float basicGrade = 1.0f;
            for(int y=0;y<size;y++)
            {
                for(int x=0;x<size;x++)
                {
                    CheckerData current = board[x, y];
                    if(current!=null)
                    {
                        if (current.Owner == currentPlayer)
                        {
                            if(x==0 || x==size-1)
                            {
                                ret += basicGrade;
                            }

                            ret+= basicGrade;

                            if ((currentPlayer.Direction == 1 && y == size - 1) || (currentPlayer.Direction == -1 && y == 0))
                            {
                                ret += 5 * basicGrade;
                            }
                        }
                        else
                        {
                            ret -= basicGrade;

                            if ((currentPlayer.Direction == 1 && y == 0) || (currentPlayer.Direction == -1 && y == size-1))
                            {
                                ret -= 5 * basicGrade;
                            }
                        }

                    }
                }
            }
            return ret;
        }
    }
}
