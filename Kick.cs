using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MultiplayerTetris
{
    public class Kick
    {
        private static Vector2[][] _wallKickA = new Vector2[][]
        {

            // 0 >>
            new Vector2[] { },                                                                                                      // 0
            new Vector2[] { new Vector2(0, 0), new Vector2(-1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2) },     // 1
            new Vector2[] { },                                                                                                      // 2
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2) },       // 3
            
            // 1 >>
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2) },      // 0
            new Vector2[] { },                                                                                                      // 1
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(-1, 2) },      // 2
            new Vector2[] { },                                                                                                      // 3

           // 2 >> 
            new Vector2[] { },                                                                                                      // 0
            new Vector2[] { new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 2), new Vector2(-1, 2) },    // 1
            new Vector2[] { },                                                                                                      // 2
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2) },       // 3
            
            // 3 >>
            new Vector2[] { new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2) },   // 0
            new Vector2[] { },                                                                                                      // 1
            new Vector2[] { new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2) },   // 2
            new Vector2[] { }                                                                                                       // 3
        };
        
        private static Vector2[][] _wallKickB = new Vector2[][]
        {

            // 0 >>
            new Vector2[] { },                                                                                                      // 0
            new Vector2[] { new Vector2(0, 0), new Vector2(-2, 0), new Vector2(1, 0), new Vector2(-2, 1), new Vector2(1, -2) },     // 1
            new Vector2[] { },                                                                                                      // 2
            new Vector2[] { new Vector2(0, 0), new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, -2), new Vector2(2, 1) },       // 3
            
            // 1 >>
            new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(-1, 0), new Vector2(2, -1), new Vector2(-1, 2) },     // 0
            new Vector2[] { },                                                                                                      // 1
            new Vector2[] { new Vector2(0, 0), new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, -2), new Vector2(2, 1) },     // 2
            new Vector2[] { },                                                                                                      // 3

           // 2 >> 
            new Vector2[] { },                                                                                                      // 0
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, 2), new Vector2(-2, -1) },     // 1
            new Vector2[] { },                                                                                                      // 2
            new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(-1, 0), new Vector2(2, -1), new Vector2(-1, 2) },     // 3
            
            // 3 >>
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, 2), new Vector2(-2, -1) },     // 0
            new Vector2[] { },                                                                                                      // 1
            new Vector2[] { new Vector2(0, 0), new Vector2(-2, 0), new Vector2(1, 0), new Vector2(-2, 1), new Vector2(1, -2) },     // 2
            new Vector2[] { }                                                                                                       // 3
        };
        
        public static Vector2[] GetWallKick(Tetromino.Type type, BitArray lastRotation, BitArray newRotation)
        {
            BitArray combinedRotation = new BitArray(4);
            combinedRotation[3] = lastRotation[0];
            combinedRotation[2] = lastRotation[1];
            combinedRotation[1] = newRotation[0];
            combinedRotation[0] = newRotation[1];



            int[] array = new int[1];
            combinedRotation.CopyTo(array, 0);
            int rotationInt = array[0];



            switch (type)
            {
                case Tetromino.Type.J:
                case Tetromino.Type.L:
                case Tetromino.Type.T:
                case Tetromino.Type.S:
                case Tetromino.Type.Z:
                    return _wallKickA[rotationInt];
                case Tetromino.Type.I:
                    
                    return _wallKickB[rotationInt];
                default:
                    return new Vector2[5];
            }
        }
    }
}