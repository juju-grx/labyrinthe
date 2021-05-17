using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    [SerializeField]
    private int sizeMap = 16;
    [SerializeField]
    private List<GameObject> chunksPrefabs = new List<GameObject>();

    private const byte Nord = 8;
    private const byte Est = 4;
    private const byte Sud = 2;
    private const byte Ouest = 1;

    private int[,] chunksValue = new int[128, 128];
    private int[,] gradientValue = new int[128, 128];
    private GameObject[,] chunks = new GameObject[128, 128];

    private int ordre = 0;
    public bool mapAleatoire;
    public bool mapSpecific;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ClearChunks();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ClearChunks();
            for (int i = 0; i < sizeMap; i++)
            {
                for (int j = 0; j < sizeMap; j++)
                {
                    gradientValue[i, j] = 0;
                    if (i == 0 || i == sizeMap - 1 || j == 0 || j == sizeMap - 1)
                    {
                        chunksValue[i,j] = -1;
                    } else
                    {
                        chunksValue[i, j] = 0;
                    }
                }
            }

            MapGeneration(1, 1, 1, 1);
            if(mapSpecific)
            { SpecificMapGenerationAdditionnal(); }
            Generation();
        }
    }

    private void MapGeneration(int i0, int j0, int i, int j )
	{
        if (chunksValue[i, j] == -1 ) { return; }
        if (chunksValue[i, j] != 0  ) { return; }

        { // Détermination des connections
            if(i0 == i && j0 < j)
            {
                chunksValue[i, j] |= Nord;
                chunksValue[i0, j0] |= Sud;
            }
            if(i0 > i && j0 == j)
            {
                chunksValue[i, j] |= Est;
                chunksValue[i0, j0] |= Ouest;
            }
            if(i0 == i && j0 > j)
            {
                chunksValue[i, j] |= Sud;
                chunksValue[i0, j0] |= Nord;
            }
            if(i0 < i && j0 == j)
            {
                chunksValue[i, j] |= Ouest;
                chunksValue[i0, j0] |= Est;
            }
        }
        
        int[] voisine = new int[8];
 
        { // Choix aléatoire de la voisine à visiter
            if(mapAleatoire)
            {
                ordre = Random.Range(1, 4);
            }
            else
            {
                if(ordre < 5)
                {
                    ordre ++;
                } else 
                {
                    ordre = 1;
                }
            }
            
            if (ordre == 1)
            { 
                voisine[0] = i - 1; voisine[1] = j    ;
                voisine[2] = i + 1; voisine[3] = j    ;
                voisine[4] = i    ; voisine[5] = j - 1;
                voisine[6] = i    ; voisine[7] = j + 1;
            }
            if (ordre == 2)
            {
                voisine[0] = i + 1; voisine[1] = j    ;
                voisine[2] = i    ; voisine[3] = j - 1;
                voisine[4] = i    ; voisine[5] = j + 1;
                voisine[6] = i - 1; voisine[7] = j    ;
            }
            if (ordre == 3)
            {
                voisine[0] = i    ; voisine[1] = j - 1;
                voisine[2] = i    ; voisine[3] = j + 1;
                voisine[4] = i - 1; voisine[5] = j    ;
                voisine[6] = i + 1; voisine[7] = j    ;
            }
            if (ordre == 4)
            {
                voisine[0] = i    ; voisine[1] = j + 1;
                voisine[2] = i - 1; voisine[3] = j    ;
                voisine[4] = i + 1; voisine[5] = j    ;
                voisine[6] = i    ; voisine[7] = j - 1;
            }
        }

        { // Appel des voisines
            MapGeneration(i, j, voisine[0], voisine[1]);
            MapGeneration(i, j, voisine[2], voisine[3]);
            MapGeneration(i, j, voisine[4], voisine[5]);
            MapGeneration(i, j, voisine[6], voisine[7]);
        }
    }

    private void SpecificMapGenerationAdditionnal()
    {
        //PropagerGradient(1, 1);
        for (int i = 1; i < sizeMap - 1 ; i++)
        {
            for (int j = 1; j < sizeMap - 1; j++)
            {
                if(chunksValue[i, j] == 1 || chunksValue[i, j] == 2 || chunksValue[i, j] == 4 || chunksValue[i, j] == 8)
                {
                    //Debug.Log(i +":"+ j + " ------------------------- ");
                    PropagerGradient(i, j);
                }
            }
        }
    }

    private void PropagerGradient(int i0, int j0)
    {
        for (int i = 1; i < sizeMap - 1 ; i++)
        {
            for (int j = 1; j < sizeMap - 1; j++)
            {
                gradientValue[i, j] = 0;
            }
        }
        //Debug.Log("-> " + i0 + ":" + j0);
        gradientValue[i0, j0] = 1;
        bool modif = true;
        int value = 1;
        int numV = 0;
        int[] tbV= new int[2] {i0, j0};

        if(chunksValue[i0 + 1, j0] == -1)
        { numV++; }
        if(chunksValue[i0 - 1, j0] == -1)
        { numV++; }
        if(chunksValue[i0, j0 + 1] == -1)
        { numV++; }
        if(chunksValue[i0, j0 - 1] == -1)
        { numV++; }

        //for (int d = 0; d < 100 ; d++)
        while (modif)
        {
            //Debug.Log("-------------------------------------------------------------------------------------------------------");
            for (int i = 1; i < sizeMap - 1 ; i++)
            {
                for (int j = 1; j < sizeMap - 1; j++)
                {
                    //Debug.Log("value Co: " + i + ":" + j + " => " + gradientValue[i, j] + " -> value: " + value);
                    if(gradientValue[i, j] == value)
                    {
                        //Debug.Log(chunksValue[i + 1, j] + " : " + chunksValue[i - 1, j] + " : " + chunksValue[i, j + 1] + " : " + chunksValue[i, j - 1]);
                        if(chunksValue[i + 1, j] == 1 || chunksValue[i + 1, j] == 3 || chunksValue[i + 1, j] == 5 || chunksValue[i + 1, j] == 7 || chunksValue[i + 1, j] == 9 || chunksValue[i + 1, j] == 11 || chunksValue[i + 1, j] == 13 || chunksValue[i + 1, j] == 15)
                        {
                            //Debug.Log("Est");
                            if(gradientValue[i + 1, j] == 0)
                            {
                                //Debug.Log( (i + 1) + ":" + j + " -> value + 1");
                                gradientValue[i + 1, j] = value + 1;
                            }
                        }
                        if(chunksValue[i - 1, j] == 4 || chunksValue[i - 1, j] == 5 || chunksValue[i - 1, j] == 6 || chunksValue[i - 1, j] == 7 || chunksValue[i - 1, j] == 12 || chunksValue[i - 1, j] == 13 || chunksValue[i - 1, j] == 14 || chunksValue[i - 1, j] == 15)
                        {
                            //Debug.Log("Ouest");
                            if(gradientValue[i - 1, j] == 0)
                            {
                                //Debug.Log( (i - 1) + ":" + j + " -> value + 1");
                                gradientValue[i - 1, j] = value + 1;
                            }
                        }
                        if(chunksValue[i, j + 1] == 8 || chunksValue[i, j + 1] == 9 || chunksValue[i, j + 1] == 10 || chunksValue[i, j + 1] == 11 || chunksValue[i, j + 1] == 12 || chunksValue[i, j + 1] == 13 || chunksValue[i, j + 1] == 14 || chunksValue[i, j + 1] == 15)
                        {
                            //Debug.Log("Sud");
                            if(gradientValue[i, j + 1] == 0)
                            {
                                //Debug.Log( i + ":" + (j + 1) + " -> value + 1");
                                gradientValue[i, j + 1] = value + 1;
                            }
                        }
                        if(chunksValue[i, j - 1] == 2 || chunksValue[i, j - 1] == 3 || chunksValue[i, j - 1] == 6 || chunksValue[i, j - 1] == 7 || chunksValue[i, j - 1] == 10 || chunksValue[i, j - 1] == 11 || chunksValue[i, j - 1] == 14 || chunksValue[i, j - 1] ==  15)
                        {
                            //Debug.Log("Nord");
                            if(gradientValue[i, j - 1] == 0)
                            {
                                //Debug.Log( i + ":" + (j - 1)+ " -> value + 1");
                                gradientValue[i, j - 1] = value + 1;
                            }
                        }

                        if(gradientValue[i + 1, j] == gradientValue[i0, j0])
                        {
                            //Debug.Log("voisine 1");
                            numV++;
                            if(gradientValue[i, j] > gradientValue[tbV[0], tbV[1]])
                            {
                                tbV[0] = i;
                                tbV[1] = j;
                            }
                        }
                        if(gradientValue[i - 1, j] == gradientValue[i0, j0])
                        {   
                            //Debug.Log("voisine 2");
                            numV++;
                            if(gradientValue[i, j] > gradientValue[tbV[0], tbV[1]])
                            {
                                tbV[0] = i;
                                tbV[1] = j;
                            }
                        }
                        if(gradientValue[i, j + 1] == gradientValue[i0, j0])
                        {   
                            //Debug.Log("voisine 3");
                            numV++;
                            if(gradientValue[i, j] > gradientValue[tbV[0], tbV[1]])
                                {
                                    tbV[0] = i;
                                    tbV[1] = j;
                                }
                        }
                        if(gradientValue[i, j - 1] == gradientValue[i0, j0])
                        {   
                            //Debug.Log("voisine 4");
                            numV++;
                            if(gradientValue[i, j] > gradientValue[tbV[0], tbV[1]])
                                {
                                    tbV[0] = i;
                                    tbV[1] = j;
                                }
                        }
                        
                        
                    }
                    //Debug.Log("numVoisine: " + numV);
                    if(numV == 4)
                    {
                        int sizeBoucle;
                        //Debug.Log("okokok");
                        if(sizeMap >= 16)
                        {
                            sizeBoucle = sizeMap/2;
                        } else 
                        { sizeBoucle = 8; }
                        if(gradientValue[tbV[0], tbV[1]] >= sizeBoucle)
                        {
                            //Debug.Log("nice");
                            if(i0 + 1 == tbV[0] && j0 == tbV[1])
                            {
                                //Debug.Log("ouverture Est");
                                chunksValue[i0, j0] |= Est;
                                chunksValue[tbV[0], tbV[1]] |= Ouest;
                            }
                            if(i0 - 1 == tbV[0] && j0 == tbV[1])
                            {
                                //Debug.Log("ouverture Ouest");
                                chunksValue[i0, j0] |= Ouest;
                                chunksValue[tbV[0], tbV[1]] |= Est;
                            }
                            if(i0 == tbV[0] && j0 + 1== tbV[1])
                            {
                                //Debug.Log("ouverture Sud");
                                chunksValue[i0, j0] |= Sud;
                                chunksValue[tbV[0], tbV[1]] |= Nord;
                            }
                            if(i0 == tbV[0] && j0 - 1== tbV[1])
                            {
                                //Debug.Log("ouverture Nord");
                                chunksValue[i0, j0] |= Nord;
                                chunksValue[tbV[0], tbV[1]] |= Sud;
                            }
                        }
                        modif = false;
                    }
                }
            }
            value ++;
            //Debug.Log(numV);
            //modif = false;
        }

    }

    private void Generation()
    {
        int numPrefabs = 5;
        int orientation = 0;

        for (int i = 0; i < sizeMap; i++)
        {
            for (int j = 0; j < sizeMap; j++)
            {
                numPrefabs = 5;
                orientation = 0;
                
                { // définition de la prefabs et de son angle
                if (chunksValue[i, j] == -1) { numPrefabs = 5; orientation =   0; }// Debug.Log("Wall"                             ); }
                if (chunksValue[i, j] ==  1) { numPrefabs = 0; orientation = 180; }// Debug.Log("stop -> ouest"                    ); }
                if (chunksValue[i, j] ==  2) { numPrefabs = 0; orientation =  90; }// Debug.Log("stop -> sud"                      ); }
                if (chunksValue[i, j] ==  3) { numPrefabs = 2; orientation = 270; }// Debug.Log("angle -> sud-ouest"               ); }
                if (chunksValue[i, j] ==  4) { numPrefabs = 0; orientation =   0; }// Debug.Log("stop -> est"                      ); }
                if (chunksValue[i, j] ==  5) { numPrefabs = 1; orientation =   0; }// Debug.Log("droit -> est-ouest"               ); }
                if (chunksValue[i, j] ==  6) { numPrefabs = 2; orientation = 180; }// Debug.Log("angle -> est-sud"                 ); }
                if (chunksValue[i, j] ==  7) { numPrefabs = 3; orientation = 180; }// Debug.Log("intersection -> est-sud-ouest"    ); }
                if (chunksValue[i, j] ==  8) { numPrefabs = 0; orientation = 270; }// Debug.Log("stop -> nord"                     ); }
                if (chunksValue[i, j] ==  9) { numPrefabs = 2; orientation =   0; }// Debug.Log("angle -> nord-ouest"              ); }
                if (chunksValue[i, j] == 10) { numPrefabs = 1; orientation =  90; }// Debug.Log("droit -> nord-sud"                ); }
                if (chunksValue[i, j] == 11) { numPrefabs = 3; orientation = 270; }// Debug.Log("intersection -> nord-ouest-sud"   ); }
                if (chunksValue[i, j] == 12) { numPrefabs = 2; orientation =  90; }// Debug.Log("angle -> nord-est"                ); }
                if (chunksValue[i, j] == 13) { numPrefabs = 3; orientation =   0; }// Debug.Log("intersection -> est-nord-ouest"   ); }
                if (chunksValue[i, j] == 14) { numPrefabs = 3; orientation =  90; }// Debug.Log("intersection -> nord-est-sud"     ); }
                if (chunksValue[i, j] == 15) { numPrefabs = 4; orientation =   0; }// Debug.Log("croissement -> nord-est-sud-ouest"); }
                }

                GameObject chunk = Instantiate(chunksPrefabs[numPrefabs], new Vector3(j * 15, 50, i * 15), Quaternion.Euler(new Vector3(0, orientation, 0)));
                chunks[i,j] = chunk;
            }
        }
    }
    private void ClearChunks()
    {
        for (int i = 0; i < sizeMap; i++)
        {
            for (int j = 0; j < sizeMap; j++)
            {
                chunksValue[i, j] = 0;
                Destroy(chunks[i,j], 2f);
                chunks[i,j] = null;
            }
        }
    }
}
