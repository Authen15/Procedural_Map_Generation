// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// //TODO changer newCellsArray en newCellsArrayColors, et ne stocker que les couleurs pour les appliquers apres (gagne de la ram)

// public class CellBlur
// {
//     HexMapData hexMapData;
//     Color[,] newColorsArray;

//     public CellBlur(HexMapData hexMapData)
//     {
//         this.hexMapData = hexMapData;
//         newColorsArray = new Color[hexMapData.size, hexMapData.size];
//     }


//     public void BlurAllCells(HexMapData hexMapData, int blurRange)
//     {
        
//         for(int i = blurRange; i < hexMapData.size - blurRange; i++){
//             for(int j = blurRange; j < hexMapData.size - blurRange; j++){
//                 HexCell cell = CellsArray[i, j];
//                 newColorsArray[i,j] = GetBlurCellColor(cell, blurRange);
//             }
//         }

//         for(int i = blurRange; i < hexMapData.size -1  - blurRange; i++){
//             for(int j = blurRange; j < hexMapData.size -1  - blurRange; j++){
//                 CellsArray[i, j].gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", newColorsArray[i,j]);
//             }
//         }

        
//     }

//     private Color GetBlurCellColor(HexCell cell, int blurSize)
//     {
//         // if cell is water then don't blur
//         if(cell.BiomeLevel.isWater){
//             return cell.CellObject.GetComponent<Renderer>().material.GetColor("_BaseColor");
//         }

//         List<HexCell> neighbours = new List<HexCell>();
//         // if all cells are of the same biome then don't blur
//         if(!GetNeighboursCells(cell, neighbours, blurSize)){
//             return cell.CellObject.GetComponent<Renderer>().material.GetColor("_BaseColor");
//         }

//         Color transitionColor = GetAverageColor(neighbours);
//         return transitionColor;
//     }


//     // return null if all neighbours are of the same biome
//     private bool GetNeighboursCells(HexCell cell, List<HexCell> neighbours, int blurSize)
//     {
//         bool isAtLeastOneDifferentBiome = false;

//         for (int x = - blurSize; x <= blurSize; x++)
//         {
//             for (int z = - blurSize; z <= blurSize; z++)
//             {
//                 HexCell neighbour = CellsArray[cell.X + x, cell.Z + z];
//                 // check if neighbour is null 
//                 if(neighbour == null){
//                     Debug.Log("neighbour is null" + cell.X + x + " " + cell.Z + z);
//                     continue;
//                 }
//                 // if neighbour is water don't use it color
//                 if(neighbour.BiomeLevel.isWater){
//                     continue;
//                 }
//                 if(neighbour.Biome != cell.Biome){
//                     isAtLeastOneDifferentBiome = true;
//                 }
//                 neighbours.Add(neighbour);
//             }
//         }
//         return isAtLeastOneDifferentBiome;
//     }

//     private Color GetAverageColor(List<HexCell> neighbours)
//     {
//         Color averageColor = new Color(0, 0, 0, 0);
//         foreach (HexCell cell in neighbours)
//         {
//             GameObject cellObj = cell.CellObject;
//             averageColor += cellObj.GetComponent<Renderer>().material.GetColor("_BaseColor");
//         }
//         averageColor /= neighbours.Count;
//         return averageColor;
//     }


//     // ne pas comtper les cellules water
//     // blur uniquement si au moins une cellules de biome différent

// }
